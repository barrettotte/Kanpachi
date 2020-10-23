using System;
using System.Data.Odbc;

namespace Kanpachi.Lib{

    public static class SqlUtils{

        // Simple util to display rows retrieved via ODBC
        public static void DisplayRows(OdbcDataReader reader){
            int colCount = reader.FieldCount;
            for(int i = 0; i < colCount; i++){
                Console.Write($"{reader.GetName(i)}:");
            }
            Console.WriteLine();

            while(reader.Read()){
                for(int i = 0; i < colCount; i++){
                    object obj = reader.GetValue(i);
                    String col = (obj == null ? "NULL" : obj.ToString());
                    Console.Write($"{col},");
                }
                Console.WriteLine();
            }
        }

        // Map ODBC row to source member
        public static SrcMbr RowToSrcMbr(OdbcDataReader reader){
            var mbr = new SrcMbr();
            mbr.Name = reader.GetString(0).Trim();       // TABLE_PARTITION
            mbr.Attribute = reader.GetString(1).Trim();  // SOURCE_TYPE
            mbr.LineCount = reader.GetInt32(2);          // NUMBER_ROWS
            mbr.Text = reader.GetString(3).Trim();       // PARTITION_TEXT
            mbr.RecordLength = reader.GetInt32(4);       // AVGROWSIZE
            mbr.DataSize = reader.GetInt32(5);           // DATA_SIZE
            mbr.Created = reader.GetDateTime(6);         // CREATE_TIMESTAMP
            mbr.Updated = reader.GetDateTime(7);         // LAST_CHANGE_TIMESTAMP
            return mbr;
        }

        // Map ODBC row to source physical file
        public static SrcPf RowToSrcPf(OdbcDataReader reader){
            var spf = new SrcPf();
            spf.Name = reader.GetString(0).Trim();  // SYSTEM_TABLE_NAME
            spf.Text = reader.GetString(1).Trim();  // TABLE_TEXT
            return spf;
        }

        // Map ODBC row to library
        public static Library RowToLib(OdbcDataReader reader){
            var lib = new Library();
            lib.Name = reader.GetString(0).Trim();  // SYSTEM_SCHEMA_NAME
            lib.Text = reader.GetString(1).Trim();  // SCHEMA_TEXT
            return lib;
        }
    }
}