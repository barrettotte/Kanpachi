using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Kanpachi.CLI{

    class Program{

        //TODO: DRY!

        private static void ParseConfigCmd(ConfigCmd baseCmd){
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<ConfigGet, ConfigSet, ConfigLs, ConfigReset>(baseCmd.SubArgs);
            parserResult
                .WithParsed<ConfigGet>(x => Console.WriteLine($"Get config['{x.Key}']"))
                .WithParsed<ConfigLs>(x => Console.WriteLine($"list config"))
                .WithParsed<ConfigReset>(x => Console.WriteLine($"reset config['{x.Key}']"))
                .WithParsed<ConfigSet>(x => Console.WriteLine($"Set config['{x.Key}'] = '{x.Value}'"))
                .WithNotParsed(x => {
                    var helpText = HelpText.AutoBuild(parserResult, ht => {
                        ht.AutoHelp = false;
                        ht.AutoVersion = false;
                        return HelpText.DefaultParsingErrorsHandler(parserResult, ht);
                    }, e => e);
                    Console.WriteLine(helpText);
                });
        }

        private static void ParseExecCmd(ExecCmd baseCmd){
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<ExecCl, ExecShell, ExecSql>(baseCmd.SubArgs);
            parserResult
                .WithParsed<ExecCl>(x => Console.WriteLine($"Execute CL"))
                .WithParsed<ExecShell>(x => Console.WriteLine($"Execute Shell"))
                .WithParsed<ExecSql>(x => Console.WriteLine($"Execute SQL"))
                .WithNotParsed(x => {
                    var helpText = HelpText.AutoBuild(parserResult, ht => {
                        ht.AutoHelp = false;
                        ht.AutoVersion = false;
                        return HelpText.DefaultParsingErrorsHandler(parserResult, ht);
                    }, e => e);
                    Console.WriteLine(helpText);
                });
        }

        private static void ParseIfsCmd(IfsCmd baseCmd){
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<IfsGetDir, IfsGetFile, IfsLs>(baseCmd.SubArgs);
            parserResult
                .WithParsed<IfsGetDir>(x => Console.WriteLine($"IFS get directory"))
                .WithParsed<IfsGetFile>(x => Console.WriteLine($"IFS get file"))
                .WithParsed<IfsLs>(x => Console.WriteLine($"IFS list"))
                .WithNotParsed(x => {
                    var helpText = HelpText.AutoBuild(parserResult, ht => {
                        ht.AutoHelp = false;
                        ht.AutoVersion = false;
                        return HelpText.DefaultParsingErrorsHandler(parserResult, ht);
                    }, e => e);
                    Console.WriteLine(helpText);
                });
        }

        private static void ParseProfileCmd(ProfileCmd baseCmd){
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<ProfileAdd, ProfileRm, ProfileLs, ProfileDft>(baseCmd.SubArgs);
            parserResult
                .WithParsed<ProfileAdd>(x => Console.WriteLine($"Add profile"))
                .WithParsed<ProfileLs>(x => Console.WriteLine($"List profiles"))
                .WithParsed<ProfileRm>(x => Console.WriteLine($"Remove profile"))
                .WithParsed<ProfileDft>(x => Console.WriteLine($"Set default profile"))
                .WithNotParsed(x => {
                    var helpText = HelpText.AutoBuild(parserResult, ht => {
                        ht.AutoHelp = false;
                        ht.AutoVersion = false;
                        return HelpText.DefaultParsingErrorsHandler(parserResult, ht);
                    }, e => e);
                    Console.WriteLine(helpText);
                });
        }

        private static void ParseQsysCmd(QsysCmd baseCmd){
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<QsysGetLib, QsysGetMbr, QsysGetSpf, QsysLsLib, QsysLsSpf>(baseCmd.SubArgs);
            parserResult
                .WithParsed<QsysGetLib>(x => Console.WriteLine($"QSYS get lib"))
                .WithParsed<QsysGetMbr>(x => Console.WriteLine($"QSYS get member"))
                .WithParsed<QsysGetSpf>(x => Console.WriteLine($"QSYS get source physical file"))
                .WithParsed<QsysLsLib>(x => Console.WriteLine($"QSYS list library"))
                .WithParsed<QsysLsSpf>(x => Console.WriteLine($"QSYS list source physical file"))
                .WithNotParsed(x => {
                    var helpText = HelpText.AutoBuild(parserResult, ht => {
                        ht.AutoHelp = false;
                        ht.AutoVersion = false;
                        return HelpText.DefaultParsingErrorsHandler(parserResult, ht);
                    }, e => e);
                    Console.WriteLine(helpText);
                });
        }

        static void Main(string[] args){
            // ClientTest test = new ClientTest();
            // test.Test();

            // <object> <verb> <options>
            // config   get   -k sshport
            // config   set   -k sshport -v 22

            // Parse base commands
            Parser.Default.ParseArguments<ConfigCmd, ExecCmd, IfsCmd, ProfileCmd, QsysCmd>(args)
                .WithParsed<ConfigCmd>(x => ParseConfigCmd(x))
                .WithParsed<ExecCmd>(x => ParseExecCmd(x))
                .WithParsed<IfsCmd>(x => ParseIfsCmd(x))
                .WithParsed<ProfileCmd>(x => ParseProfileCmd(x))
                .WithParsed<QsysCmd>(x => ParseQsysCmd(x))
            ;
        }
    }
}