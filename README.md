## Kanpachi

A subpar CLI to interact with IBM i.


## Features
- Fetch files and directories from IFS.
- Fetch libraries, source physical files, and source members from QSYS.
- Execute CL, SQL, and shell commands.


## Client Requirements
- ODBC Driver for IBM i (Details in [Setup](##Setup))


## Host Requirements
- IBM i 7.3+ (```DSPSFWRSC``` to check OS version)
- User profile with home directory setup on IFS.
- **TODO:** Check how things act if default shell not set to bash.


## Setup
- **TODO:** Windows - [docs/setup-windows.md](docs/setup-windows.md)
- Linux - [docs/setup-linux.md](docs/setup-linux.md)
- Mac - Sorry, nothing personal; I don't know a single thing about MacOS.


## Recommended VS Code Extensions
- [IBMi Languages](https://marketplace.visualstudio.com/items?itemName=barrettotte.ibmi-languages) - My syntax highlighter for RPG/RPGLE, CL, DDS, and MI.


## Project Goals
- [ ] Phase 1 - CLI commands to fetch source
- [ ] Phase 2 - CLI commands to execute SQL, CL, and shell
- [ ] Phase 3 - CLI commands to update source
- [ ] Phase 4 - API over Kanpachi.LIB

After the API is written, I plan to make a VS Code extension as a frontend to the Kanpachi API in a separate repository.


## References
- [IBM i 7.3 Documentation](https://www.ibm.com/support/knowledgecenter/en/ssw_ibm_i_73/rzahg/welcome.htm)
- [SSH.NET](https://github.com/sshnet/SSH.NET)
- [RPGPGM](https://www.rpgpgm.com/)
- [CommandLineParser Documentation](https://github.com/commandlineparser/commandline/wiki)