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

            this.SshClient = new SshClient(conn.Host, conn.Port, 
                conn.Credentials.UserName, conn.Credentials.Password);
            this.SftpClient = new SftpClient(conn.Host, conn.Port, 
                conn.Credentials.UserName, conn.Credentials.Password);
            this.Db2Conn = new OdbcConnection("Driver={IBM i Access ODBC Driver};" + String.Format(
                "System={0};Uid={1};Pwd={2}", conn.Host, conn.User, conn.Credentials.Password
            ));
        }

        public IBMiClient(IBMiConnection conn, SshClient ssh, SftpClient sftp, OdbcConnection db2){
            this.Connection = conn;
            this.SshClient = ssh;
            this.SftpClient = sftp;
            this.Db2Conn = db2;
        }

        public void Connect(){
            this.SshClient.Connect();
            this.SftpClient.Connect();
            this.Db2Conn.Open();
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
