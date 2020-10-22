using System.Collections.Generic;
using CommandLine;

namespace Kanpachi.CLI{

    [Verb("exec", HelpText="Execute something on IBM i.")]
    class ExecCmd{
        [Value(0)]
        public IEnumerable<string> SubArgs {get; set;}
    }

    [Verb("cl", HelpText="Execute a Control Language (CL) command.")]
    class ExecCl : BaseCmd{
        [Value(0, HelpText="CL command string", Required=true)]
        public string ClCmd {get; set;}
    }

    [Verb("sh", HelpText="Execute a shell command.")]
    class ExecShell : BaseCmd{
        [Value(0, HelpText="Shell command string", Required=true)]
        public string ShellCmd {get; set;}
    }

    [Verb("sql", HelpText="Execute a SQL string.")]
    class ExecSql : BaseCmd{
        [Value(0, HelpText="SQL string", Required=true)]
        public string SqlCmd {get; set;}
    }

    // TODO: help command
}