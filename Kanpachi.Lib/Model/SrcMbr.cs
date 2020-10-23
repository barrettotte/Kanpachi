using System;
using Newtonsoft.Json;

namespace Kanpachi.Lib{

    public class SrcMbr : IbmiObject{

        public string Attribute {get; set;}
        public int LineCount {get; set;}
        public int RecordLength {get; set;}
        public int DataSize {get; set;}
        public DateTime Created {get; set;}
        public DateTime Updated {get; set;}

        [JsonIgnore]
        public byte[] Content {get; set;} // actual source code


        public SrcMbr(){
            //
        }

        public SrcMbr(string name, string text, string attribute, int lineCount, 
            int recordLength, int dataSize, DateTime created, DateTime updated
        ) : base(name, text)
        {
            Attribute = attribute;
            LineCount = lineCount;
            RecordLength = recordLength;
            DataSize = dataSize;
            Created = created;
            Updated = updated;
            Content = null;
        }

        public override string ToString(){
            return base.ToString() + $"{Attribute,-10} | {LineCount,5} line(s) | {RecordLength,4} column(s) | {DataSize,5} byte(s)";
        }
    }
}
