using System;
using System.Collections.Generic;
using System.Data;
using ConsoleTables;

namespace Kanpachi.Lib{

    public class ExecService: BaseService{

        public KanpachiProfile Profile {get;}


        public ExecService(KanpachiProfile profile){
            Profile = profile;
        }

        public void ExecCL(string clString){
            using(KanpachiClient client = new KanpachiClient(Profile)){
                var cmd = client.ExecCL(clString);
                if(cmd.ExitCode != 0){
                    throw new KanpachiShellException(
                        $"Command\n\t{cmd.CmdText}\n completed with exit code {cmd.ExitCode}\n\t{cmd.StdErr}");
                }
                Console.WriteLine(cmd.StdOut);
            }
        }

        public void ExecShell(string shellString){
            using(KanpachiClient client = new KanpachiClient(Profile)){
                var cmd = client.ExecShell(shellString);
                if(cmd.ExitCode != 0){
                    throw new KanpachiShellException(
                        $"Command\n\t{cmd.CmdText}\n completed with exit code {cmd.ExitCode}\n\t{cmd.StdErr}");
                }
                Console.WriteLine(cmd.StdOut);
            }
        }

        public void ExecSql(string sqlString){
             using(KanpachiClient client = new KanpachiClient(Profile)){
                DataTable tbl = client.ExecSQL(sqlString);

                if(tbl.Rows.Count > 0){
                    List<string> cols = new List<string>();
                    foreach(DataColumn col in tbl.Columns){
                        cols.Add(col.ColumnName);
                    }
                    ConsoleTable ct = new ConsoleTable(cols.ToArray());

                    foreach(DataRow row in tbl.Rows){
                        ct.AddRow(row.ItemArray);
                    }
                    ct.Write(Format.Alternative);
                } else{
                    Console.WriteLine("Statement executed successfully.");
                }
            }
        }
    }
}