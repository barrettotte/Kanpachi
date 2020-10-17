using System;

namespace Kanpachi.Lib{

    public class KanpachiException: Exception{
        
        public KanpachiException(){}
        public KanpachiException(string message): base(message){}
    }
}
