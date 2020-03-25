using System.Net;

namespace IBMi.Lib.SSH.Config
{
    public class SshConfig
    {
        public NetworkCredential Credentials{get; set;}
        public int Port{get; set;}
    }
}