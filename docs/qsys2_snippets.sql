﻿-- SQL snippets



-- Get all libraries
select * from QSYS2.SYSSCHEMAS limit 100;

select SYSTEM_SCHEMA_NAME, SCHEMA_TEXT, 
  SCHEMA_OWNER, SCHEMA_SIZE, CREATION_TIMESTAMP
from QSYS2.SYSSCHEMAS
where SCHEMA_OWNER <> 'QSYS'
order by SYSTEM_SCHEMA_NAME
;


-- Get source physical files
select SYSTEM_TABLE_NAME, TABLE_TEXT, ROW_LENGTH
from QSYS2.SYSTABLES
where SYSTEM_TABLE_SCHEMA='BOLIB'
  and FILE_TYPE='S' and TABLE_TYPE='P'
limit 10;


-- Get source member list with details
select
  TABLE_PARTITION, SOURCE_TYPE,
  NUMBER_ROWS, PARTITION_TEXT,
  AVGROWSIZE,
  DATA_SIZE,
  CREATE_TIMESTAMP, 
  LAST_CHANGE_TIMESTAMP
from QSYS2.SYSPARTITIONSTAT
where SYSTEM_TABLE_SCHEMA='BOLIB'
  and SYSTEM_TABLE_NAME='QRPGLESRC'
;




