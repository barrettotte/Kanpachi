# Phase 1

A rough checklist for "phase 1" of this project.
For now, I'm focusing on reading IBM i source rather than writing it.

<br>

## Profiles
Managing profiles. Command prefix: ```profile```
- [ ] ```{help | h}``` - help
- [ ] ```{add | a}``` - add a new profile
- [ ] ```{rmv | rm}``` - delete a profile
- [ ] ```{list | ls}``` - list profiles ('*' on active profile)
- [ ] ```login [<profile-name>]``` - login with profile
- [ ] ```logout``` - logout from active profile

<br>

## Config
Manage configuration of active profile. Command prefix: ```config```
- [ ] ```{help | h}``` - help
- [ ] ```{get | g} <key>``` - get value of key from config
- [ ] ```{set | s} <key> <value>``` - set value of key from config
- [ ] ```{list | ls}``` - list key-value pairs from config
- [ ] ```{reset | rs} [<key>]``` - reset entire config (or key) to default

<br>

## SFTP
IFS. Command prefix: ```ifs```
- [ ] ```{help | h}``` - help
- [ ] ```{get_file | gf} <path>``` - download a file from IFS
- [ ] ```{get_dir | gd} <path>``` - recursively download a directory from IFS
- [ ] ```{list | ls} [<path>]``` - list contents of IFS path

QSYS. Command prefix: ```qsys```
- [ ] ```{help | h}``` - help
- [ ] ```{get_mbr | gm} [<lib>/]<src-pf>/<mbr>``` - download a source member
- [ ] ```{get_spf | gs} [<lib>/]<src-pf>``` - download a source physical file (all members)
- [ ] ```{get_lib | gl} <lib>``` - download a library
- [ ] ```{list_spf | lss} [<lib>/]<src-pf>``` - list members of source physical file
- [ ] ```{list_lib | lsl} <lib>``` - list objects of library

SPLF. Command prefix: ```splf```
- [ ] ```{help | h}``` - help
- [ ] ```{get_job | gj} <jobnum>/<user>/<jobname>``` - download SPLFs from job

<br>

## SSH
Command prefix: ```exec```

Executing DB2 SQL.
- [ ] ```sql <sql-string>``` - exec SQL string
- [ ] ```{sql_file | sqlf} <path>``` - exec SQL file

Executing shell commands.
- [ ] ```cmd <cmd-string>``` - execute a command string
- [ ] ```{cmd_file | cmdf} <path>``` - execute shell file

Executing Control Language.
- [ ] ```cl <cmd-string>``` - execute a CL string
- [ ] ```{cl_file | clf} <path>``` - execute CL file

Misc.
- [ ] ```{help | h}``` - help

<br>
