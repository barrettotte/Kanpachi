using System;
using Kanpachi.Lib;

namespace Kanpachi.CLI{

    class Program{

        static void Main(string[] args){
            CmdParser cmdParser = new CmdParser();
            cmdParser.Parse(args);


            // var profile = new KanpachiProfile("myprofile", "SOME400", "OTTEB");
            // profile.PasswordDecrypted = "mypassword123";
            
            // profile.Password = SecUtils.EncryptProfile(profile);
            // Console.WriteLine(profile.Password);

            // profile.PasswordDecrypted = SecUtils.DecryptProfile(profile);
            // Console.WriteLine(profile.PasswordDecrypted);

            // Console.WriteLine("--------------------");

            // profile.Password = SecUtils.EncryptProfile(profile);
            // Console.WriteLine(profile.Password);

            // profile.PasswordDecrypted = SecUtils.DecryptProfile(profile);
            // Console.WriteLine(profile.PasswordDecrypted);
        }
    }
}