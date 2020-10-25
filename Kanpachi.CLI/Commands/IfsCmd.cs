using System.Collections.Generic;
using CommandLine;

namespace Kanpachi.CLI{

    [Verb("ifs", HelpText="Interact with Integrated File System (IFS).")]
    class IfsCmd{
        [Value(0)]
        public IEnumerable<string> SubArgs {get; set;}
    }

    [Verb("get_dir", HelpText="Download a directory from IFS.")]
    class IfsGetDir : BaseCmd{
        [Value(0, HelpText="Absolute path to directory on IFS.", Required=true)]
        public string ServerPath {get; set;}

        [Value(1, HelpText="Path to download directory to.")]
        public string ClientPath {get; set;}
    }

    [Verb("get_file", HelpText="Download a file from IFS.")]
    class IfsGetFile : BaseCmd{
        [Value(0, HelpText="Absolute path to file on IFS.", Required=true)]
        public string ServerPath {get; set;}

        [Value(1, HelpText="Path to download file to.")]
        public string ClientPath {get; set;}
    }

    [Verb("ls", HelpText="List contents of directory on IFS.")]
    class IfsLs : BaseCmd{
        [Value(0, HelpText="Absolute path to directory on IFS.")]
        public string ServerPath {get; set;}
    }
}