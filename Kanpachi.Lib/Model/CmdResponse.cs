namespace Kanpachi.Lib{

    public class CmdResponse{
        
        public string CmdText {get; set;}
        public int ExitCode {get; set;}
        public string StdErr {get; set;}
        public string StdOut {get; set;}


        public CmdResponse(){
            //
        }

        public CmdResponse(string cmdText, int exitCode, string stdErr, string stdOut){
            CmdText = cmdText;
            ExitCode = exitCode;
            StdErr = stdErr;
            StdOut = stdOut;
        }

        public override string ToString(){
            return $"Command \n\t {CmdText}\ncompleted with exit code {ExitCode}.\n{StdOut}";
        }
    }
}
