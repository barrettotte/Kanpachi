using System.Collections.Generic;
using CommandLine;

namespace Kanpachi.CLI{


    [Verb("exec", HelpText="Execute something on IBM i.")]
    class ExecOptions{
            
        [Option("sql", HelpText="Execute SQL string.")]
        public string ExecSql {get; set;}

        [Option("cmd", HelpText="Execute a shell command.")]
        public string ExecCmd {get; set;}

        [Option("cl", HelpText="Execute a Control Language (CL) command")]
        public string ExecCl {get; set;}
    }

    [Verb("ifs", HelpText="Interact with Integrated File System (IFS).")]
    class IfsOptions{

        [Option("get_file", HelpText="Download file from IFS.")]
        public string GetFile {get; set;}

        [Option("get_dir", HelpText="Download a directory from IFS.")]
        public string GetDir {get; set;}

        [Option("ls", HelpText="List contents of directory on IFS.")]
        public string ListDir {get; set;}
    }



    [Verb("qsys", HelpText="Interact with QSYS Library.")]
    class QsysOptions{

        [Option("get_mbr", HelpText="Download a source member.")]
        public string GetMbr {get; set;}

        [Option("get_spf", HelpText="Download a source physical file.")]
        public string GetSpf {get; set;}

        [Option("get_lib", HelpText="Download a library.")]
        public string GetLib {get; set;}
            
        [Option("ls_spf", HelpText="List source members of source physical file.")]
        public string ListSpf {get; set;}

        [Option("ls_lib", HelpText="List source physical files in library.")]
        public string ListLib {get; set;}
    }

    [Verb("splf", HelpText="Interact with Spooled Files.")]
    class SplfOptions{

        [Option("getjob", HelpText="Download spooled files of a job.")]
        public string GetJob {get; set;}
    }
}