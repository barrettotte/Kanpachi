-- QSYS2 snippets
-- https://www.ibm.com/support/knowledgecenter/en/SSAE4W_9.6.0/db2/rbafzcatalogtbls.htm
-- https://www.ibm.com/support/knowledgecenter/en/ssw_ibm_i_72/rzajq/rzajqservicessys.htm

-- Get all libraries
select * from QSYS2.SYSSCHEMAS limit 100;

select SYSTEM_SCHEMA_NAME, coalesce(SCHEMA_TEXT,'')
from QSYS2.SYSSCHEMAS;


select SYSTEM_SCHEMA_NAME, SCHEMA_TEXT, 
  SCHEMA_OWNER, SCHEMA_SIZE, CREATION_TIMESTAMP
from QSYS2.SYSSCHEMAS
where SCHEMA_OWNER <> 'QSYS'
order by SYSTEM_SCHEMA_NAME
;

select SYSTEM_SCHEMA_NAME, SCHEMA_TEXT
from QSYS2.SYSSCHEMAS
where SYSTEM_SCHEMA_NAME = 'BOLIB'
;


-- Get objects in library
-- https://www.ibm.com/support/knowledgecenter/en/ssw_ibm_i_72/rzajq/rzajqudfobjectstat.htm
select OBJNAME, OBJTYPE, OBJATTRIBUTE, OBJTEXT, OBJSIZE, OBJLONGNAME,
  (SOURCE_LIBRARY ||'/'|| SOURCE_FILE ||'/'|| SOURCE_MEMBER) as SOURCE_LOCATION,
  OBJCREATED, CHANGE_TIMESTAMP
from table(QSYS2.OBJECT_STATISTICS('BOLIB', '*ALL')) -- optional 3rd parm '*ALLSIMPLE'
;


-- Get source physical files in library
select SYSTEM_TABLE_NAME, TABLE_TEXT
from QSYS2.SYSTABLES
where SYSTEM_TABLE_SCHEMA='BOLIB'
  and FILE_TYPE='S' and TABLE_TYPE='P'
;


-- Get source physical file
select SYSTEM_TABLE_NAME, TABLE_TEXT
from QSYS2.SYSTABLES
where SYSTEM_TABLE_SCHEMA='BOLIB'
  and FILE_TYPE='S' and TABLE_TYPE='P'
  and SYSTEM_TABLE_NAME='QRPGLESRC'
;


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


select *
from QSYS2.SYSPARTITIONSTAT
where SYSTEM_TABLE_SCHEMA='BOLIB'
  and SYSTEM_TABLE_NAME='QSQDSRC'
;


select count(*), SOURCE_TYPE
from QSYS2.SYSPARTITIONSTAT
where SYSTEM_TABLE_SCHEMA='BOLIB'
group by rollup(SOURCE_TYPE)
;


-- Get columns of table
select * 
from QSYS2.SYSCOLUMNS
where TABLE_SCHEMA='BOLIB'
  and TABLE_NAME='PERSON'
;


-- get SQL procedures and functions
select * 
from QSYS2.SYSROUTINES
where SPECIFIC_SCHEMA='BOLIB' 
;


-- get SQL procedures
select * 
from QSYS2.SYSPROCS
where SPECIFIC_SCHEMA='BOLIB'
;


-- get SQL functions
select * 
from QSYS2.SYSFUNCS
where SPECIFIC_SCHEMA='BOLIB'
;


-- get current library list
select * from QSYS2.LIBRARY_LIST_INFO;


-- get spooled files
select OUTPUT_QUEUE_NAME, OUTPUT_QUEUE_LIBRARY_NAME,
  SPOOLED_FILE_NAME, CREATE_TIMESTAMP, JOB_NAME,
  SIZE, TOTAL_PAGES, FORM_TYPE
from QSYS2.OUTPUT_QUEUE_ENTRIES_BASIC
where USER_NAME='OTTEB'
limit 10;


-- execute CL from SQL
call QSYS2.QCMDEXC('DSPLIBL');


-- switch shell to bash
call QSYS2.SET_PASE_SHELL_INFO('*CURRENT', '/QOpenSys/QIBM/ProdData/OPS/tools/bin/bash');


-- get job log
select * from table(QSYS2.JOBLOG_INFO('some job'));

