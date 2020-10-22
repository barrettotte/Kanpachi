using System.Collections.Generic;

namespace Kanpachi.Lib{

    public class Library : IbmiObject{

        public List<SrcPf> SrcPfs {get; set;}


        public Library(){
            //
        }

        public Library(string name, string text): base(name, text){
            SrcPfs = new List<SrcPf>();
        }

        public override string ToString(){
            return base.ToString() + $" | {SrcPfs.Count,5} source physical file(s)";
        }
    }
}
