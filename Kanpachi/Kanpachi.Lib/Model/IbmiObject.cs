namespace Kanpachi.Lib{

    public class IbmiObject{

        public string Name {get;}
        public string Type {get;}        // TODO: enum
        public string Attribute {get;}   // TODO: enum
        public string Text {get;}

        public IbmiObject(string name, string type, string attribute, string text){
            Name = name;
            Type = type;
            Attribute = attribute;
            Text = text;
        }

        public override string ToString(){
            return $"{Name}  |  {Type}  |  {Attribute}  |  {Text}";  // TODO: 5250 emulator length
        }
    }
}
