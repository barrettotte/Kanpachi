using System.Collections.Generic;

namespace Kanpachi.Lib{

    public class Library : IbmiObject{

        public List<SrcPf> SrcPfs {get; set;}


        public Library(){
            SrcPfs = new List<SrcPf>();
        }

        public Library(string name, string text): base(name, text){
            SrcPfs = new List<SrcPf>();
        }
    }
}
