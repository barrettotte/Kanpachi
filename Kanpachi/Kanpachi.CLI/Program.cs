using System;
using CommandLine;

namespace Kanpachi.CLI{

    class Program{

        static int Foo(){
            return 0;
        }

        static void Main(string[] args){
            // ClientTest test = new ClientTest();
            // test.Test();

            // <object> <verb> <options>
            // config   get    sshport

            if(args.Length > 0){
                Console.Out.WriteLine(String.Join(",", args));
                switch(args[0]){
                    case "config": ParseConfigCmd(args); break;
                }
            }
            
            
            // Parser.Default
            //     .ParseArguments<ConfigOptions, ExecOptions, IfsOptions, ProfileOptions, QsysOptions, SplfOptions>(args)
            //     .MapResult(
            //         (ConfigOptions o) => {
            //             return 0;
            //         },
            //         (ExecOptions o) => {
            //             return 0;
            //         },
            //         (IfsOptions o) => {
            //             return 0;
            //         },
            //         (ProfileOptions o) => {
            //             return 0;
            //         },
            //         (QsysOptions o) => {
            //             return 0;
            //         },
            //         (SplfOptions o) => {
            //             return 0;
            //         },
            //         e => 1
            //     );
        }
    }
}