using System;


namespace Kanpachi.Lib.Client{

    public class IbmiClientException: Exception{
        
        public IbmiClientException(){}
        public IbmiClientException(string message): base(message){}

    }
}
