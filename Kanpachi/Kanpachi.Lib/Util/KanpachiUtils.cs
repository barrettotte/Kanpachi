using System;
using System.Net;
using System.Security;

namespace Kanpachi.Lib{

    public static class KanpachiUtils{

        // Get user and password
        public static NetworkCredential GetCredentials(string inHost="", string inUser=""){
            string user = string.Empty;
            string host = string.Empty;
            SecureString pwd = new SecureString();

            if(string.IsNullOrEmpty(inHost)){
                Console.Write($"Enter host: ");
                inHost = Console.ReadLine();

                if(string.IsNullOrEmpty(inHost)){
                    throw new KanpachiException("Host cannot be blank.");
                }
            }
            host = inHost;

            if(string.IsNullOrEmpty(inUser)){
                Console.Write($"Enter user for {host}: ");
                inUser = Console.ReadLine();

                if(string.IsNullOrEmpty(inUser)){
                    throw new KanpachiException("User cannot be blank.");
                }
            }
            user = inUser;

            Console.Write($"Enter password for {user}@{host}: ");
            while(true){
                ConsoleKeyInfo i = Console.ReadKey(true);
                if(i.Key == ConsoleKey.Enter){
                    break;
                } else if(i.Key == ConsoleKey.Backspace && pwd.Length > 0){
                    pwd.RemoveAt(pwd.Length - 1);
                    Console.Write("\b \b");
                } else if(i.KeyChar != '\u0000'){
                    pwd.AppendChar(i.KeyChar);
                    Console.Write("*");
                }
            }
            Console.Write("\n");
            return new NetworkCredential(user, pwd, host);
        }
    }
}
