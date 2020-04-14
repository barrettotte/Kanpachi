using System.Collections.Generic;
using System.Net;
using System.Text;

namespace IBMi.Lib.Config{

    public class IBMiConnection{

        public const int DFT_SSH_PORT = 22;

        public NetworkCredential Credentials {get;}
        public string Host {get;} 
        public string User {get;}                // TODO: validate username size <= 10 + regex
        public int Port {get; set;}              // TODO: validate port range [0-65536]

        public string Library {get; set;}        // TODO: validate library size <= 10 + regex
        public string IfsPath {get; set;}        // TODO: validate path with regex

        public Encoding DefaultEncoding {get; set;}
        public List<Encoding> FallbackEncodings {get; set;}
        
        public IBMiConnection(NetworkCredential creds, int port){
            this.Credentials = creds;
            this.User = creds.UserName;     
            this.Host = creds.Domain;
            this.Port = port;               
            this.Library = "QTEMP";  
            this.IfsPath = "/home/" + this.User + "/";
            this.DefaultEncoding = Encoding.UTF8;
            this.FallbackEncodings = new List<Encoding>(){
                CodePagesEncodingProvider.Instance.GetEncoding(37) //IBM037
            };
            
        }

        public IBMiConnection(NetworkCredential creds): this(creds, DFT_SSH_PORT){}

    }
}
