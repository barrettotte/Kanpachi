using System.Collections.Generic;

namespace IBMi.Lib.Client{

    public class SrcPf : IbmiObject{

        public List<string> Members {get;}

        public SrcPf(string name, string type, string attribute, string text) : base(name, type, attribute, text){
            this.Members = new List<string>();
        }

        public override string ToString(){
            return base.ToString() + $" - {Members.Count} members";
        }
    }
}
