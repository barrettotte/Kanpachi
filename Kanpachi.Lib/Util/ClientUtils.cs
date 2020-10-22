using System;
using System.Net;
using System.Security;
using Renci.SshNet;

namespace Kanpachi.Lib{

    public static class ClientUtils{

        // attempt to connect x times to fix known issue with SSH library
        public static void ConnectWithRetry(BaseClient client, int attempts){
            if(!client.IsConnected){
                bool isAuthenticated = true;

                for(int i = 0; i < attempts && isAuthenticated && !client.IsConnected; i++){
                    try{
                        client.Connect();
                    } 
                    catch(Renci.SshNet.Common.SshAuthenticationException){
                        isAuthenticated = false; // leave early so we don't lock out account from bad credentials
                    } 
                    catch(Renci.SshNet.Common.SshConnectionException){
                        // Fix bizarre error found with Renci.SshNet => Server response does not contain SSH protocol identification.
                        // https://stackoverflow.com/questions/54523798/randomly-getting-renci-sshnet-sftpclient-connect-throwing-sshconnectionexception
                        //
                        // This occurred frequently when connecting to PUB400.COM, really not sure why.
                    }
                }
                if(!isAuthenticated){
                    throw new KanpachiAuthenticationException("Failed to authenticate with current credentials for " + 
                        $"{client.ConnectionInfo.Username}@{client.ConnectionInfo.Host}.");
                }
                if(!client.IsConnected){
                    throw new KanpachiConnectionException($"Failed to connect to {client.ConnectionInfo.Host} after {attempts} attempt(s).");
                }
            }
        }

        // Get host, user, and password interactively
        public static NetworkCredential GetCredentials(string inHost="", string inUser=""){
            string user = string.Empty;
            string host = string.Empty;

            if(string.IsNullOrEmpty(inHost.Trim())){
                Console.Write($"Enter host: ");
                inHost = Console.ReadLine();

                if(string.IsNullOrEmpty(inHost.Trim())){
                    throw new KanpachiProfileException("Host cannot be blank.");
                }
            }
            host = inHost;

            if(string.IsNullOrEmpty(inUser.Trim())){
                Console.Write($"Enter user for {host}: ");
                inUser = Console.ReadLine();

                if(string.IsNullOrEmpty(inUser.Trim())){
                    throw new KanpachiProfileException("User cannot be blank.");
                }
            }
            user = inUser;
            SecureString pwd = ClientUtils.PromptPassword($"Enter password for {user}@{host}: ");
            return new NetworkCredential(user, pwd, host);
        }

        // Prompt for password interactively
        public static SecureString PromptPassword(string prompt){
            SecureString pwd = new SecureString();
            Console.Write(prompt);

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
            return pwd;
        }
    }
}