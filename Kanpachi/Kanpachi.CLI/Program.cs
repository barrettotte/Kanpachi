using System;
using System.Security;
using System.Net;
using Kanpachi.Lib;

namespace Kanpachi.CLI{

    class Program{

        static void Main(string[] args){
            // ClientTest test = new ClientTest();
            // test.Test();

            // Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

            // CmdParser cmdParser = new CmdParser();
            // cmdParser.Parse(args);

            // Test Pbkdf2
            SecureString password = new NetworkCredential("", "This is my password").SecurePassword;
            // var hash = Pbkdf2.Hash(password);
            // Console.WriteLine(Pbkdf2.Validate("uhh", hash));
            // Console.WriteLine(Pbkdf2.Validate(password, hash));


            var encrypted = SecUtils.EncryptPassword(password);
            Console.WriteLine(encrypted);

            var decrypted = SecUtils.DecryptPassword(encrypted);
            Console.WriteLine(decrypted);
        }
    }
}