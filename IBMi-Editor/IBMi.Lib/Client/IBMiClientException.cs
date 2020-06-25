using System;


namespace IBMi.Lib.Client{

    public class IBMiClientException: Exception{
        
        public IBMiClientException(){}
        public IBMiClientException(string message): base(message){}

    }
}
