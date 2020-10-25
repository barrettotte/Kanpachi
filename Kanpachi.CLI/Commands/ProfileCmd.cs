using System.Collections.Generic;
using CommandLine;

namespace Kanpachi.CLI{

    [Verb("profile", HelpText="Manage connection profiles.")]
    class ProfileCmd{
        [Value(0)]
        public IEnumerable<string> SubArgs {get; set;}
    }

    [Verb("add", HelpText="Add a new profile.")]
    class ProfileAdd: BaseCmd{
        [Value(0, HelpText="Name of profile.", Required=true)]
        public string Profile {get; set;}
    }

    [Verb("ls", HelpText="List profiles.")]
    class ProfileLs: BaseCmd{
        // no arguments
    }

    [Verb("rm", HelpText="Remove a profile.")]
    class ProfileRm: BaseCmd{
        [Value(0, HelpText="Name of profile.", Required=true)]
        public string Profile {get; set;}
    }

    [Verb("get_active", HelpText="Get active profile.")]
    class GetProfileActive: BaseCmd{
        // no arguments
    }

    [Verb("set_active", HelpText="Set active profile.")]
    class SetProfileActive: BaseCmd{
        [Value(0, HelpText="Name of profile.", Required=true)]
        public string Profile {get; set;}
    }

    [Verb("set", HelpText="Set profile value.")]
    class ProfileSetValue: BaseCmd{
        [Value(0, HelpText="Key to set.", Required=true)]
        public string Key {get; set;}

        [Value(1, HelpText="Value.", Required=true)]
        public string Value {get; set;}
    }

    [Verb("get", HelpText="Get profile value.")]
    class ProfileGetValue: BaseCmd{
        [Value(0, HelpText="Key to get.", Required=true)]
        public string Key {get; set;}
    }
}