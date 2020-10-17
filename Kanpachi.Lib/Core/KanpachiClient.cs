using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text;
using System.Threading;
using Renci.SshNet;

namespace Kanpachi.Lib{

    public class KanpachiClient: IDisposable{

        public KanpachiProfile Profile {get;}

        public IbmiConnection Connection {get;}
        public SshClient SshClient {get;}
        public SftpClient SftpClient {get;}
        public OdbcConnection Db2Conn {get;}


        public KanpachiClient(KanpachiProfile profile){
            Profile = profile;

            SshClient = new SshClient(profile.Host, profile.Port, profile.User, profile.PasswordDecrypted);
            SshClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(profile.Timeout);

            SftpClient = new SftpClient(profile.Host, profile.Port, profile.User, profile.PasswordDecrypted);
            SftpClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(profile.Timeout);

            // https://www.ibm.com/support/knowledgecenter/ssw_ibm_i_74/rzaik/connectkeywords.htm
            string connStr = "Driver={IBM i Access ODBC Driver};" +
                 $"System={profile.Host};Uid={profile.User};Pwd={profile.PasswordDecrypted};";
            
            // Db2Conn = new OdbcConnection(connStr);
            // Db2Conn.ConnectionTimeout = (int) profile.Timeout;
            // TODO: move DB2 driver to nested object in profile => allow driver and other keyword configuration
        }

        public void Connect(){
            Console.Write($"Establishing connection to {Profile.Host}...");

            SshUtils.ConnectWithRetry(SshClient, Profile.ConnectAttempts);
            Console.Write("SSH Success...");
            SshUtils.ConnectWithRetry(SftpClient, Profile.ConnectAttempts);
            Console.Write("SFTP Success...");
            // Db2Conn.Open();
            Console.WriteLine("Connected.");

            // Console.WriteLine(Profile.User);
            // Console.WriteLine(Profile.PasswordDecrypted);
            // Console.WriteLine(Profile.Port);
            CmdResponse rs = ExecCmd($"mkdir -p \"{Profile.IfsCachePath}\"");
        }

        public void Disconnect(){
            this.SshClient.Disconnect();
            this.SftpClient.Disconnect();
            this.Db2Conn.Close();
        }

        public void Dispose(){
            this.Disconnect();
        }

        // Execute a CL command
        public CmdResponse ExecCL(string cl){
            // TODO: probably have to handle some string escaping here ?
            return ExecCmd($"system \"{cl}\"");
        }

        // Run a command
        // NOTE: Might need to look into setting shell with client??? ```chsh -s /bin/ksh```
        //       Not every user has the "correct" shell setup out of the gate...
        public CmdResponse ExecCmd(string cmdString){
            Console.WriteLine(cmdString);

            SshCommand cmd = SshClient.CreateCommand(cmdString);
            IAsyncResult task = cmd.BeginExecute();

            while(!task.IsCompleted){
                Thread.Sleep(500);
            }
            // TODO: add max timeout on command

            CmdResponse response = new CmdResponse(cmd.ExitStatus, cmd.Error, cmd.Result);
            cmd.EndExecute(task); 

            return response;
        }

        // handle downloading a source member from IFS or QSYS.LIB
        // TODO: return member with member type (and record length?)
        public byte[] DownloadMember(string serverPath){
            Encoding encoding = Encoding.ASCII;
            string srcPath = serverPath;

            if(serverPath.StartsWith("/QSYS.LIB")){
                // Move source member from QSYS.LIB to IFS. This fixes encoding issues presented with SFTP.
                // It does not appear to be possible to preserve line endings with IBMi via SFTP...
                //
                // TODO: However, maybe I can look up SRCPF row length and partition the data that way ?

                encoding = CodePagesEncodingProvider.Instance.GetEncoding(37); // IBM037;
                srcPath = $"{Profile.IfsCachePath}/DOWN.MBR";
                // Console.WriteLine($"Copying member '{serverPath}' to '{srcPath}'");

                // TODO: CL command to get member description, parse for record length and member type.

                CmdResponse resp = ExecCL($"CPYTOSTMF FROMMBR('{serverPath}') TOSTMF('{srcPath}') STMFOPT(*REPLACE)");
                // TODO: check status, throw exception if != 0
            }
            return Encoding.Convert(encoding, Encoding.ASCII, this.SftpClient.ReadAllBytes(srcPath));
        }
    }
}
