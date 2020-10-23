using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kanpachi.Lib{

    public class QsysService: BaseService{

        public KanpachiProfile Profile {get;}


        public QsysService(KanpachiProfile profile){
            Profile = profile;
        }

        // Download entire library
        public void GetLibrary(string lib, string downloadPath){
            if(!RegexUtils.MatchIbmiObject(lib)){
                throw new KanpachiFormatException("Invalid library name");
            }
            using(KanpachiClient client = new KanpachiClient(Profile)){
                var spfs = client.GetSrcPfList(lib);
                foreach(SrcPf spf in spfs){
                    var members = client.GetSrcMbrList(lib, spf.Name);
                    foreach(SrcMbr srcMbr in members){
                        // TODO: progress output
                        DownloadSrcMbr(client, downloadPath, lib, spf.Name, srcMbr);
                    }
                }
            }
        }

        // Download source physical file at LIB/SRCPF
        public void GetSpf(string qsysPath, string downloadPath){
            if(!RegexUtils.MatchSrcPfPath(qsysPath)){
                throw new KanpachiFormatException("Invalid QSYS path. Expected source physical file path of format LIB/SRCPF.");
            }
            var splitPath = qsysPath.ToUpper().Split('/');
            (string lib, string spf) = (splitPath[0], splitPath[1]);

            using(KanpachiClient client = new KanpachiClient(Profile)){
                SrcPf srcPf = client.GetSrcPfDetails(lib, spf);
                var members = client.GetSrcMbrList(lib, spf);

                foreach(SrcMbr srcMbr in members){
                    // TODO: progress output
                    DownloadSrcMbr(client, downloadPath, lib, spf, srcMbr);
                }
            }
        }

        // Download source member at LIB/SRCPF/MBR
        public void GetMember(string qsysPath, string downloadPath){
            if(!RegexUtils.MatchSrcMbrPath(qsysPath)){
                throw new KanpachiFormatException("Expected source member path of format LIB/SRCPF/MBR.");
            }
            var splitPath = qsysPath.ToUpper().Split('/');
            (string lib, string spf, string mbr) = (splitPath[0], splitPath[1], splitPath[2]);

            var srcPath = $"/QSYS.LIB/{lib}.LIB/{spf}.FILE/{mbr}.MBR";
            var clientPath = BuildLocalQsysPath(downloadPath, lib, spf);

            using(KanpachiClient client = new KanpachiClient(Profile)){
                SrcMbr srcMbr = client.GetSrcMbrDetails(lib, spf, mbr);
                DownloadSrcMbr(client, downloadPath, lib, spf, srcMbr);
            }
        }

        // Display list of libraries on active profile's host
        public void ListLibraries(){
            using(KanpachiClient client = new KanpachiClient(Profile)){
                foreach(Library lib in client.GetLibraries()){
                    Console.WriteLine(lib);
                }
            }
        }

        // Display list of source physical files
        public void ListSpfs(string qsysPath){
            if(!RegexUtils.MatchIbmiObject(qsysPath)){
                throw new KanpachiFormatException("Expected name of library.");
            }
            using(KanpachiClient client = new KanpachiClient(Profile)){
                foreach(SrcPf spf in client.GetSrcPfList(qsysPath)){
                    Console.WriteLine(spf);
                }
            }
        }

        // Display list of source members in LIB/SRCPF
        public void ListMembers(string qsysPath){
            if(!RegexUtils.MatchSrcPfPath(qsysPath)){
                throw new KanpachiFormatException("Expected source physical file path of format LIB/SRCPF.");
            }
            var splitPath = qsysPath.ToUpper().Split('/');
            (string lib, string spf) = (splitPath[0], splitPath[1]);

            using(KanpachiClient client = new KanpachiClient(Profile)){
                foreach(SrcMbr mbr in client.GetSrcMbrList(lib, spf)){
                    Console.WriteLine(mbr);
                }
            }
        }

        // split src member from byte array to records, based on record length of source physical file
        private List<string> SplitSrcMbr(SrcMbr srcMbr){
            List<string> src = new List<string>();
            int rcdLen = srcMbr.RecordLength - 12; // Ex: RPG  92-12 = 80 columns
            byte[] record;

            for(int i = 0; i < srcMbr.Content.Length; i += rcdLen){
                record = new byte[rcdLen];
                Array.Copy(srcMbr.Content, i, record, 0, rcdLen);
                src.Add(Encoding.UTF8.GetString(record).TrimEnd());
            }
            return src;
        }

        // write QSYS metadata (TEXT, RECORDLEN, ATTRIBUTE, etc)
        private void WriteQsysMetadata(KanpachiClient client, string downloadPath, string lib, string spf, SrcMbr mbr){
            Library library = null;
            var metadataPath = Path.Combine(ClientUtils.BuildDownloadPath(downloadPath, Profile), "QSYS", lib, $"{lib}.json");
            
            if(File.Exists(metadataPath)){
                // read existing metadata file for update
                using(StreamReader f = File.OpenText(metadataPath)){
                    library = (Library) new JsonSerializer().Deserialize(f, typeof(Library));
                    int spfIdx = library.SrcPfs.FindIndex(x => x.Name == spf);

                    // source physical file not found in library metadata, so add it
                    if(spfIdx == -1){
                        library.SrcPfs.Add(client.GetSrcPfDetails(lib, spf));
                        spfIdx = library.SrcPfs.Count - 1;
                    }
                    
                    SrcPf srcPf = library.SrcPfs[spfIdx];
                    int mbrIdx = srcPf.Members.FindIndex(x => x.Name == mbr.Name);

                    // source member not found in source physical file metadata, so add it
                    if(mbrIdx == -1){
                        srcPf.Members.Add(mbr);
                        mbrIdx = srcPf.Members.Count - 1;
                    }
                    library.SrcPfs[spfIdx].Members[mbrIdx] = mbr;
                }
            } else{
                // library doesn't exist in metadata, setup new one
                library = client.GetLibraryDetails(lib);
                SrcPf srcPf = client.GetSrcPfDetails(lib, spf);
                srcPf.Members.Add(mbr);
                library.SrcPfs.Add(srcPf);
            }

            // update metadata file
            using(StreamWriter f = File.CreateText(metadataPath)){
                f.Write(JsonConvert.SerializeObject(library, Formatting.Indented, new JsonSerializerSettings{
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
            }
        }

        // download source member and write to local file
        private void DownloadSrcMbr(KanpachiClient client, string downloadPath, string lib, string spf, SrcMbr srcMbr){
            var outPath = Path.Combine(BuildLocalQsysPath(downloadPath, lib, spf), $"{srcMbr.Name}.{srcMbr.Attribute}");
            var qsysPath = $"/QSYS.LIB/{lib}.LIB/{spf}.FILE/{srcMbr.Name}.MBR";

            WriteQsysMetadata(client, downloadPath, lib, spf, srcMbr);
            Console.WriteLine($"Downloading {qsysPath} to {outPath}");

            srcMbr.Content = client.DownloadMember(qsysPath);
            File.WriteAllText(outPath, string.Join("\n", SplitSrcMbr(srcMbr)));
        }

        // build local QSYS directory structure
        private string BuildLocalQsysPath(string clientPath, string lib, string spf){
            var basePath = ClientUtils.BuildDownloadPath(clientPath, Profile);
            var qsysPath = Path.Combine(basePath, "QSYS", lib, spf);

            if(!Directory.Exists(qsysPath)){
                Directory.CreateDirectory(qsysPath);
            }
            return qsysPath;
        }
    }
}