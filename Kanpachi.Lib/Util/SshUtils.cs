using Renci.SshNet;

namespace Kanpachi.Lib{

    public static class SshUtils{

        // attempt to connect x times to fix known issue with SSH library
        public static void ConnectWithRetry(BaseClient client, int attempts){
            if(!client.IsConnected){
                bool isAuthenticated = true;

                for(int i = 0; i < attempts && isAuthenticated && !client.IsConnected; i++){
                    try{
                        client.Connect();
                    } catch(Renci.SshNet.Common.SshAuthenticationException){
                        isAuthenticated = false; // leave early so we don't lock out account from bad credentials
                    } catch(Renci.SshNet.Common.SshConnectionException){
                        // Fix bizarre error found with Renci.SshNet => Server response does not contain SSH protocol identification.
                        // https://stackoverflow.com/questions/54523798/randomly-getting-renci-sshnet-sftpclient-connect-throwing-sshconnectionexception
                        //
                        // Might have something to do with my connection speed, but I can't be certain.
                    }
                }
                if(!isAuthenticated){
                    throw new KanpachiAuthenticationException("Failed to authenticate with current credentials for " + 
                        $"{client.ConnectionInfo.Username}@{client.ConnectionInfo.Host}.");
                }
                if(!client.IsConnected){
                    throw new KanpachiException($"Failed to connect to {client.ConnectionInfo.Host} after {attempts} attempt(s).");
                }
            }
        }
        
    }
}