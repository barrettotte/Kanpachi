using System;
using System.Net;
using System.Security;

namespace Kanpachi.Lib{

    public class ClientTest{

        public void Test(){
            //var conn = new IbmiConnection(GetCredentials("PUB400.COM", "OTTEB"), 2222);
            // var conn = new IbmiConnection(GetCredentials("DEV400", "OTTEB"), 22);
            
        //     using(var client = new KanpachiClient(conn)){
        //         Console.WriteLine($"Connecting to {conn.Host} as {conn.User}...");
        //         client.Connect();

        //         // Test SFTP - working
        //         // client.DownloadMember("/QSYS.LIB/BOLIB.LIB/QRPGSRC.FILE/HELLORPG.MBR", Path.GetFullPath("./HELLO.rpg"));
        //         // client.DownloadMember("/home/OTTEB/Hello-IBMi/QRPGLESRC/hellogit.rpgle", Path.GetFullPath("./hello.rpgle"));
        //         // client.DownloadMember("/QSYS.LIB/BOLIB.LIB/QRPGLESRC.FILE/TESTTWILIO.MBR", Path.GetFullPath("./TESTTWILIO.sqlrpgle"));

        //         // Test SSH - working
        //         CmdResponse resp = client.RunCL("DSPLIBL");
        //         Console.WriteLine(resp);

        //         // client.GetLibraries();


        //         // Test DB2
        //         // TODO: make a SQL query runner?
        //         // var sqlCmd = client.Db2Conn.CreateCommand();
        //         // sqlCmd.CommandText = @"
        //         //     SELECT TABLE_SCHEMA, TABLE_NAME, TABLE_PARTITION, SOURCE_TYPE
        //         //     FROM QSYS2.SYSPARTITIONSTAT WHERE TABLE_SCHEMA = 'OTTEB1'
        //         //     ORDER BY TABLE_PARTITION";
        //         // var dbReader = sqlCmd.ExecuteReader();

        //         // int colCount = dbReader.FieldCount;
        //         // for(int i = 0; i < colCount; i++){
        //         //     String col = dbReader.GetName(i);
        //         //     Console.Write(col + ":");
        //         // }
        //         // Console.WriteLine();
        //         // while(dbReader.Read()){
        //         //     for(int i = 0; i < colCount; i++){
        //         //         object obj = dbReader.GetValue(i);
        //         //         String col = (obj == null ? "NULL" : obj.ToString());
        //         //         Console.Write(col + ",");
        //         //     }
        //         //     Console.WriteLine();
        //         // }
        //         // dbReader.Close();
        //         // sqlCmd.Dispose();

        //         client.Disconnect();
        //     }
        }

        
    }
}
