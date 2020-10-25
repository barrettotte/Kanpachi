using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;
using System.Threading;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace Kanpachi.Lib{

    public class KanpachiClient: IDisposable{

        public KanpachiProfile Profile {get;}
        public SshClient SshClient {get;}
        public SftpClient SftpClient {get;}
        public OdbcConnection Db2Client {get;}

        private string IfsCache {get;}


        public KanpachiClient(KanpachiProfile profile){
            Profile = profile;

            SshClient = new SshClient(profile.Host, profile.Port, profile.User, profile.PasswordDecrypted);
            SshClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(profile.Timeout);

            SftpClient = new SftpClient(profile.Host, profile.Port, profile.User, profile.PasswordDecrypted);
            SftpClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(profile.Timeout);

            string connStr = "Driver={IBM i Access ODBC Driver};" +
                $"System={profile.Host};Uid={profile.User};Pwd={profile.PasswordDecrypted};NAM=1;DBQ=,*USRLIBL;";
            Db2Client = new OdbcConnection(connStr);
            Db2Client.ConnectionTimeout = (int) Math.Round(profile.Timeout);

            IfsCache = $"{Profile.IfsUserPath}/.kanpachi";
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
    
            if(!SftpClient.Exists(IfsCache)){
                Console.WriteLine($"Creating IFS cache at {IfsCache}.");
                ExecShell($"mkdir -p \"{IfsCache}\""); // swap out for SQL QCMDEXC?
            }
        }

        // Execute a CL command
        public CmdResponse ExecCL(string clString){
            return ExecShell($"system \"{clString}\"");
        }

        // Execute a shell command
        public CmdResponse ExecShell(string shellString){
            SshCommand cmd = SshClient.CreateCommand(shellString);
            IAsyncResult task = cmd.BeginExecute();
            Console.WriteLine(shellString);

            while(!task.IsCompleted){
                Thread.Sleep(500);
            }
            CmdResponse response = new CmdResponse(shellString, cmd.ExitStatus, cmd.Error, cmd.Result);
            cmd.EndExecute(task);

            return response;
        }

        // execute DB2 SQL command
        public DataTable ExecSQL(string sqlString){
            using(var sqlCmd = Db2Client.CreateCommand()){
                sqlCmd.CommandText = sqlString;
                Console.WriteLine(sqlCmd.CommandText + '\n');

                DataTable tbl = new DataTable();
                using(var dbReader = sqlCmd.ExecuteReader()){
                    tbl.Load(dbReader);
                }
                return tbl;
            }
        }

        // list contents of directory in IFS
        public List<SftpFile> ListDirectory(string dirPath){
            try{
                return SftpClient.ListDirectory(dirPath) as List<SftpFile>;
            } catch(Exception e){
                throw new KanpachiSftpException($"Error occurred listing contents of {dirPath}\n\t{e.Message}");
            }
        }

        // download a file from IFS
        public byte[] DownloadFile(string filePath){
            // try{
                return SftpClient.ReadAllBytes(filePath);
            // } catch(Exception e){
            //     throw new KanpachiSftpException($"Error occurred downloading file at {filePath}\n\t{e.Message}");
            // }
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
                select TABLE_PARTITION, coalesce(SOURCE_TYPE,''), NUMBER_ROWS, coalesce(PARTITION_TEXT,''),
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
                select TABLE_PARTITION, coalesce(SOURCE_TYPE,''), NUMBER_ROWS, coalesce(PARTITION_TEXT,''),
                  AVGROWSIZE, DATA_SIZE, CREATE_TIMESTAMP, LAST_CHANGE_TIMESTAMP
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