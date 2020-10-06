using System.Collections.Generic;
using CommandLine;

namespace Kanpachi.CLI{

    [Verb("profile", HelpText="Manage connection profiles.")]
    class ProfileCmd{
        [Value(0)]
        public IEnumerable<string> SubArgs {get; set;}
    }

    [Verb("add", HelpText="Add a new profile.")]
    class ProfileAdd : BaseCmd{
        [Value(0, HelpText="Name of profile.", Required=true)]
        public string Profile {get; set;}
    }

    [Verb("ls", HelpText="List profiles.")]
    class ProfileLs : BaseCmd{
        [Value(0, HelpText="Name of profile.")]
        public string profile {get; set;}
    }

    [Verb("rm", HelpText="Remove a profile.")]
    class ProfileRm : BaseCmd{
        [Value(0, HelpText="Name of profile.", Required=true)]
        public string Profile {get; set;}
    }

    [Verb("set_dft", HelpText="Set default profile connection.")]
    class ProfileDft : BaseCmd{
        [Value(0, HelpText="Name of profile.")]
        public string profile {get; set;}
    }

    // Idk might not do these:
    // TODO: [Verb("connect", HelpText="Connect to IBM i with profile.")]
    // TODO: [Verb("disconnect", HelpText="Disconnect from IBM i")]

}