## Kanpachi

A subpar CLI to interact with IBM i.

This has just been a little pet project I've been casually working on.
I've only really tested it on my work's system with V7R3. 
So, this might not work very well for other people.


## Features
- Connection profiles for different systems/users.
- Fetch files and directories from IFS.
- Fetch libraries, source physical files, and source members from QSYS.
- Execute CL, SQL, and shell commands.


## Host Requirements
- IBM i 7.3+ (```DSPSFWRSC``` to check OS version)
- User profile with home directory setup on IFS.
- **TODO:** Check how things act if default shell not set to bash.


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


## References
- [IBM i 7.3 Documentation](https://www.ibm.com/support/knowledgecenter/en/ssw_ibm_i_73/rzahg/welcome.htm)
- [SSH.NET](https://github.com/sshnet/SSH.NET)
- [RPGPGM](https://www.rpgpgm.com/)
- [CommandLineParser Documentation](https://github.com/commandlineparser/commandline/wiki)
