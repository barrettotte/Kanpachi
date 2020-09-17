namespace IBMi.Lib.Client{

    public class CmdResponse{

        public int Status {get;}
        public string StdErr {get;}
        public string StdOut {get;}

        public CmdResponse(int status, string stdErr, string stdOut){
            Status = status;
            StdErr = stdErr;
            StdOut = stdOut;
        }

        public override string ToString(){
            return $"Command completed with status {Status}.\n{StdOut}\n{StdErr}";
        }
    }
}
