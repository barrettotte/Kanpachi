# CLI Documentation

Basic documentation to show commands available in Kanpachi CLI.

Documentation formatting loosely based on https://developers.google.com/style/code-syntax


## Command Groups
- ```exec``` - command group for executing CL, SQL, and Shell commands.
- ```ifs``` - command group for interacting with IFS.
- ```profile``` - command group for interacting with connection profiles.
- ```qsys``` - command group for interacting with QSYS filesystem.


## Exec Command Group
For each ```exec``` command, use quotation marks to enclose a command with spaces.

```cl CL_STRING``` - execute a CL command

**Example:** ```kanpachi exec cl "DSPLIBL"```

<br>

```sh SHELL_STRING``` - execute a shell command

**Example:** ```kanpachi exec sh "cat /home/OTTEB/hello.rpgle"```

<br>

```sql SQL_STRING``` - execute a SQL command (statement or query)

**Example:** ```kanpachi exec sql "select SYSTEM_SCHEMA_NAME from QSYS2.SYSSCHEMAS limit 25"```

<br>


## IFS Command Group
If optional argument ```OUT_PATH``` is not specified, downloads are placed in current working directory.

```get_dir IFS_PATH [OUT_PATH]``` - download a directory from IFS

**Example:** ```kanpachi ifs get_dir /home/OTTEB/RPG-BF repos/RPG-BF```

<br>

```get_file IFS_PATH [OUT_PATH]``` - download a file from IFS

**Example:** ```kanpachi ifs get_file /home/OTTEB/ifsread.rpgle```

<br>

```ls IFS_PATH``` - list directory contents on IFS

**Example:** ```kanpachi ifs ls /home/OTTEB```

<br>


## Profile Command Group

```profile add PROFILE_NAME``` - add a new connection profile

**Example:** ```kanpachi profile add MY400DEV```

<br>

```profile ls``` - list available connection profiles

**Example:** ```kanpachi profile ls```

<br>

```profile rm PROFILE_NAME``` - remove a connection profile

**Example:** ```kanpachi profile rm MY400DEV```

<br>

```profile get_active``` - get current active profile's name

**Example:** ```kanpachi profile get_active```

<br>

```profile set_active PROFILE_NAME``` - set connection profile as active

**Example:** ```kanpachi profile set_active MY400DEV```

<br>


## Qsys Command Group
If optional argument ```OUT_PATH``` is not specified, downloads are placed in current working directory.

```get_lib QSYS_PATH [OUT_PATH]``` - download a library

**Example:** ```kanpachi qsys get_lib BOLIB```

<br>

```get_spf QSYS_PATH [OUT_PATH]``` - download a source physical file

**Example:** ```kanpachi qsys get_spf BOLIB/QRPGLESRC example/rpgle```

<br>

```get_mbr QSYS_PATH [OUT_PATH]``` - download a source member

**Example:** ```kanpachi qsys get_mbr BOLIB/QRPGLESRC/IFSREAD example/rpgle```

<br>

```ls_lib``` - list libraries on host

**Example:** ```kanpachi qsys ls_lib```

<br>

```ls_spf QSYS_PATH``` - list source physical files in library

**Example:** - ```kanpachi qsys ls_spf BOLIB```

<br>

```ls_mbr QSYS_PATH``` - list members in source physical file

**Example:** - ```kanpachi qsys ls_spf BOLIB/QRPGLESRC```

<br>
