using CommandLine;

namespace Kanpachi.CLI{

    interface IConfigOptions{
        [Value(0, HelpText="Key of configuration value.", Required=true)]
        string Key {get; set;}

        [Option('p', "profile", HelpText="Profile to target.")]
        string Profile {get; set;}
    }

    [Verb("config-get", HelpText="Get configuration value. (defaults to active)")]
    public class GetConfigOptions : IConfigOptions{
        public string Key {get; set;}
        public string Profile {get; set;}
    }

    [Verb("config-set", HelpText="Set configuration value. (defaults to active)")]
    public class SetConfigOptions : IConfigOptions{
        public string Key {get; set;}
        public string Profile {get; set;}

        [Value(1, HelpText="New configuration value.", Required=true)]
        public string Value {get; set;}
    }

    [Verb("config-reset", HelpText="Reset configuration value. (defaults to active)")]
    public class ResetConfigOptions : IConfigOptions{
        public string Key {get; set;}
        public string Profile {get; set;}

        //TODO: [Option('a', "all", HelpText="Reset entire configuration to default.")]
    }

    // TODO: List key-value pairs from config => ls
    // TODO: Reset active configuration profile back to default

}