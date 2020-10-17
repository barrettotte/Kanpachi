using System;
using System.IO;
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
        public void GetMember(string memberPath, string downloadPath){
            var splitMbr = memberPath.ToUpper().Split('/');
            
            // TODO: validate member path with regex instead of this
            if(splitMbr.Length != 3){
                throw new KanpachiException("Expected member path of format LIB/SRCPF/MBR");
            }
            string serverPath = $"/QSYS.LIB/{splitMbr[0]}.LIB/{splitMbr[1]}.FILE/{splitMbr[2]}.MBR";
            string clientPath = string.Empty;

            if(downloadPath == null || downloadPath.Trim().Length == 0){
                if(Profile.DownloadPath == null || downloadPath.Trim().Length == 0){
                    clientPath = Directory.GetCurrentDirectory();
                } else{
                    clientPath = Profile.DownloadPath;
                }
            } else{
                clientPath = Path.GetFullPath(downloadPath);
            }
            Console.WriteLine($"Downloading '{serverPath}' to '{clientPath}'");

            using(KanpachiClient client = new KanpachiClient(Profile)){
                Console.WriteLine(Profile.PasswordDecrypted);
                client.Connect();
                // byte[] src = client.DownloadMember(serverPath);
                // File.WriteAllBytes(Path.Combine(clientPath, "mbr.txt"), src); 
                // TODO: file name based on source member type
            }
        }
    }
}