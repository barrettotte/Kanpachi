using System;
using System.IO;
using System.Net;
using System.Security;
using IBMi.Lib.Client;
using System.Text;

using IBMi.Lib.Config;

namespace IBMi.Lib{

    public class ClientTest{

        public void Test(){
            string rpgle = "/home/OTTEB/hello-IBMi/src/hellogit.rpgle";
            var conn = new IBMiConnection(GetCredentials("PUB400.COM", "OTTEB"), 2222);
            
            using(var ibmi = new IBMiClient(conn)){
                Console.WriteLine("Connecting to {0} as {1}...", conn.Host, conn.User);
                ibmi.Connect();
                
                // Test SFTP
                using(Stream fs = File.Create(Path.GetFullPath(@"./hello.rpgle"))){
                    Console.WriteLine("Downloading '{0}'...", rpgle);
                    ibmi.SftpClient.DownloadFile(rpgle, fs);
                }

                // Test SSH
                var cmd = ibmi.SshClient.RunCommand("system 'DSPLIBL'");
                Console.WriteLine(cmd.Result);


                // Test DB2
                // TODO: make a SQL query runner?
                var sqlCmd = ibmi.Db2Conn.CreateCommand();
                sqlCmd.CommandText = @"
                    SELECT TABLE_SCHEMA, TABLE_NAME, TABLE_PARTITION, SOURCE_TYPE
                    FROM QSYS2.SYSPARTITIONSTAT WHERE TABLE_SCHEMA = 'OTTEB1'
                    ORDER BY TABLE_PARTITION";
                var dbReader = sqlCmd.ExecuteReader();

                int colCount = dbReader.FieldCount;
                for(int i = 0; i < colCount; i++){
                    String col = dbReader.GetName(i);
                    Console.Write(col + ":");
                }
                Console.WriteLine();

                while(dbReader.Read()){
                    for(int i = 0; i < colCount; i++){
                        object obj = dbReader.GetValue(i);
                        String col = (obj == null ? "NULL" : obj.ToString());
                        Console.Write(col + ",");
                    }
                    Console.WriteLine();
                }
                dbReader.Close();
                sqlCmd.Dispose();


                ibmi.Disconnect();
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
