using CommandLine;

namespace Kanpachi.CLI{

    class BaseCmd{
        [Option('q', "quiet", HelpText="Suppress all output.")]
        public bool Quiet {get; set;}
    } // TODO:
}