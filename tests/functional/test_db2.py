# Test setting up a DB2 for i connection and querying a table

import test_utils, pyodbc

host,user,pwd = test_utils.get_creds()

conn = pyodbc.connect(driver='{iSeries Access ODBC Driver}', system=host, uid=user, pwd=pwd)
csr = conn.cursor()
csr.execute(' '.join([
    "SELECT TABLE_SCHEMA, TABLE_NAME, TABLE_PARTITION, SOURCE_TYPE",
    "FROM QSYS2.SYSPARTITIONSTAT WHERE TABLE_SCHEMA = 'BOLIB'",
    "ORDER BY TABLE_PARTITION"
]))

for row in csr: print(row)

csr.close()
conn.close()
