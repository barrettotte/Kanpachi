using System;
using Kanpachi.Lib;

namespace Kanpachi.CLI{

    class Program{

        static void Main(string[] args){
            // ClientTest test = new ClientTest();
            // test.Test();

            // Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

            // CmdParser cmdParser = new CmdParser();
            // cmdParser.Parse(args);

            // Test encryption
            var password = "helloworld123";
            var hash = Pbkdf2.Hash(password);

            Console.WriteLine(Pbkdf2.Validate("uhh", hash));
            Console.WriteLine(Pbkdf2.Validate(password, hash));
        }
    }
}