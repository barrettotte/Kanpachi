using CommandLine;

namespace Kanpachi.CLI{

    [Verb("add-profile", HelpText="Add a new profile.")]
    class AddProfileOptions{

    }

    [Verb("rm-profile", HelpText="Remove a profile.")]
    class RmProfileOptions{

    }

    // TODO: [Verb("ls-profile", HelpText="List profiles.")]

    // Idk might not do these:
    // TODO: [Verb("connect", HelpText="Connect to IBM i with profile.")]
    // TODO: [Verb("disconnect", HelpText="Disconnect from IBM i")]

}