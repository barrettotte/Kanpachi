using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text;
using System.Threading;
using Renci.SshNet;

namespace Kanpachi.Lib{

    public class KanpachiClient: IDisposable{

        public IbmiConnection Connection {get;}
        public SshClient SshClient {get;}
        public SftpClient SftpClient {get;}
        public OdbcConnection Db2Conn {get;}


        public KanpachiClient(){
            //
        }

        public KanpachiClient(KanpachiProfile profile){
            // this.Connection = conn;

            // this.SshClient = new SshClient(conn.Host, conn.Port, conn.User, conn.Credentials.Password);
            // this.SshClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(conn.Timeout);

            // this.SftpClient = new SftpClient(conn.Host, conn.Port, conn.User, conn.Credentials.Password);
            // this.SftpClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(conn.Timeout);

            // this.Db2Conn = new OdbcConnection("Driver={IBM i Access ODBC Driver};" +
            //     $"System={conn.Host};Uid={conn.User};Pwd={conn.Credentials.Password}");
            // this.Db2Conn.ConnectionTimeout = (int) conn.Timeout;
        }

        // Run a CL command
        public CmdResponse RunCL(string cl){
            // TODO: probably have to handle some string escaping here ?
            return RunCmd($"system \"{cl}\"");
        }

        // Run a command
        // NOTE: Might need to look into setting shell with client??? ```chsh -s /bin/ksh```
        //       Not every user has the "correct" shell setup out of the gate...
        public CmdResponse RunCmd(string cmdString){
            SshCommand cmd = this.SshClient.CreateCommand(cmdString);
            IAsyncResult task = cmd.BeginExecute();

            while(!task.IsCompleted){
                Thread.Sleep(500);
            }
            // TODO: add max timeout on command

            CmdResponse response = new CmdResponse(cmd.ExitStatus, cmd.Error, cmd.Result);
            cmd.EndExecute(task); 

            return response;
        }

        // open all connections needed
        public void Connect(){
            ConnectWithRetry(this.SshClient);
            ConnectWithRetry(this.SftpClient);
            this.Db2Conn.Open();

            // Make sure cache directory is created
            CmdResponse resp = RunCmd($"mkdir -p \"{this.Connection.IfsCache}\"");
            // TODO: check status, throw exception if != 0

            // TODO: add list of libraries to IBMi model
        }

        // attempt to connect x times to fix known issue with SSH library
        private void ConnectWithRetry(BaseClient client){
            for(int i = 0; i < this.Connection.ConnectAttempts && !client.IsConnected; i++){
                try{
                    client.Connect();
                } catch(Renci.SshNet.Common.SshConnectionException){
                    // Fix bizarre error found with Renci.SshNet => Server response does not contain SSH protocol identification.
                    // https://stackoverflow.com/questions/54523798/randomly-getting-renci-sshnet-sftpclient-connect-throwing-sshconnectionexception
                }
            }
            if(!client.IsConnected){
                throw new KanpachiException($"Failed to connect after {this.Connection.ConnectAttempts} attempt(s)");
            }
        }

        // handle downloading a source member from IFS or QSYS.LIB
        public void DownloadMember(string target, string destination){
            Console.WriteLine($"Downloading '{target}' to '{destination}'");
            Encoding encoding = Encoding.ASCII;
            string srcPath = target;

            if(target.StartsWith("/QSYS.LIB")){
                // Move source member from QSYS.LIB to IFS. This fixes encoding issues presented with SFTP.
                // It does not appear to be possible to preserve line endings with IBMi via SFTP...
                
                // However, maybe I can look up SRCPF row length and partition the data that way ?

                encoding = CodePagesEncodingProvider.Instance.GetEncoding(37); // IBM037;
                srcPath = $"{this.Connection.IfsCache}/DOWN.MBR";
                Console.WriteLine($"Copying member '{target}' to '{srcPath}'");

                CmdResponse resp = RunCL($"CPYTOSTMF FROMMBR('{target}') TOSTMF('{srcPath}') STMFOPT(*REPLACE)");
                // TODO: check status, throw exception if != 0
            }

            byte[] src = Encoding.Convert(encoding, Encoding.ASCII, this.SftpClient.ReadAllBytes(srcPath));
            System.IO.File.WriteAllBytes(destination, src);
        }

        // Get list of all libraries in QSYS.LIB
        public List<Library> GetLibraries(){
            List<Library> libraries = new List<Library>();

            CmdResponse resp = RunCL($"DSPLIB LIB(*ALL)");
            Console.WriteLine(resp.StdOut);
            // TODO: parse libraries out of STDOUT
            // OR
            // Just go for an all SQL approach with system views???

            return libraries;
        }

        // Get source physical files in library
        public List<SrcPf> GetSrcPfs(){
            List<SrcPf> spfs = new List<SrcPf>();
            /*select SYSTEM_TABLE_NAME, TABLE_TEXT, ROW_LENGTH
              from QSYS2.SYSTABLES
              where SYSTEM_TABLE_SCHEMA='BOLIB' and FILE_TYPE='S' and TABLE_TYPE='P'
              ; */
            return spfs;
        }

        public void Disconnect(){
            this.SshClient.Disconnect();
            this.SftpClient.Disconnect();
            this.Db2Conn.Close();
        }

        public void Dispose(){
            this.Disconnect();
            this.SshClient.Dispose();
            this.SftpClient.Dispose();
        }

        ~KanpachiClient(){
            this.Dispose();
        }
    }
}
