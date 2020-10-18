using System;

namespace Kanpachi.Lib{

    public class SrcMbr{

        public string Name {get; set;}
        public string Type {get; set;}
        public int LineCount {get; set;}
        public string Text {get; set;}
        public int RecordLength {get; set;}
        public int Size {get; set;}
        public DateTime Created {get; set;}
        public DateTime Updated {get; set;}
        public byte[] Content {get; set;}


        public SrcMbr(){}

        public SrcMbr(string name, string type, int lineCount, string text, 
          int recordLength, int size, DateTime created, DateTime updated){
            Name = name;
            Type = type;
            LineCount = lineCount;
            Text = text;
            RecordLength = recordLength;
            Size = size;
            Created = created;
            Updated = updated;
            Content = null;
        }

        public override string ToString(){
            return $"{Name,10} | {Type,10} | {Text,50} | {LineCount,5} line(s) | {Size,6} byte(s)";
        }
    }
}
