namespace Kanpachi.CLI{

    class Program{

        static void Main(string[] args){
            CmdParser cmdParser = new CmdParser();
            cmdParser.Parse(args);
        }
    }
}