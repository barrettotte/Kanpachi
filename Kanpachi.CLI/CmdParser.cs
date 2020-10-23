using System;
using CommandLine;
using CommandLine.Text;
using Kanpachi.Lib;

namespace Kanpachi.CLI{

    class CmdParser{

        private ProfileService profileService;

        // TODO: this is gross, refactor it to something...

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
            var profile = profileService.GetActiveProfile();
            if(profile == null){
                throw new KanpachiProfileException("No active profile set.");
            }
            var ifsService = new IfsService(profile);
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<IfsGetDir, IfsGetFile, IfsLs>(baseCmd.SubArgs);

            parserResult
                .WithParsed<IfsGetDir>(args => ifsService.GetDirectory(args.ServerPath, args.ClientPath))
                .WithParsed<IfsGetFile>(args => ifsService.GetFile(args.ServerPath, args.ClientPath))
                .WithParsed<IfsLs>(args => ifsService.ListDirectory(args.ServerPath))
                .WithNotParsed(_ => WriteHelpText(parserResult));
        }

        private void ParseProfileCmd(ProfileCmd baseCmd){
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<ProfileAdd, ProfileRm, ProfileLs, GetProfileActive, SetProfileActive>(baseCmd.SubArgs);
            parserResult
                .WithParsed<ProfileAdd>(args => profileService.AddProfile(args.Profile))
                .WithParsed<ProfileLs>(_ => profileService.ListProfiles())
                .WithParsed<ProfileRm>(args => profileService.RemoveProfile(args.Profile))
                .WithParsed<GetProfileActive>(args => Console.WriteLine(profileService.GetActiveProfile(true).Name))
                .WithParsed<SetProfileActive>(args => profileService.SetActiveProfile(args.Profile))
                .WithNotParsed(_ => WriteHelpText(parserResult));
        }

        // TODO: slow because of decryption...
        private void ParseQsysCmd(QsysCmd baseCmd){
            var profile = profileService.GetActiveProfile();
            if(profile == null){
                throw new KanpachiProfileException("No active profile set.");
            }
            var qsysService = new QsysService(profile);
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<QsysGetLib, QsysGetMbr, QsysGetSpf, QsysLsLib, QsysLsSpf>(baseCmd.SubArgs);

            parserResult
                .WithParsed<QsysGetLib>(args => qsysService.GetLibrary(args.ServerPath, args.ClientPath))
                .WithParsed<QsysGetMbr>(args => qsysService.GetMember(args.ServerPath, args.ClientPath))
                .WithParsed<QsysGetSpf>(args => qsysService.GetSpf(args.ServerPath, args.ClientPath))
                .WithParsed<QsysLsLib>(_ => qsysService.ListLibraries())
                .WithParsed<QsysLsSpf>(args => qsysService.ListSpfs(args.ServerPath))
                .WithParsed<QsysLsMbr>(args => qsysService.ListMembers(args.ServerPath))
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
            profileService = new ProfileService();

            Parser.Default.ParseArguments<ConfigCmd, ExecCmd, IfsCmd, ProfileCmd, QsysCmd>(args)
                .WithParsed<ConfigCmd>(x => ParseConfigCmd(x))
                .WithParsed<ExecCmd>(x => ParseExecCmd(x))
                .WithParsed<IfsCmd>(x => ParseIfsCmd(x))
                .WithParsed<ProfileCmd>(x => ParseProfileCmd(x))
                .WithParsed<QsysCmd>(x => ParseQsysCmd(x))
            ;
        }

        // TODO: Decide on having an interactive option
        //   - prompt once for password, use it multiple times in memory for interactive session
        //   - this is a middle ground between prompt for password and storing encrypted locally
    }
}