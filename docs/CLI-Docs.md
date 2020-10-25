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

<br>

**Execute a CL command** - ```cl CL_STRING```

Example: ```kanpachi exec cl "DSPLIBL"```

<br>

**Execute a shell command** - ```sh SHELL_STRING```

Example: ```kanpachi exec sh "cat /home/OTTEB/hello.rpgle"```

<br>

**Execute a SQL command (statement or query)** - ```sql SQL_STRING```

Example: ```kanpachi exec sql "select SYSTEM_SCHEMA_NAME from QSYS2.SYSSCHEMAS limit 25"```

<br>


## IFS Command Group
If optional argument ```OUT_PATH``` is not specified, downloads are placed in current working directory.

<br>

**Download a directory from IFS** - ```get_dir IFS_PATH [OUT_PATH]```

Example: ```kanpachi ifs get_dir /home/OTTEB/RPG-BF repos/RPG-BF```

<br>

**Download a file from IFS** - ```get_file IFS_PATH [OUT_PATH]```

Example: ```kanpachi ifs get_file /home/OTTEB/ifsread.rpgle```

<br>

**List directory contents on IFS** - ```ls IFS_PATH```

Example: ```kanpachi ifs ls /home/OTTEB```

<br>


## Profile Command Group

**Add a new connection profile** - ```profile add PROFILE_NAME```

Example: ```kanpachi profile add MY400DEV```

<br>

**List available connection profiles** - ```profile ls```

Example: ```kanpachi profile ls```

<br>

**Remove a connection profile** - ```profile rm PROFILE_NAME```

Example: ```kanpachi profile rm MY400DEV```

<br>

**Get current active profile's name** - ```profile get_active```

Example: ```kanpachi profile get_active```

<br>

**Set connection profile as active** - ```profile set_active PROFILE_NAME```

Example: ```kanpachi profile set_active MY400DEV```

<br>

**Set value on active profile** - ```profile set KEY VALUE```

Example: ```kanpachi profile set port 2222```

<br>

**Get value from active profile** - ```profile get KEY```

Example: ```kanpachi profile get host```

<br>


## Qsys Command Group
If optional argument ```OUT_PATH``` is not specified, downloads are placed in current working directory.

<br>

**Download a library** - ```get_lib QSYS_PATH [OUT_PATH]```

Example: ```kanpachi qsys get_lib BOLIB```

<br>

**Download a source physical file** - ```get_spf QSYS_PATH [OUT_PATH]```

Example: ```kanpachi qsys get_spf BOLIB/QRPGLESRC example/rpgle```

<br>

**Download a source member** - ```get_mbr QSYS_PATH [OUT_PATH]```

Example: ```kanpachi qsys get_mbr BOLIB/QRPGLESRC/IFSREAD example/rpgle```

<br>

**List libraries on host** - ```ls_lib```

Example: ```kanpachi qsys ls_lib```

<br>

**List source physical files in library** - ```ls_spf QSYS_PATH```

Example: - ```kanpachi qsys ls_spf BOLIB```

<br>

**List members in source physical file** - ```ls_mbr QSYS_PATH```

Example: - ```kanpachi qsys ls_spf BOLIB/QRPGLESRC```

<br>
