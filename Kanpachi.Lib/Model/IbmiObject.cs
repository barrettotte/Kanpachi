namespace Kanpachi.Lib{

    public class IbmiObject{

        public string Name {get; set;}
        public string Text {get; set;}


        public IbmiObject(){
            //
        }

        public IbmiObject(string name, string text){
            Name = name;
            Text = text;
        }

        public override string ToString(){
            return $"| {Name,-10} | {Text,-50} | ";
        }
    }
}
