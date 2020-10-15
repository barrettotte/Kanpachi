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

            string password = "this is my password";
            var key = SecUtils.DeriveKey("OTTEB_MY400");

            byte[] encrypted = SecUtils.EncryptAes(key, password);
            Console.WriteLine(Convert.ToBase64String(encrypted));

            string unencrypted = SecUtils.DecryptAes(key, encrypted);
            Console.WriteLine(unencrypted);
        }
    }
}