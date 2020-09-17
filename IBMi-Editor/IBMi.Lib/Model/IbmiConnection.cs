using System.Collections.Generic;
using System.Net;
using System.Text;

namespace IBMi.Lib.Client{

    public class IbmiConnection{

        public const int DFT_SSH_PORT = 22;
        public const int DFT_CONNECT_ATTEMPTS = 5;
        public const double DFT_TIMEOUT = 10.0;

        public NetworkCredential Credentials {get;}
        public string Host {get;} 
        public string User {get;}                // TODO: validate username size <= 10 + regex
        public int Port {get; set;}              // TODO: validate port range [0-65536]
        public double Timeout {get; set;}
        public int ConnectAttempts {get; set;}

        public string Library {get; set;}        // TODO: validate library size <= 10 + regex
        public string IfsPath {get; set;}        // TODO: validate path with regex
        public string IfsCache {get; set;}       // temporarily cache things in IFS

        public Encoding DefaultEncoding {get; set;}
        public List<Encoding> FallbackEncodings {get; set;}


        public IbmiConnection(NetworkCredential creds, int port){
            this.Credentials = creds;
            this.User = creds.UserName;
            this.Host = creds.Domain;
            this.Port = port;  
            this.Library = "QTEMP";
            this.IfsPath = "/home/" + this.User + "/";
            this.IfsCache = this.IfsPath + "cli";
            this.DefaultEncoding = Encoding.UTF8;
            this.FallbackEncodings = new List<Encoding>(){
                CodePagesEncodingProvider.Instance.GetEncoding(37) //IBM037
            };
            this.Timeout = DFT_TIMEOUT;
            this.ConnectAttempts = DFT_CONNECT_ATTEMPTS;
        }
    }
}
