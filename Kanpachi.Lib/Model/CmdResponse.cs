using System;

namespace Kanpachi.Lib{

    public class CmdResponse{

        public int ExitCode {get; set;}
        public string StdErr {get; set;}
        public string StdOut {get; set;}
        public DateTimeOffset StartTime {get; set;}
        public DateTimeOffset ExitTime {get; set;}
        public TimeSpan RunTime {get; set;}

        public CmdResponse(){}

        public CmdResponse(int exitCode, string stdErr, string stdOut){
            ExitCode = exitCode;
            StdErr = stdErr;
            StdOut = stdOut;
        }

        public override string ToString(){
            return $"Command completed with exit code {ExitCode} in {RunTime.ToString()}.";
        }
    }
}
