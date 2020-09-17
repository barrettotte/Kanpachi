using System;


namespace IBMi.Lib.Client{

    public class IbmiClientException: Exception{
        
        public IbmiClientException(){}
        public IbmiClientException(string message): base(message){}

    }
}
