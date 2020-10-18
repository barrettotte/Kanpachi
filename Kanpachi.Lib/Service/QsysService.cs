using System;
using System.IO;
using System.Text;
using Kanpachi.Lib;

namespace Kanpachi.CLI{

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
            var clientPath = string.Empty;
            var lib = splitPath[0];
            var spf = splitPath[1];
            var mbr = splitPath[2];

            // TODO: move to separate function...this is gross
            if(downloadPath == null || downloadPath.Trim().Length == 0){
                if(Profile.DownloadPath == null || downloadPath.Trim().Length == 0){
                    clientPath = Directory.GetCurrentDirectory();
                } else{
                    clientPath = Profile.DownloadPath;
                }
            } else{
                clientPath = Path.GetFullPath(downloadPath);
            }

            clientPath = Path.Combine(clientPath, "QSYS", lib, spf);  // add QSYS directory structure
            if(!Directory.Exists(clientPath)){
                Directory.CreateDirectory(clientPath);
            }
            Console.WriteLine($"Downloading '/QSYS.LIB/{lib}.LIB/{spf}.FILE/{mbr}.MBR' to '{clientPath}'");

            using(KanpachiClient client = new KanpachiClient(Profile)){
                client.Connect();

                SrcMbr srcMbr = client.DownloadMember(lib, spf, mbr);
                string src = string.Empty;
                int rcdLen = srcMbr.RecordLength;
                byte[] record;

                for(int i = 0; i < srcMbr.Content.Length; i += rcdLen){
                    record = new byte[rcdLen];
                    Array.Copy(srcMbr.Content, i, record, 0, rcdLen);
                    src += ((i > 0) ? "\n" : "") + Encoding.ASCII.GetString(record).TrimEnd();
                }
                var outPath = Path.Combine(clientPath, $"{srcMbr.Name}.{srcMbr.Type}");
                File.WriteAllText(outPath, src);
                Console.WriteLine($"Successfully wrote '{outPath}'");
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
                var members = client.GetMemberListDetailed(splitPath[0], splitPath[1]);
                foreach(var mbr in members){
                    Console.WriteLine(mbr);
                    // TODO: better formatting
                }
            }
        }
    }
}