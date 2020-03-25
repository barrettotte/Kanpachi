using System;
using System.Net;
using System.Security;
using Renci.SshNet;

namespace IBMi.Lib
{
    public class SshTest
    {
        public void Test(){
            string host = "PUB400.COM";
            string user = "OTTEB";
            int port = 2222;
            NetworkCredential creds = GetCredentials(host, user);

            using(SshClient client = new SshClient(creds.Domain, port, creds.UserName, creds.Password)){
                client.Connect();
                Console.WriteLine(client.ConnectionInfo.ServerVersion);
                
            }
        }

        // Get user and password
        private NetworkCredential GetCredentials(string host, string inUser=""){
            string user = string.Empty;
            SecureString pwd = new SecureString();

            if(string.IsNullOrEmpty(inUser)){
                Console.Write(string.Format("Enter user for {0}: ", host));
                user = Console.ReadLine();
            } else{
                user = inUser;
            }
            Console.Write(String.Format("Enter password for {0}@{1}: ", user, host));

            while(true){
                ConsoleKeyInfo i = Console.ReadKey(true);
                if(i.Key == ConsoleKey.Enter){
                    break;
                } else if(i.Key == ConsoleKey.Backspace && pwd.Length > 0){
                    pwd.RemoveAt(pwd.Length - 1);
                    Console.Write("\b \b");
                } else if(i.KeyChar != '\u0000'){
                    pwd.AppendChar(i.KeyChar);
                    Console.Write("*");
                }
            }
            Console.Write("\n");
            return new NetworkCredential(user, pwd, host);
        }
    }
}
