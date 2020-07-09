using System;
using System.Text;
using System.IO;
using System.Data.Odbc;
using System.Threading;
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

        // Run a command (blocking)
        private int RunCmd(string cmdString){
            var cmd = this.SshClient.CreateCommand(cmdString);
            var task = cmd.BeginExecute();
            while(!task.IsCompleted){
                Thread.Sleep(1000);
            }
            var result = cmd.EndExecute(task);
            // TODO: add max timeout on command
            return 0;
        }

        // 
        public void Connect(){
            ConnectWithRetry(this.SshClient);
            ConnectWithRetry(this.SftpClient);
            this.Db2Conn.Open();

            // Make sure cache directory is created
            int status = RunCmd(string.Format("mkdir -p \"{0}\"", this.Connection.IfsCache));
            // TODO: check status
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

        // Handle downloading a file from IFS or QSYS.LIB
        public void Download(string target, string destination){
            Console.WriteLine(String.Format("Downloading '{0}' to '{1}'", target, destination));

            //using(var fs = new FileStream(destination, FileMode.OpenOrCreate)){
                Encoding encoding = Encoding.ASCII;
                string fp = target;

                if(target.StartsWith("/QSYS.LIB")){
                    encoding = CodePagesEncodingProvider.Instance.GetEncoding(37); // IBM037;
                    fp = this.Connection.IfsCache + "/" + "DOWN.MBR";

                    Console.WriteLine(String.Format("Copying member '{0}' to '{1}'", target, fp));

                    int status = RunCmd(String.Format(
                        "system \"CPYTOSTMF FROMMBR('{0}') TOSTMF('{1}') STMFOPT(*REPLACE)\"", target, fp));
                    // TODO: check status

                }
                //this.SftpClient.DownloadFile(fp, fs);
                var src = Encoding.Convert(encoding, Encoding.ASCII, this.SftpClient.ReadAllBytes(fp));
                System.IO.File.WriteAllBytes(destination, src);
            //}
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
