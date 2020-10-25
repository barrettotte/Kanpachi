using Renci.SshNet.Sftp;
using System;
using System.IO;
using System.Text;

namespace Kanpachi.Lib{

    public class IfsService: BaseService{

        public KanpachiProfile Profile {get;}


        public IfsService(KanpachiProfile profile){
            Profile = profile;
        }

        // List contents of IFS directory
        public void ListDirectory(string serverPath){
            var ifsPath = (serverPath == null || serverPath.Length == 0) ? Profile.IfsUserPath : serverPath;

            using(KanpachiClient client = new KanpachiClient(Profile)){
                var entries = client.ListDirectory(ifsPath);
                foreach(SftpFile entry in client.ListDirectory(ifsPath)){
                    Console.WriteLine(entry.FullName);
                }
            }
        }

        // Download file from IFS
        public void GetFile(string serverPath, string clientPath){
            var fileName = Path.GetFileName(serverPath);
            var outPath = ClientUtils.BuildDownloadPath(clientPath, Profile);

            if(!Path.HasExtension(outPath)){
                outPath = Path.Combine(outPath, fileName);
            }
            using(KanpachiClient client = new KanpachiClient(Profile)){
                Console.WriteLine($"Downloading {serverPath} to {outPath}");
                File.WriteAllText(outPath, Encoding.UTF8.GetString(client.DownloadFile(serverPath)));
            }
        }

        // Download a directory from IFS
        public void GetDirectory(string serverPath, string clientPath){
            var outPath = ClientUtils.BuildDownloadPath(clientPath, Profile);

            using(KanpachiClient client = new KanpachiClient(Profile)){
                Console.WriteLine($"Downloading {serverPath} to {clientPath}");
                GetDirectoryRecursively(client, serverPath, outPath);
            }
        }

        // download each file and directory recursively from IFS
        private void GetDirectoryRecursively(KanpachiClient client, string serverPath, string outPath){
            Console.WriteLine($"Downloading directory {serverPath} to {outPath}");

            foreach(SftpFile f in client.ListDirectory(serverPath)){
                if(f.Name != "." && f.Name != ".."){
                    if(f.IsDirectory){
                        var outSubPath = Path.Combine(outPath, f.Name);
                        if(!Directory.Exists(outSubPath)){
                            Directory.CreateDirectory(outSubPath);
                        }
                        GetDirectoryRecursively(client, f.FullName, outSubPath);
                    } else{
                        var outFile = Path.Combine(outPath, f.Name.Replace('/', Path.DirectorySeparatorChar));
                        Console.WriteLine($"Downloading file {f.FullName} to {outFile}");
                        File.WriteAllText(outFile, Encoding.UTF8.GetString(client.DownloadFile(f.FullName)));
                    }
                }
            }
        }
    }
}