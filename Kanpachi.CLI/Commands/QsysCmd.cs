using System.Collections.Generic;
using CommandLine;

namespace Kanpachi.CLI{

    [Verb("qsys", HelpText="Interact with QSYS File System.")]
    class QsysCmd{
        [Value(0)]
        public IEnumerable<string> SubArgs {get; set;}
    }

    [Verb("get_lib", HelpText="Download a library from QSYS.")]
    class QsysGetLib : BaseCmd{
        [Value(0, HelpText="Name of library.", Required=true)]
        public string ServerPath {get; set;}

        [Value(1, HelpText="Path to download library to.")]
        public string ClientPath {get; set;}
    }

    [Verb("get_mbr", HelpText="Download a source member from QSYS.")]
    class QsysGetMbr : BaseCmd{
        [Value(0, HelpText="Path to source member in QSYS. (LIB/SRCPF/MBR)", Required=true)]
        public string ServerPath {get; set;}

        [Value(1, HelpText="Path to download source member to.")]
        public string ClientPath {get; set;}
    }

    [Verb("get_spf", HelpText="Download a source physical file from QSYS.")]
    class QsysGetSpf : BaseCmd{
        [Value(0, HelpText="Path to source physical file in QSYS (LIB/SRCPF).", Required=true)]
        public string ServerPath {get; set;}

        [Value(1, HelpText="Path to download source physical file to.")]
        public string ClientPath {get; set;}
    }

    [Verb("ls_lib", HelpText="List libraries on host.")]
    class QsysLsLib : BaseCmd{
        // no arguments
    }

    [Verb("ls_spf", HelpText="List source physical files in library.")]
    class QsysLsSpf : BaseCmd{
        [Value(0, HelpText="Name of Library.", Required=true)]
        public string ServerPath {get; set;}
    }

    [Verb("ls_mbr", HelpText="List source members in source physical file.")]
    class QsysLsMbr : BaseCmd{
        [Value(0, HelpText="Path to source physical file in QSYS (LIB/SRCPF).", Required=true)]
        public string ServerPath {get; set;}
    }

    // TODO: help command
}