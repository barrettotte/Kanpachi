# IBMi-Editor-API


## Project Phases
1. Viewing source
2. Angular interface
3. VS Code Extension
4. Editing and compiling source


## To Do
* Replace FTP with SFTP
* Unit testing on IBMi object
* File caching logic? Sqlite?
* Flask API over IBMi object
  * GET **/lib** - Get list of libraries
  * GET **/lib/{lib}** - Get library data
    * list of objects and relevant object description info
  * GET **/lib/{lib}/spf/{spf}** - Get source physical file data
    * list of members and relevant object description info
  * GET **/lib/{lib}/spf/{spf}/mbr/{mbr}** - Get source member data
    * source code and relevant object description info
* IBMi object
  * Login
  * Get list of libraries
  * Get list of PF-SRC from each library
  * Get list of members in each SRCPF
  * Get source of a member (only when requested)
  * Get list of PF-DTA/LF from library
  * Gen SQL method
  * FTP handling

