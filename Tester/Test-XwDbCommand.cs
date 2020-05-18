using System;
using XwMaxLib.Data;

namespace Tester
{
    partial class Program
    {
        //***********************************************************************************
        static public void MenuXwDbCommand()
        {
            while (true)
            {
                Console.ResetColor();
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("------------------------- XwDbCommand ---------------------------");
                Console.WriteLine("0 - Main Menu");
                Console.WriteLine("1 - MSSQL - SP - Dataset (Needs TestDB)");
                Console.WriteLine("2 - MSSQL - SP - DataReader (Needs TestDB)");
                Console.WriteLine("3 - MSSQL - Format Query (Needs TestDB)");
                Console.WriteLine("4 - MYSQL - SP - Dataset (Needs TestDB)");
                Console.WriteLine("5 - MYSQL - SP - DataReader (Needs TestDB)");
                Console.WriteLine("6 - MYSQL - Format Query (Needs TestDB)");
                Console.WriteLine("7 - MYSQL - Make Query (Needs TestDB)");
                Console.WriteLine("8 - SQLITE - Format Query (Needs TestDB)");
                Console.WriteLine("9 - Many MySQL Connections (Needs TestDB)");
                Console.ResetColor();
                Console.Write("Select option: ");
                string option = Console.ReadLine().Trim();
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("--------------------------- Run Test ----------------------------");
                Console.ResetColor();
                switch (option)
                {
                    case "0":
                        return;
                    case "1":
                        MSSQL_SP_DATASET();
                        break;
                    case "2":
                        MSSQL_SP_DATAREADER();
                        break;
                    case "3":
                        MSSQL_FORMAT_QUERY();
                        break;
                    case "4":
                        MYSQL_SP_DATASET();
                        break;
                    case "5":
                        MYSQL_SP_DATAREADER();
                        break;
                    case "6":
                        MYSQL_FORMAT_QUERY();
                        break;
                    case "7":
                        MYSQL_MAKE_QUERY();
                        break;
                    case "8":
                        SQLITE_FORMAT_QUERY();
                        break;
                    case "9":
                        MYSQL_MANY_CONNS();
                        break;
                    default:
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Unknown option");
                        }
                        break;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("----------------------------- DONE ------------------------------");
                Console.ReadKey();
            }
        }

        //***********************************************************************************
        static public void MSSQL_SP_DATASET()
        {
            using (XwDbCommand sql = new XwDbCommand("DBCONN_MSTEST"))
            {
                sql.Mode = XwDbMode.DataSet;
                sql.AddParameter("@Name", "%C%");
                sql.ExecuteSP("TestSP");
                Console.WriteLine(sql.GetDebugCommand());
                if (sql.ReturnedData)
                {
                    Console.WriteLine($"CNT: {sql.RowCount}");
                    while (sql.Read())
                    {
                        Console.WriteLine(sql.Value("Name").ToString());
                    }
                }
            }
        }

        //***********************************************************************************
        static public void MSSQL_SP_DATAREADER()
        {
            using (XwDbCommand sql = new XwDbCommand("DBCONN_MSTEST"))
            {
                sql.Mode = XwDbMode.DataReader;
                sql.AddParameter("@Name", "%C%");
                sql.ExecuteSP("TestSP");
                Console.WriteLine(sql.GetDebugCommand());
                if (sql.ReturnedData)
                {
                    Console.WriteLine($"CNT: {sql.RowCount}");
                    while (sql.Read())
                    {
                        Console.WriteLine(sql.Value("Name").ToString());
                    }
                }
            }
        }

        //***********************************************************************************
        static public void MSSQL_FORMAT_QUERY()
        {
            using (XwDbCommand sql = new XwDbCommand("DBCONN_MSTEST"))
            {
                sql.AddParameter("@Name", "%A%");
                sql.ExecuteTX("SELECT Name FROM Tester WHERE Name LIKE @Name");
                Console.WriteLine(sql.GetDebugCommand());
                
                while (sql.Read())
                    Console.WriteLine(sql.Value("Name").ToString());
                
                sql.AddParameter("@Name", "%A%");
                sql.ExecuteTX("SELECT TOP 1 Name FROM Tester WHERE Name LIKE @Name");

                if (sql.Read())
                    Console.WriteLine(sql.Value("Name").ToString());
            }
        }

        //***********************************************************************************
        static public void MYSQL_SP_DATASET()
        {
            using (XwDbCommand sql = new XwDbCommand("DBCONN_MYTEST"))
            {
                sql.Mode = XwDbMode.DataSet;
                sql.AddParameter("$Name", "%C%");
                sql.ExecuteSP("TestSP");
                Console.WriteLine(sql.GetDebugCommand());
                while (sql.Read())
                {
                    Console.WriteLine(sql.Value("Name").ToString());
                }
            }
        }

        //***********************************************************************************
        static public void MYSQL_SP_DATAREADER()
        {
            using (XwDbCommand sql = new XwDbCommand("DBCONN_MYTEST"))
            {
                sql.Mode = XwDbMode.DataReader;
                sql.AddParameter("$Name", "%C%");
                sql.ExecuteSP("TestSP");
                Console.WriteLine(sql.GetDebugCommand());
                while (sql.Read())
                {
                    Console.WriteLine(sql.Value("Name").ToString());
                }
            }
        }

        //***********************************************************************************
        static public void MYSQL_FORMAT_QUERY()
        {
            using (XwDbCommand sql = new XwDbCommand("DBCONN_MYTEST"))
            {
                sql.AddParameter("@Name", "%A%");
                sql.ExecuteTX("SELECT * FROM Tester WHERE Name LIKE @Name");
                Console.WriteLine(sql.GetDebugCommand());
                while (sql.Read())
                {
                    Console.WriteLine(sql.Value("Name").ToString());
                }
            }
        }

        //***********************************************************************************
        static public void MYSQL_MAKE_QUERY()
        {
            using (XwDbCommand sql = new XwDbCommand("DBCONN_MYTEST"))
            {
                sql.Make(MakeType.UPSERT, "Tester");
                sql.AddParameter("ID", 12);
                sql.AddParameter("@Name", "POIS");
                sql.ExecuteMK();
                Console.WriteLine(sql.GetDebugCommand());
            }
        }

        //***********************************************************************************
        static public void SQLITE_FORMAT_QUERY()
        {
            using (XwDbCommand sql = new XwDbCommand("DBCONN_LITETEST"))
            {
                if (!sql.TableExists("Tester"))
                {
                    sql.ExecuteTX(@"CREATE TABLE [Tester] (
                    [id] TEXT, 
                    [client] INTEGER,
                    [objid] INTEGER,
                    [objtype] TEXT,
                    [mediatype] TEXT,
                    [mediaext] TEXT,
                    PRIMARY KEY (id,objtype,mediatype,client,objid)
                    );");
                }

                /*
                sql.AddParameter("@Name", "%A%");
                sql.ExecuteTX("SELECT * FROM Tester WHERE Name LIKE @Name");
                Console.WriteLine(sql.GetTextCommand());
                while (sql.Read())
                {
                    Console.WriteLine(sql.Value("Name").ToString());
                }
                */
            }
        }

        //***********************************************************************************
        static public void MYSQL_MANY_CONNS()
        {
            for (int i = 0; i < 20; i++)
            {
                using (XwDbCommand sql = new XwDbCommand("DBCONN_MYTEST"))
                {
                    sql.Mode = XwDbMode.DataReader;
                    sql.AddParameter("$Name", "%C%");
                    sql.ExecuteSP("TestSP");
                    Console.WriteLine(sql.GetDebugCommand());
                    while (sql.Read())
                    {

                    }
                }
            }
        }

    }
}
