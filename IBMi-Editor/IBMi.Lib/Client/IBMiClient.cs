using System;
using System.Data.Odbc;
using Renci.SshNet;
using IBMi.Lib.Config;

namespace IBMi.Lib.Client{

    public class IBMiClient: IDisposable{

        public IBMiConnection Connection {get;}
        public SshClient SshClient {get;}
        public SftpClient SftpClient {get;}
        public OdbcConnection Db2Conn {get;}


        public IBMiClient(IBMiConnection conn){
            this.Connection = conn;

            this.SshClient = new SshClient(conn.Host, conn.Port, conn.User, conn.Credentials.Password);
            this.SshClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(conn.Timeout);

            this.SftpClient = new SftpClient(conn.Host, conn.Port, conn.User, conn.Credentials.Password);
            this.SftpClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(conn.Timeout);

            this.Db2Conn = new OdbcConnection("Driver={IBM i Access ODBC Driver};" + String.Format(
                "System={0};Uid={1};Pwd={2}", conn.Host, conn.User, conn.Credentials.Password));
            this.Db2Conn.ConnectionTimeout = (int) conn.Timeout;
        }

        public void Connect(){
            ConnectWithRetry(this.SshClient);
            ConnectWithRetry(this.SftpClient);
            this.Db2Conn.Open();
        }

        // attempt to connect x times to fix known issue with SSH library
        private void ConnectWithRetry(BaseClient client){
            for(int i = 0; i < this.Connection.ConnectAttempts && !client.IsConnected; i++){
                try{
                    client.Connect();
                } catch(Renci.SshNet.Common.SshConnectionException){
                    // Server response does not contain SSH protocol identification.
                    // https://stackoverflow.com/questions/54523798/randomly-getting-renci-sshnet-sftpclient-connect-throwing-sshconnectionexception
                }
            }
            if(!client.IsConnected){
                throw new IBMiClientException(String.Format(
                    "Failed to connect after {0} attempt(s)", this.Connection.ConnectAttempts));
            }
        }

        public void Disconnect(){
            this.SshClient.Disconnect();
            this.SftpClient.Disconnect();
            this.Db2Conn.Close();
        }

        public void Dispose(){
            this.SshClient.Dispose();
            this.SftpClient.Dispose();
        }

        ~IBMiClient(){
            this.Dispose();
        }
    }
}
