using CommandLine;

namespace Kanpachi.CLI{

    class BaseCmd{
        [Option('q', "quiet", HelpText="Suppress output.")]
        public bool Quiet {get; set;}

        [Option('v', "verbose", HelpText="Verbose output.")]
        public bool Verbose {get; set;}
    }
}