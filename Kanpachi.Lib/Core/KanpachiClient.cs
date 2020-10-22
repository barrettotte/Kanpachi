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


        public KanpachiClient(KanpachiProfile profile){
            Profile = profile;

            SshClient = new SshClient(profile.Host, profile.Port, profile.User, profile.PasswordDecrypted);
            SshClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(profile.Timeout);

            SftpClient = new SftpClient(profile.Host, profile.Port, profile.User, profile.PasswordDecrypted);
            SftpClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(profile.Timeout);

            string connStr = "Driver={IBM i Access ODBC Driver};" +
                $"System={profile.Host};Uid={profile.User};Pwd={profile.PasswordDecrypted};";
            Db2Client = new OdbcConnection(connStr);
            Db2Client.ConnectionTimeout = (int) Math.Round(profile.Timeout);

            Connect();
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
                ExecCmd($"mkdir -p \"{Profile.IfsCachePath}\""); // swap out for SQL QCMDEXC?
            }
        }

        // Execute a CL command
        public CmdResponse ExecCL(string clString){
            return ExecCmd($"system \"{clString}\"");
        }

        // Execute a command
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

        // execute DB2 SQL command  TODO: not finished
        public void ExecSQL(string sqlString){
            using(var sqlCmd = Db2Client.CreateCommand()){
                sqlCmd.CommandText = sqlString;
                Console.WriteLine(sqlCmd.CommandText + '\n');

                using(var dbReader = sqlCmd.ExecuteReader()){
                    SqlUtils.DisplayRows(dbReader);
                }
            }
        }

        // download a source member from QSYS
        public byte[] DownloadMember(string srcPath){
            Encoding encoding = CodePagesEncodingProvider.Instance.GetEncoding(37); // IBM037;
            try{
                byte[] src = SftpClient.ReadAllBytes(srcPath);
                return Encoding.Convert(encoding, Encoding.ASCII, src);
            } catch(Exception e){
                throw new KanpachiSftpException($"Error occurred downloading source member at {srcPath}\n\t{e.Message}");
            }
        }

        // Get details of library
        public Library GetLibraryDetails(string lib){
            var sqlString = $@"
                select SYSTEM_SCHEMA_NAME, coalesce(SCHEMA_TEXT,'')
                from QSYS2.SYSSCHEMAS
                where SYSTEM_SCHEMA_NAME = '{lib}'
                limit 1
            ";
            using(var sqlCmd = Db2Client.CreateCommand()){
                Console.WriteLine($"Gathering details on library {lib}...\n{sqlString}\n");
                sqlCmd.CommandText = sqlString;
                using(var dbReader = sqlCmd.ExecuteReader()){
                    while(dbReader.Read()){
                        return SqlUtils.RowToLib(dbReader);
                    }
                }
            }
            throw new KanpachiQsysException($"Could not find library {lib}.");
        }

        // Get details of source physical file
        public SrcPf GetSrcPfDetails(string lib, string spf){
            var sqlString = $@"
                select SYSTEM_TABLE_NAME, coalesce(TABLE_TEXT,'')
                from QSYS2.SYSTABLES
                where SYSTEM_TABLE_SCHEMA='{lib}' and FILE_TYPE='S' and TABLE_TYPE='P' and SYSTEM_TABLE_NAME='{spf}'
                limit 1
            ";
            using(var sqlCmd = Db2Client.CreateCommand()){
                Console.WriteLine($"Gathering details on source physical file {lib}/{spf}...\n{sqlString}\n");
                sqlCmd.CommandText = sqlString;
                using(var dbReader = sqlCmd.ExecuteReader()){
                    while(dbReader.Read()){
                        return SqlUtils.RowToSrcPf(dbReader);
                    }
                }
            }
            throw new KanpachiQsysException($"Could not find source physical file at {lib}/{spf}.");
        }

        // Get details of source member => type, size, record length, etc.
        public SrcMbr GetSrcMbrDetails(string lib, string spf, string mbr){
            var sqlString = $@"
                select TABLE_PARTITION, SOURCE_TYPE, NUMBER_ROWS, coalesce(PARTITION_TEXT,''),
                  AVGROWSIZE, DATA_SIZE, CREATE_TIMESTAMP, LAST_CHANGE_TIMESTAMP
                from QSYS2.SYSPARTITIONSTAT
                where SYSTEM_TABLE_SCHEMA='{lib}' and SYSTEM_TABLE_NAME='{spf}' and TABLE_PARTITION='{mbr}'
                limit 1
            ";
            using(var sqlCmd = Db2Client.CreateCommand()){
                Console.WriteLine($"Gathering details on source member {lib}/{spf}/{mbr}...\n{sqlString}\n");
                sqlCmd.CommandText = sqlString;
                using(var dbReader = sqlCmd.ExecuteReader()){
                    while(dbReader.Read()){
                        return SqlUtils.RowToSrcMbr(dbReader);
                    }
                }
            }
            throw new KanpachiQsysException($"Could not find source member at {lib}/{spf}/{mbr}.");
        }

        // Get list of libraries on host
        public List<Library> GetLibraries(){
            var libs = new List<Library>();
            var sqlString = $@"
                select SYSTEM_SCHEMA_NAME, coalesce(SCHEMA_TEXT,'')
                from QSYS2.SYSSCHEMAS
            ";
            using(var sqlCmd = Db2Client.CreateCommand()){
                Console.WriteLine($"Gathering list of libraries on {Profile.Host}...\n{sqlString}\n");
                sqlCmd.CommandText = sqlString;
                using(var dbReader = sqlCmd.ExecuteReader()){
                    while(dbReader.Read()){
                        libs.Add(SqlUtils.RowToLib(dbReader));
                    }
                }
            }
            return libs;
        }

        // Get list of source physical files in LIB
        public List<SrcPf> GetSrcPfList(string lib){
            var spfs = new List<SrcPf>();
            var sqlString = $@"
                select SYSTEM_TABLE_NAME, coalesce(TABLE_TEXT,'')
                from QSYS2.SYSTABLES
                where SYSTEM_TABLE_SCHEMA='{lib}' and FILE_TYPE='S' and TABLE_TYPE='P'
            ";
            using(var sqlCmd = Db2Client.CreateCommand()){
                Console.WriteLine($"Gathering source physical file list of library {lib}...\n{sqlString}\n");
                sqlCmd.CommandText = sqlString;
                using(var dbReader = sqlCmd.ExecuteReader()){
                    while(dbReader.Read()){
                        spfs.Add(SqlUtils.RowToSrcPf(dbReader));
                    }
                }
            }
            return spfs;
        }

        // Get list of source members at LIB/SPF
        public List<SrcMbr> GetSrcMbrList(string lib, string spf){
            var members = new List<SrcMbr>();
            var sqlString = $@"
                select TABLE_PARTITION, coalesce(SOURCE_TYPE,''), NUMBER_ROWS, coalesce(PARTITION_TEXT,''), AVGROWSIZE
                from QSYS2.SYSPARTITIONSTAT
                where SYSTEM_TABLE_SCHEMA='{lib}' and SYSTEM_TABLE_NAME='{spf}'
                order by 1
            ";
            using(var sqlCmd = Db2Client.CreateCommand()){
                Console.WriteLine($"Gathering member list of source physical file {lib}/{spf}...\n{sqlString}\n");
                sqlCmd.CommandText = sqlString;
                using(var dbReader = sqlCmd.ExecuteReader()){
                    while(dbReader.Read()){
                        members.Add(SqlUtils.RowToSrcMbr(dbReader));
                    }
                }
            }
            return members;
        }

        public void Disconnect(){
            SshClient?.Disconnect();
            SftpClient?.Disconnect();
            Db2Client?.Close();
        }

        public void Dispose(){
            Disconnect();
        }
    }
}