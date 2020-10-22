using System.Collections.Generic;

namespace Kanpachi.Lib{

    public class SrcPf : IbmiObject{

        public List<SrcMbr> Members {get; set;}


        public SrcPf(){
            Members = new List<SrcMbr>();
        }

        public SrcPf(string name, string text): base(name, text){
            Members = new List<SrcMbr>();
        }

        public override string ToString(){
            return base.ToString() + $"{Members.Count,5} member(s) | ";
        }
    }
}
