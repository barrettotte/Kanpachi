using System;
using System.IO;
using Kanpachi.Lib;

namespace Kanpachi.CLI{

    public class BaseService{
        
        protected KanpachiClient Client {get; set;}

        protected string KanpachiPath {get; set;}  //  /home/<user>/.kanpachi


        public BaseService(): this(new KanpachiClient()){}

        public BaseService(KanpachiClient client){
            Client = client;
            KanpachiPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".kanpachi");
        }
    }
}