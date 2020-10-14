using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Kanpachi.Lib{

    public class KanpachiProfile{
    
        [JsonIgnore]
        public NetworkCredential Credentials {get; set;}
        public string Name {get; set;}

        public string Host {get; set;}
        public string User {get; set;}
        public string Password {get; set;}          // (encrypted)
        public bool IsDefault {get; set;}           // set profile as default

        public int Port {get; set;}
        public double Timeout {get; set;}           // time in seconds before timeout.
        public int ConnectAttempts {get; set;}      // number of times to attempt if connection failed.
        public string DefaultEncoding {get; set;}   //


        public KanpachiProfile(){
            //
        }


        public KanpachiProfile(string name, NetworkCredential creds){
            Name = name;
            Host = creds.Domain;
            User = creds.UserName;
            Credentials = creds;
            
            Port = KanpachiDefaults.SshPort;
            ConnectAttempts = KanpachiDefaults.ConnectAttempts;
            Timeout = KanpachiDefaults.Timeout;
            DefaultEncoding = KanpachiDefaults.DefaultEncoding;
        }
    }
}
