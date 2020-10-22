using System;
using System.IO;

namespace Kanpachi.Lib{

    public class BaseService{

        protected string KanpachiPath {get; set;}  //  /home/<user>/.kanpachi

        public BaseService(){
            KanpachiPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".kanpachi");
        }
    }
}