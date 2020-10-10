using System.Collections.Generic;

namespace Kanpachi.Lib{

    public class Library{

        public string Name {get;}
        public string Type {get;}
        public string Text {get;}
        public List<IbmiObject> Objects {get;}

        public Library(string name, string type, string text){
            Name = name;
            Type = type;
            Text = text;
            Objects = new List<IbmiObject>();
        }

        public override string ToString(){
            return $"{Name}  {Type}  {Text}";  // TODO: pad with proper 5250 widths
        }
    }
}
