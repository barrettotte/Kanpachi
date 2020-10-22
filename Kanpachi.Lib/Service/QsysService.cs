using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kanpachi.Lib{

    public class QsysService: BaseService{

        public KanpachiProfile Profile {get;}

        public QsysService(KanpachiProfile profile){
            Profile = profile;
        }

        public void GetMember(string qsysPath, string downloadPath){
            if(!RegexUtils.MatchSrcMbrPath(qsysPath)){
                throw new KanpachiFormatException("Expected source member path of format LIB/SRCPF/MBR");
            }
            var splitPath = qsysPath.ToUpper().Split('/');
            (string lib, string spf, string mbr) = (splitPath[0], splitPath[1], splitPath[2]);
            
            var srcPath = $"/QSYS.LIB/{lib}.LIB/{spf}.FILE/{mbr}.MBR";
            var clientPath = BuildLocalQsysPath(downloadPath, lib, spf);
            Console.WriteLine($"Downloading {srcPath}  => {clientPath}");

            using(KanpachiClient client = new KanpachiClient(Profile)){
                client.Connect();

                SrcMbr srcMbr = client.GetSrcMbrDetails(lib, spf, mbr);
                srcMbr.Content = client.DownloadMember(srcPath);

                // TODO: save metadata to JSON file, root of library
                var outPath = Path.Combine(clientPath, $"{srcMbr.Name}.{srcMbr.Attribute}");
                File.WriteAllText(outPath, string.Join("\n", SplitSrcMbr(srcMbr)));
                Console.WriteLine($"Successfully wrote {outPath}");
            }
        }

        public void GetMemberList(string qsysPath){
            if(!RegexUtils.MatchSrcPfPath(qsysPath)){
                throw new KanpachiFormatException("Expected source physical file path of format LIB/SRCPF");
            }
            var splitPath = qsysPath.ToUpper().Split('/');

            using(KanpachiClient client = new KanpachiClient(Profile)){
                client.Connect();
                var members = client.GetSrcMbrList(splitPath[0], splitPath[1]);
                foreach(var mbr in members){
                    Console.WriteLine(mbr);
                    // TODO: better formatting
                }
            }
        }

        public void GetSpf(string qsysPath, string downloadPath){
            if(!RegexUtils.MatchSrcPfPath(qsysPath)){
                throw new KanpachiFormatException("Invalid QSYS path. Expected source physical file path of format LIB/SRCPF");
            }
            var splitPath = qsysPath.ToUpper().Split('/');
            (string lib, string spf) = (splitPath[0], splitPath[1]);

            using(KanpachiClient client = new KanpachiClient(Profile)){
                client.Connect();
                SrcPf srcPf = client.GetSrcPfDetails(lib, spf);
                var members = client.GetSrcMbrList(lib, spf);

                foreach(SrcMbr mbr in members){
                    var fullQsysPath = $"/QSYS.LIB/{lib}.LIB/{spf}.FILE/{mbr.Name}.MBR";
                    var clientPath = BuildLocalQsysPath(downloadPath, lib, spf);
                    Console.WriteLine($"Downloading {fullQsysPath} to {clientPath}");
                    mbr.Content = client.DownloadMember(fullQsysPath);

                    // TODO: save metadata to JSON file, root of library
                    var outPath = Path.Combine(clientPath, $"{mbr.Name}.{mbr.Attribute}");
                    File.WriteAllText(outPath, string.Join("\n", SplitSrcMbr(mbr)));
                }
            }
        }

        public void GetLibrary(string lib, string downloadPath){
            // TODO: validate path with regex
            if(!RegexUtils.MatchIbmiObject(lib)){
                throw new KanpachiFormatException("Invalid library name");
            }
            using(KanpachiClient client = new KanpachiClient(Profile)){
                client.Connect();
                Library library = client.GetLibraryDetails(lib);

                foreach(SrcPf spf in client.GetSpfList(lib)){
                    foreach(SrcMbr mbr in client.GetSrcMbrList(lib, spf.Name)){
                        var qsysPath = $"/QSYS.LIB/{library.Name}.LIB/{spf.Name}.FILE/{mbr.Name}.MBR";
                        
                        var clientPath = BuildLocalQsysPath(downloadPath, library.Name, spf.Name);
                        Console.WriteLine($"Downloading {qsysPath} to {clientPath}");
                        mbr.Content = client.DownloadMember(qsysPath);

                        // TODO: save metadata to JSON file, root of library
                        var outPath = Path.Combine(clientPath, $"{mbr.Name}.{mbr.Attribute}");
                        File.WriteAllText(outPath, string.Join("\n", SplitSrcMbr(mbr)));
                    }
                }
            }
        }

        // split src member from bytes to records, based on record length of source physical file
        private List<string> SplitSrcMbr(SrcMbr srcMbr){
            List<string> src = new List<string>();
            int rcdLen = srcMbr.RecordLength;
            byte[] record;

            for(int i = 0; i < srcMbr.Content.Length; i += rcdLen){
                record = new byte[rcdLen];
                Array.Copy(srcMbr.Content, i, record, 0, rcdLen);
                src.Add(Encoding.ASCII.GetString(record).TrimEnd()); // TODO: there's going to be encoding issues :)
            }
            return src;
        }

        // build local QSYS directory structure
        private string BuildLocalQsysPath(string clientPath, string lib, string spf){
            string qsysPath = Path.Combine(BuildDownloadPath(clientPath), "QSYS", lib, spf);  
            if(!Directory.Exists(qsysPath)){
                Directory.CreateDirectory(qsysPath);
            }
            return qsysPath;
        }

        // build base download path
        private string BuildDownloadPath(string downloadPath){
            string clientPath = string.Empty;

            // if no argument passed, fallback to download path defined in profile
            if(downloadPath == null || downloadPath.Trim().Length == 0){
                if(Profile.DownloadPath == null || Profile.DownloadPath.Trim().Length == 0){
                    clientPath = Directory.GetCurrentDirectory();
                } else{
                    clientPath = Profile.DownloadPath;
                }
            } else{
                clientPath = Path.GetFullPath(downloadPath);
            }

            if(!Directory.Exists(clientPath)){
                Directory.CreateDirectory(clientPath);
            }
            return clientPath;
        }
    }
}