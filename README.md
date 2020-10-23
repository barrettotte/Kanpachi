## Kanpachi

A subpar CLI for interacting with IBM i.

This has just been a little toy project I've been casually working on.
I've only really tested it on my work's system with V7R3 and a bit on PUB400 with V7R4. 
So, this might not work very well for other people.


I'm sure someone could take some ideas or snippets from this and make something way better.
This definitely needs a lot more polishing.


## Features
- Connection profiles for different systems/users.
- Fetch files and directories from IFS.
- Fetch libraries, source physical files, and source members from QSYS.
- Execute CL, SQL, and shell commands.


## Host Requirements
- IBM i V7R3+ - ```DSPSFWRSC``` to check OS version.
- User profile with home directory setup on IFS.


## Setup
- **TODO:** Windows - [docs/setup-windows.md](docs/setup-windows.md)
- **TODO** Linux - [docs/setup-linux.md](docs/setup-linux.md)
- Mac - Sorry, nothing personal; I don't know a single thing about MacOS.


## Recommended VS Code Extensions
- [IBMi Languages](https://marketplace.visualstudio.com/items?itemName=barrettotte.ibmi-languages) - My syntax highlighter for RPG, RPGLE, CL, DDS, and MI.


## Project Goals
- [ ] Phase 1 - CLI commands to create profiles, fetch source, and execute commands (SQL, CL, shell)
- [ ] Phase 2 - CLI commands to update source on IFS and QSYS
- [ ] Phase 3 - API over Kanpachi.LIB
- [ ] Phase 4 - VS Code extension (frontend) over Kanpachi.API (separate repository)

I'm only working on this project when I'm in the mood to. So, it might be awhile on a lot of this, if at all.


## Additional Ideas
Stuff I was thinking about while developing, but haven't really started.

- Run Kanpachi CLI interactively (would need to add exit command)
- Generate markdown file for each library's QSYS metadata.
- Command to update local QSYS files based off QSYS metadata updated timestamp. (like ```git pull```)
- Command to download a spooled file or all spooled files for a job.


## References
- [IBM i 7.3 Documentation](https://www.ibm.com/support/knowledgecenter/en/ssw_ibm_i_73/rzahg/welcome.htm)
- [IBM i DB2 Connection String Keywords](https://www.ibm.com/support/knowledgecenter/ssw_ibm_i_74/rzaik/connectkeywords.htm)
- [SSH.NET](https://github.com/sshnet/SSH.NET)
- [RPGPGM](https://www.rpgpgm.com/)
- [CommandLineParser Documentation](https://github.com/commandlineparser/commandline/wiki)
