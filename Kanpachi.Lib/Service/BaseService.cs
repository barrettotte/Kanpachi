using System;
using System.IO;
using Kanpachi.Lib;

namespace Kanpachi.CLI{

    public class BaseService{

        protected string KanpachiPath {get; set;}  //  /home/<user>/.kanpachi

        public BaseService(){
            KanpachiPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".kanpachi");
        }
    }
}