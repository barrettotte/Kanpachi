using System;
using CommandLine;
using CommandLine.Text;
using Kanpachi.Lib;

namespace Kanpachi.CLI{

    class CmdParser{

        private void ParseConfigCmd(ConfigCmd baseCmd){
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<ConfigGet, ConfigSet, ConfigLs, ConfigReset>(baseCmd.SubArgs);
            parserResult
                .WithParsed<ConfigGet>(x => Console.WriteLine($"Get config['{x.Key}']"))
                .WithParsed<ConfigLs>(x => Console.WriteLine($"list config"))
                .WithParsed<ConfigReset>(x => Console.WriteLine($"reset config['{x.Key}']"))
                .WithParsed<ConfigSet>(x => Console.WriteLine($"Set config['{x.Key}'] = '{x.Value}'"))
                .WithNotParsed(_ => WriteHelpText(parserResult));
        }

        private void ParseExecCmd(ExecCmd baseCmd){
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<ExecCl, ExecShell, ExecSql>(baseCmd.SubArgs);
            parserResult
                .WithParsed<ExecCl>(x => Console.WriteLine($"Execute CL"))
                .WithParsed<ExecShell>(x => Console.WriteLine($"Execute Shell"))
                .WithParsed<ExecSql>(x => Console.WriteLine($"Execute SQL"))
                .WithNotParsed(_ => WriteHelpText(parserResult));
        }

        private void ParseIfsCmd(IfsCmd baseCmd){
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<IfsGetDir, IfsGetFile, IfsLs>(baseCmd.SubArgs);
            parserResult
                .WithParsed<IfsGetDir>(x => Console.WriteLine($"IFS get directory"))
                .WithParsed<IfsGetFile>(x => Console.WriteLine($"IFS get file"))
                .WithParsed<IfsLs>(x => Console.WriteLine($"IFS list"))
                .WithNotParsed(_ => WriteHelpText(parserResult));
        }

        private void ParseProfileCmd(ProfileCmd baseCmd){
            ProfileService service = new ProfileService();

            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<ProfileAdd, ProfileRm, ProfileLs, GetProfileDefault, SetProfileDefault>(baseCmd.SubArgs);
            parserResult
                .WithParsed<ProfileAdd>(args => service.AddProfile(args.Profile))
                .WithParsed<ProfileLs>(_ => service.ListProfiles())
                .WithParsed<ProfileRm>(args => service.RemoveProfile(args.Profile))
                .WithParsed<GetProfileDefault>(args => Console.WriteLine(service.GetDefaultProfile()))
                .WithParsed<SetProfileDefault>(args => service.SetDefaultProfile(args.Profile))
                .WithNotParsed(_ => WriteHelpText(parserResult));
        }

        private void ParseQsysCmd(QsysCmd baseCmd){
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<QsysGetLib, QsysGetMbr, QsysGetSpf, QsysLsLib, QsysLsSpf>(baseCmd.SubArgs);
            parserResult
                .WithParsed<QsysGetLib>(x => Console.WriteLine($"QSYS get lib"))
                .WithParsed<QsysGetMbr>(x => Console.WriteLine($"QSYS get member"))
                .WithParsed<QsysGetSpf>(x => Console.WriteLine($"QSYS get source physical file"))
                .WithParsed<QsysLsLib>(x => Console.WriteLine($"QSYS list library"))
                .WithParsed<QsysLsSpf>(x => Console.WriteLine($"QSYS list source physical file"))
                .WithNotParsed(_ => WriteHelpText(parserResult));
        }

        // write generic help text. (suppress --help and --version on subcommands)
        private void WriteHelpText(ParserResult<object> result){
            var helpText = HelpText.AutoBuild(result, ht => {
                ht.AutoHelp = false;
                ht.AutoVersion = false;
                return HelpText.DefaultParsingErrorsHandler(result, ht);
            }, e => e);
            Console.WriteLine(helpText);
        }

        // Parse/route main commands to subcommands
        public void Parse(string[] args){
            Parser.Default.ParseArguments<ConfigCmd, ExecCmd, IfsCmd, ProfileCmd, QsysCmd>(args)
                .WithParsed<ConfigCmd>(x => ParseConfigCmd(x))
                .WithParsed<ExecCmd>(x => ParseExecCmd(x))
                .WithParsed<IfsCmd>(x => ParseIfsCmd(x))
                .WithParsed<ProfileCmd>(x => ParseProfileCmd(x))
                .WithParsed<QsysCmd>(x => ParseQsysCmd(x))
            ;
        }

        // TODO: Decide on having an interactive option
        //   - prompt once for password, use it multiple times in memory
        //   - end session
        //   - this is a middle ground between prompt for password and storing encrypted locally
    }
}