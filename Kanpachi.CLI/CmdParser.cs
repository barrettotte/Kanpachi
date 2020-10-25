using System;
using CommandLine;
using CommandLine.Text;
using Kanpachi.Lib;

namespace Kanpachi.CLI{

    class CmdParser{

        private ProfileService profileService;

        private KanpachiProfile GetActiveProfile(){
            var profile = profileService.GetActiveProfile();
            if(profile == null){
                throw new KanpachiProfileException("No active profile set.");
            }
            return profile;
        }

        // TODO: this is gross, refactor it to something...

        private void ParseExecCmd(ExecCmd baseCmd){
            var execService = new ExecService(GetActiveProfile());
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<ExecCl, ExecShell, ExecSql>(baseCmd.SubArgs);
            parserResult
                .WithParsed<ExecCl>(args => execService.ExecCL(args.ClCmd))
                .WithParsed<ExecShell>(args => execService.ExecShell(args.ShellCmd))
                .WithParsed<ExecSql>(args => execService.ExecSql(args.SqlCmd))
                .WithNotParsed(_ => WriteHelpText(parserResult));
        }

        private void ParseIfsCmd(IfsCmd baseCmd){
            var ifsService = new IfsService(GetActiveProfile());
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

        private void ParseQsysCmd(QsysCmd baseCmd){
            var qsysService = new QsysService(GetActiveProfile());
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

            Parser.Default.ParseArguments<ExecCmd, IfsCmd, ProfileCmd, QsysCmd>(args)
                .WithParsed<ExecCmd>(x => ParseExecCmd(x))
                .WithParsed<IfsCmd>(x => ParseIfsCmd(x))
                .WithParsed<ProfileCmd>(x => ParseProfileCmd(x))
                .WithParsed<QsysCmd>(x => ParseQsysCmd(x))
            ;
        }
    }
}