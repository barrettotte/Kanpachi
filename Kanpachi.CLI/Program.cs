using System;
using Kanpachi.Lib;

namespace Kanpachi.CLI{

    class Program{

        static void Main(string[] args){
            CmdParser cmdParser = new CmdParser();
            try{
                cmdParser.Parse(args);
            } catch(KanpachiException e){
                Console.WriteLine($"\nERROR(S):\n  {e.Message}");
            } catch(Exception e){
                Console.WriteLine($"\nUNEXPECTED ERROR(S):\n  {e.Message}\n{e.StackTrace}");
            }
        }
    }
}