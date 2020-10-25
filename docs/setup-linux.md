# Linux Setup

These setup instructions are based on my installation with **Ubuntu 18.04**.
Obviously, these instructions might vary between distributions, but the same general steps should be taken.


## ODBC for IBM i
- Unix ODBC driver (Linux) - ```apt-get install unixodbc unixodbc-dev```
- ODBC drivers on Linux - https://www.ibm.com/support/knowledgecenter/en/SSEPGG_10.5.0/com.ibm.db2.luw.apdv.cli.doc/doc/t0061216.html
- View ODBC drivers with ```vim /etc/odbcinst.ini```, grab one for IBM i named "
