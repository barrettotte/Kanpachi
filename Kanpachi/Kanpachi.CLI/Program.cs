namespace Kanpachi.CLI{

    class Program{

        static void Main(string[] args){
            // ClientTest test = new ClientTest();
            // test.Test();

            // Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

            CmdParser cmdParser = new CmdParser();
            cmdParser.Parse(args);
        }
    }
}