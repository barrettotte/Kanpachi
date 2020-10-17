
namespace Kanpachi.Lib{

    public class KanpachiProfile{
    
        public string Name {get; set;}

        public string Host {get; set;}
        public string User {get; set;}
        public string Password {get; set;}          // (encrypted)
        public bool IsDefault {get; set;}           // set profile as default

        public int Port {get; set;}
        public double Timeout {get; set;}           // time in seconds before timeout.
        public int ConnectAttempts {get; set;}      // number of times to attempt if connection failed
        public string DefaultEncoding {get; set;}   //


        public KanpachiProfile(){
            //
        }


        public KanpachiProfile(string name, string host, string user){
            Name = name;
            Host = host;
            User = user;
            
            Port = KanpachiDefaults.SshPort;
            ConnectAttempts = KanpachiDefaults.ConnectAttempts;
            Timeout = KanpachiDefaults.Timeout;
            DefaultEncoding = KanpachiDefaults.DefaultEncoding;
        }
    }
}
