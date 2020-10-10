using Kanpachi.Lib;

namespace Kanpachi.CLI{

    public class BaseService{
        
        protected KanpachiClient Client {get; set;}


        public BaseService(){
            Client = new KanpachiClient();
        }

        public BaseService(KanpachiClient client){
            Client = client;
        }
    }
}