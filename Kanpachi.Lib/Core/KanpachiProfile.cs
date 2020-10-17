using Newtonsoft.Json;

namespace Kanpachi.Lib{

    public class KanpachiProfile{
    
        public string Name {get; set;}
        public bool IsActive {get; set;}            // should only be one active profile at a time
                                                    // NOTE: might need to move to application settings.json or something
        public string Host {get; set;}
        public string User {get; set;}
        public string Password {get; set;}          // encrypted

        [JsonIgnore]
        public string PasswordDecrypted {get; set;}

        public int Port {get; set;}
        public double Timeout {get; set;}            // time in seconds before timeout.
        public int ConnectAttempts {get; set;}       // number of times to attempt if connection failed
        public string DefaultEncoding {get; set;}    //

        public string DownloadPath {get; set;}       // client directory to download to
        public string IfsCachePath {get; set;}       // path to directory on IFS for temporary caching


        public KanpachiProfile(){
            // default constructor needed for JSON deserialization
        }


        public KanpachiProfile(string name, string host, string user){
            Name = name;
            IsActive = false;

            Host = host;
            User = user;
            
            Port = KanpachiDefaults.SshPort;
            ConnectAttempts = KanpachiDefaults.ConnectAttempts;
            Timeout = KanpachiDefaults.Timeout;
            DefaultEncoding = KanpachiDefaults.DefaultEncoding;

            IfsCachePath = $"/home/{user}/.kanpachi";
        }
    }
}
