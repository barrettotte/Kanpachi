using System.Collections.Generic;
using CommandLine;

namespace Kanpachi.CLI{

    [Verb("config", HelpText="Connection profile configuration.")]
    class ConfigCmd{
        [Value(0)]
        public IEnumerable<string> SubArgs {get; set;}
    }

    [Verb("get", HelpText="Get configuration value.")]
    class ConfigGet : BaseCmd{
        [Value(0, HelpText="Key of configuration value.", Required=true)]
        public string Key {get; set;}
    }

    [Verb("ls", HelpText="List key-value pairs from configuration profile")]
    class ConfigLs : BaseCmd{
        // no arguments
    }

    [Verb("reset", HelpText="Reset configuration value.")]
    class ConfigReset : BaseCmd{
        [Value(0, HelpText="Key of configuration value.", Required=true)]
        public string Key {get; set;}
    }

    [Verb("set", HelpText="Set configuration value.")]
    class ConfigSet : BaseCmd{
        [Value(0, HelpText="Key of configuration value.", Required=true)]
        public string Key {get; set;}

        [Value(1, HelpText="New configuration value.", Required=true)]
        public string Value {get; set;}
    }

    // TODO: help command
}