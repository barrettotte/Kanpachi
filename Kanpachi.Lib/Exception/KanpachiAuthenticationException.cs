namespace Kanpachi.Lib{

    public class KanpachiAuthenticationException: KanpachiException{

        public KanpachiAuthenticationException(){}
        public KanpachiAuthenticationException(string message): base(message){}
    }
}