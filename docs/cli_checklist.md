# CLI Checklist

A rough checklist of commands for the IBMi CLI.

<br>

## Profiles
Managing profiles. Prefix: ```profile```
- [ ] ```help | h``` - help
- [ ] ```add | a``` - add a new profile
- [ ] ```rmv | rm``` - delete a profile
- [ ] ```ls  | ls``` - list profiles ('*' on active profile)
- [ ] ```login``` - login with profile
- [ ] ```logout``` - logout from active profile

<br>

## Config
Manage configuration of active profile. Prefix: ```config```
- [ ] ```help | h``` - help
- [ ] ```get | g``` - get value of key from config
- [ ] ```set | s``` - set value of key from config
- [ ] ```list | ls``` - list key-value pairs from config
- [ ] ```reset | rs``` - reset config to default

<br>

## SFTP
IFS. Prefix: ```ifs```
- [ ] ```help | h``` - help
- [ ] ```get_file | gf``` - download a file from IFS
- [ ] ```get_dir | gd``` - download a directory from IFS
- [ ] ```list | ls``` - list contents of IFS path

QSYS. Prefix: ```qsys```
- [ ] ```help | h``` - help
- [ ] ```get_mbr | gm``` - download a source member
- [ ] ```get_spf | gs``` - download a source physical file (all members)
- [ ] ```get_lib | gl``` - download a library
- [ ] ```list_lib | lsl``` - list objects of library
- [ ] ```list_spf | lss``` - list members of source physical file

SPLF. Prefix: ```splf```
- [ ] ```help | h``` - help
- [ ] ```get | g``` - download a SPLF (as PDF)

<br>

## SSH
Prefix: ```exec```

Executing DB2 SQL.
- [ ] ```sql | sql``` - exec SQL string
- [ ] ```sql_file | sqlf``` - exec SQL file

Executing shell commands.
- [ ] ```cmd | cmd``` - execute a command string
- [ ] ```cmd_file | cmdf``` - execute shell file

Executing Control Language. Prefix: ```exec_cl```
- [ ] ```cl | cl``` - execute a CL string
- [ ] ```cl_file | clf``` - execute CL file

Misc.
- [ ] ```help | h``` - help

<br>
