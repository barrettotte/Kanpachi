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

        // TODO: regex for naming
        //  LIB   - [A-Za-z@#$][A-Za-z0-9._@#$]{9}
        //  SRCPF - 
        //  MBR   -  

        // BOLIB/QRPGSRC/HELLORPG => /QSYS.LIB/BOLIB.LIB/QRPGSRC.FILE/HELLORPG.MBR
        public void GetMember(string qsysPath, string downloadPath){
            var splitPath = qsysPath.ToUpper().Split('/');

            // TODO: validate member path with regex instead of this
            if(splitPath.Length != 3){
                throw new KanpachiFormatException("Expected member path of format LIB/SRCPF/MBR");
            }
            (string lib, string spf, string mbr) = (splitPath[0], splitPath[1], splitPath[2]);
            
            var srcPath = $"/QSYS.LIB/{lib}.LIB/{spf}.FILE/{mbr}.MBR";
            var clientPath = BuildQsysPath(downloadPath, lib, spf);
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

        public void GetMemberList(string serverPath){
            var splitPath = serverPath.ToUpper().Split('/');

            // TODO: validate path with regex instead of this
            if(splitPath.Length != 2){
                throw new KanpachiFormatException("Expected source physical file path of format SRCPF/MBR");
            }

            using(KanpachiClient client = new KanpachiClient(Profile)){
                client.Connect();
                var members = client.GetSrcMbrList(splitPath[0], splitPath[1]);
                foreach(var mbr in members){
                    Console.WriteLine(mbr);
                    // TODO: better formatting
                }
            }
        }

        public void GetSpf(string serverPath, string downloadPath){
            var splitPath = serverPath.ToUpper().Split('/');

            // TODO: validate path with regex instead of this
            if(splitPath.Length != 2){
                throw new KanpachiFormatException("Expected source physical file path of format SRCPF/MBR");
            }
            (string lib, string spf) = (splitPath[0], splitPath[1]);

            using(KanpachiClient client = new KanpachiClient(Profile)){
                client.Connect();
                SrcPf srcPf = client.GetSrcPfDetails(lib, spf);
                var members = client.GetSrcMbrList(lib, spf);

                foreach(SrcMbr mbr in members){
                    var qsysPath = $"/QSYS.LIB/{lib}.LIB/{spf}.FILE/{mbr.Name}.MBR";
                    var clientPath = BuildQsysPath(downloadPath, lib, spf);
                    Console.WriteLine($"Downloading {qsysPath} to {clientPath}");
                    mbr.Content = client.DownloadMember(qsysPath);

                    // TODO: save metadata to JSON file, root of library
                    var outPath = Path.Combine(clientPath, $"{mbr.Name}.{mbr.Attribute}");
                    File.WriteAllText(outPath, string.Join("\n", SplitSrcMbr(mbr)));
                }
            }
        }

        public void GetLibrary(string lib, string downloadPath){
            // TODO: validate path with regex

            using(KanpachiClient client = new KanpachiClient(Profile)){
                client.Connect();
                Library library = client.GetLibraryDetails(lib);

                foreach(SrcPf spf in client.GetSpfList(lib)){
                    foreach(SrcMbr mbr in client.GetSrcMbrList(lib, spf.Name)){
                        var qsysPath = $"/QSYS.LIB/{library.Name}.LIB/{spf.Name}.FILE/{mbr.Name}.MBR";
                        
                        var clientPath = BuildQsysPath(downloadPath, library.Name, spf.Name);
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

        // build QSYS directory structure
        private string BuildQsysPath(string clientPath, string lib, string spf){
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