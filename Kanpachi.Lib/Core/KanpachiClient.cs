using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text;
using System.Threading;
using Renci.SshNet;

namespace Kanpachi.Lib{

    public class KanpachiClient: IDisposable{

        public KanpachiProfile Profile {get;}
        public SshClient SshClient {get;}
        public SftpClient SftpClient {get;}
        public OdbcConnection Db2Client {get;}

        // TODO: swap console logging for actual logger via dep inj

        public KanpachiClient(KanpachiProfile profile){
            Profile = profile;

            // TODO: only connect to clients when needed => Make some kind of guard method to connect if not connected?
            //       could speed up initial startup significantly...

            // TODO: might be able to do most utility through SQL. So, SSH might be moved to separate client
            SshClient = new SshClient(profile.Host, profile.Port, profile.User, profile.PasswordDecrypted);
            SshClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(profile.Timeout);

            SftpClient = new SftpClient(profile.Host, profile.Port, profile.User, profile.PasswordDecrypted);
            SftpClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(profile.Timeout);

            // https://www.ibm.com/support/knowledgecenter/ssw_ibm_i_74/rzaik/connectkeywords.htm
            string connStr = "Driver={IBM i Access ODBC Driver};" +
                $"System={profile.Host};Uid={profile.User};Pwd={profile.PasswordDecrypted};";

            Db2Client = new OdbcConnection(connStr);
            Db2Client.ConnectionTimeout = (int) profile.Timeout;
            // TODO: move DB2 driver to nested object in profile => allow driver and other keyword configurations?
        }

        public void Connect(){
            Console.Write($"Establishing connection to {Profile.Host}...");
            
            ClientUtils.ConnectWithRetry(SshClient, Profile.ConnectAttempts);
            Console.Write("SSH...");

            ClientUtils.ConnectWithRetry(SftpClient, Profile.ConnectAttempts);
            Console.Write("SFTP...");

            Db2Client.Open();
            Console.Write("DB2...");
            Console.WriteLine("Connected.");

            if(!SftpClient.Exists(Profile.IfsCachePath)){
                Console.WriteLine($"Creating IFS cache at {Profile.IfsCachePath}.");
                // TODO: swap out for SQL QCMDEXC?
                ExecCmd($"mkdir -p \"{Profile.IfsCachePath}\""); // ensure temporary cache created
            }
        }

        public void Disconnect(){
            SshClient?.Disconnect();
            SftpClient?.Disconnect();
            Db2Client?.Close();
        }

        public void Dispose(){
            Disconnect();
        }

        // Execute a CL command
        public CmdResponse ExecCL(string cl){
            // TODO: probably have to handle some string escaping here ?
            return ExecCmd($"system \"{cl}\"");
        }

        // Execute a command
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
            // TODO: check if command success, throw CommandException
            CmdResponse response = new CmdResponse(cmd.ExitStatus, cmd.Error, cmd.Result);
            cmd.EndExecute(task);

            return response;
        }

        // execute a series of commands
        public List<CmdResponse> ExecCmds(List<string> cmdStrings){
            var responses = new List<CmdResponse>();
            foreach(var cmd in cmdStrings){
                responses.Add(ExecCmd(cmd));
            }
            return responses;
        }

        // execute DB2 SQL command  TODO: not finished
        public void ExecSQL(string sqlString){
            using(var sqlCmd = Db2Client.CreateCommand()){
                sqlCmd.CommandText = sqlString;
                Console.WriteLine(sqlCmd.CommandText + '\n');

                using(var dbReader = sqlCmd.ExecuteReader()){
                    int colCount = dbReader.FieldCount;
                    for(int i = 0; i < colCount; i++){
                        Console.Write($"{dbReader.GetName(i)}:");
                    }
                    Console.WriteLine();

                    while(dbReader.Read()){
                        for(int i = 0; i < colCount; i++){
                            object obj = dbReader.GetValue(i);
                            String col = (obj == null ? "NULL" : obj.ToString());
                            Console.Write($"{col},");
                        }
                        Console.WriteLine();
                    }
                }
            }
        }

        // download a source member from QSYS
        public byte[] DownloadMember(string srcPath){
            Encoding encoding = CodePagesEncodingProvider.Instance.GetEncoding(37); // IBM037;
            return Encoding.Convert(encoding, Encoding.ASCII, SftpClient.ReadAllBytes(srcPath));
        }

        // Get details of library
        public Library GetLibraryDetails(string lib){
            Library library = null;
            var sqlString = $@"
                select SYSTEM_SCHEMA_NAME, coalesce(SCHEMA_TEXT,'')
                from QSYS2.SYSSCHEMAS
                where SYSTEM_SCHEMA_NAME = '{lib}'
            ";
            Console.WriteLine($"Gathering details on library {lib}...\n{sqlString}\n");

            using(var sqlCmd = Db2Client.CreateCommand()){
                sqlCmd.CommandText = sqlString;
                using(var dbReader = sqlCmd.ExecuteReader()){
                    while(dbReader.Read()){
                        library = MapRowToLib(dbReader);
                    }
                }
            }
            return library;
        }

        // Get details of source physical file
        public SrcPf GetSrcPfDetails(string lib, string spf){
            SrcPf srcPf = null;
            var sqlString = $@"
                select SYSTEM_TABLE_NAME, coalesce(TABLE_TEXT,'')
                from QSYS2.SYSTABLES
                where SYSTEM_TABLE_SCHEMA='{lib}'
                  and FILE_TYPE='S' and TABLE_TYPE='P'
                  and SYSTEM_TABLE_NAME='{spf}'
                limit 1
            ";
            Console.WriteLine($"Gathering details on source physical file {lib}/{spf}...\n{sqlString}\n");

            using(var sqlCmd = Db2Client.CreateCommand()){
                sqlCmd.CommandText = sqlString;
                using(var dbReader = sqlCmd.ExecuteReader()){
                    while(dbReader.Read()){
                        srcPf = MapRowToSrcPf(dbReader);
                    }
                }
            }
            return srcPf;
        }

        // Get details of source member => type, size, record length, etc.
        public SrcMbr GetSrcMbrDetails(string lib, string spf, string mbr){
            SrcMbr member = null;
            var sqlString = $@"
                select TABLE_PARTITION, SOURCE_TYPE, NUMBER_ROWS, coalesce(PARTITION_TEXT,''),
                  AVGROWSIZE, DATA_SIZE, CREATE_TIMESTAMP, LAST_CHANGE_TIMESTAMP
                from QSYS2.SYSPARTITIONSTAT
                where SYSTEM_TABLE_SCHEMA='{lib}'
                  and SYSTEM_TABLE_NAME='{spf}'
                  and TABLE_PARTITION='{mbr}'
                limit 1
            ";
            Console.WriteLine($"Gathering details on source member {lib}/{spf}/{mbr}...\n{sqlString}\n");

            using(var sqlCmd = Db2Client.CreateCommand()){
                sqlCmd.CommandText = sqlString;
                using(var dbReader = sqlCmd.ExecuteReader()){
                    while(dbReader.Read()){
                        member = MapRowToSrcMbr(dbReader);
                    }
                }
            }
            return member;
        }

        public List<SrcPf> GetSpfList(string lib){
            var spfs = new List<SrcPf>();
            var sqlString = $@"
                select SYSTEM_TABLE_NAME, coalesce(TABLE_TEXT,'')
                from QSYS2.SYSTABLES
                where SYSTEM_TABLE_SCHEMA='{lib}'
                  and FILE_TYPE='S' and TABLE_TYPE='P'
            ";
            Console.WriteLine($"Gathering source physical file list of library {lib}...\n{sqlString}\n");

            using(var sqlCmd = Db2Client.CreateCommand()){
                sqlCmd.CommandText = sqlString;
                using(var dbReader = sqlCmd.ExecuteReader()){
                    while(dbReader.Read()){
                        spfs.Add(MapRowToSrcPf(dbReader));
                    }
                }
            }
            return spfs;
        }

        // Get member description
        public List<SrcMbr> GetSrcMbrList(string lib, string spf){
            var members = new List<SrcMbr>();
            var sqlString = $@"
                select TABLE_PARTITION, coalesce(SOURCE_TYPE,''), NUMBER_ROWS, 
                  coalesce(PARTITION_TEXT,''), AVGROWSIZE
                from QSYS2.SYSPARTITIONSTAT
                where SYSTEM_TABLE_SCHEMA='{lib}'
                  and SYSTEM_TABLE_NAME='{spf}'
                order by 1
            ";
            Console.WriteLine($"Gathering member list of source physical file {lib}/{spf}...\n{sqlString}\n");

            using(var sqlCmd = Db2Client.CreateCommand()){
                sqlCmd.CommandText = sqlString;
                using(var dbReader = sqlCmd.ExecuteReader()){
                    while(dbReader.Read()){
                        members.Add(MapRowToSrcMbr(dbReader));
                    }
                }
            }
            return members;
        }

        // Map ODBC row to source member
        private SrcMbr MapRowToSrcMbr(OdbcDataReader reader){
            var mbr = new SrcMbr();
            mbr.Name = reader.GetString(0).Trim();       // TABLE_PARTITION
            mbr.Attribute = reader.GetString(1).Trim();  // SOURCE_TYPE
            mbr.LineCount = reader.GetInt32(2);          // NUMBER_ROWS
            mbr.Text = reader.GetString(3).Trim();       // PARTITION_TEXT
            mbr.RecordLength = reader.GetInt32(4) - 12;  // AVGROWSIZE  => Ex: RPG  92-12 = 80 columns
            return mbr;
        }

        // Map ODBC row to source physical file
        private SrcPf MapRowToSrcPf(OdbcDataReader reader){
            var spf = new SrcPf();
            spf.Name = reader.GetString(0).Trim();  // SYSTEM_TABLE_NAME
            spf.Text = reader.GetString(1).Trim();  // TABLE_TEXT
            return spf;
        }

        // Map ODBC row to library
        private Library MapRowToLib(OdbcDataReader reader){
            var lib = new Library();
            lib.Name = reader.GetString(0).Trim();  // SYSTEM_SCHEMA_NAME
            lib.Text = reader.GetString(1).Trim();  // SCHEMA_TEXT
            return lib;
        }
    }
}
