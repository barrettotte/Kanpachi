using System;
using System.Net;
using System.Security;
using Kanpachi.Lib.Client;

namespace Kanpachi.Lib{

    public class ClientTest{

        public void Test(){
            //var conn = new IbmiConnection(GetCredentials("PUB400.COM", "OTTEB"), 2222);
            var conn = new IbmiConnection(GetCredentials("DEV400", "OTTEB"), 22);
            
            using(var client = new IbmiClient(conn)){
                Console.WriteLine($"Connecting to {conn.Host} as {conn.User}...");
                client.Connect();

                // Test SFTP - working
                // client.DownloadMember("/QSYS.LIB/BOLIB.LIB/QRPGSRC.FILE/HELLORPG.MBR", Path.GetFullPath("./HELLO.rpg"));
                // client.DownloadMember("/home/OTTEB/Hello-IBMi/QRPGLESRC/hellogit.rpgle", Path.GetFullPath("./hello.rpgle"));
                // client.DownloadMember("/QSYS.LIB/BOLIB.LIB/QRPGLESRC.FILE/TESTTWILIO.MBR", Path.GetFullPath("./TESTTWILIO.sqlrpgle"));

                // Test SSH - working
                CmdResponse resp = client.RunCL("DSPLIBL");
                Console.WriteLine(resp);

                // client.GetLibraries();


                // Test DB2
                // TODO: make a SQL query runner?
                // var sqlCmd = client.Db2Conn.CreateCommand();
                // sqlCmd.CommandText = @"
                //     SELECT TABLE_SCHEMA, TABLE_NAME, TABLE_PARTITION, SOURCE_TYPE
                //     FROM QSYS2.SYSPARTITIONSTAT WHERE TABLE_SCHEMA = 'OTTEB1'
                //     ORDER BY TABLE_PARTITION";
                // var dbReader = sqlCmd.ExecuteReader();

                // int colCount = dbReader.FieldCount;
                // for(int i = 0; i < colCount; i++){
                //     String col = dbReader.GetName(i);
                //     Console.Write(col + ":");
                // }
                // Console.WriteLine();
                // while(dbReader.Read()){
                //     for(int i = 0; i < colCount; i++){
                //         object obj = dbReader.GetValue(i);
                //         String col = (obj == null ? "NULL" : obj.ToString());
                //         Console.Write(col + ",");
                //     }
                //     Console.WriteLine();
                // }
                // dbReader.Close();
                // sqlCmd.Dispose();

                client.Disconnect();
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
