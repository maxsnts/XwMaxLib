using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using XwMaxLib.Diagnostics;
using XwMaxLib.Extensions;

//TODO: Multiple Resultsets

namespace XwMaxLib.Data
{
    public enum XwDbMode
    {
        DataSet,
        DataReader
    }

    public enum XwDbProvider
    {
        MSSQL,
        MYSQL,
        SQLITE
    }

    public enum MakeType
    {
        REPLACE,
        UPSERT,
        INSERT,
        UPDATE
        //DELETE
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************
    public class XwDbCommand : IDisposable
    {
        private DbConnection _Connection;
        private DbCommand _Command;
        private DataSet _DataSet;
        private DbDataReader _DataReader;
        private XwDbProvider _Provider;
        private Profiler _Profiler;
        private int CurrentDataSetIndex = -1;
        private string StringLiteralPrefix = string.Empty;
        private MakeType Maketype;
        private string MakeTable = string.Empty;
        private string MakeWhere = string.Empty;
        private string DebugCommand = string.Empty;
        
        //Use this to set the mode only once for all connections
        public static XwDbMode DefaultMode = XwDbMode.DataReader;
        public static int DefaultCommandTimeout = 30;

        //for this connection only
        public XwDbMode Mode = DefaultMode;

        public bool CloseByKilling = false;
        
        public int CommandTimeout
        {
            set { _Command.CommandTimeout = value; }
            get { return _Command.CommandTimeout; }
        }
        
        //********************************************************************************
        public XwDbCommand(string connectionString, string providerName, Profiler profiler = null)
        {
            CreateConnection(connectionString, providerName, profiler);
        }
        
        //********************************************************************************
        public XwDbCommand(string connectionStringConfigName, Profiler profiler = null)
        {
            CreateConnection(ConfigurationManager.ConnectionStrings[connectionStringConfigName].ConnectionString,
                ConfigurationManager.ConnectionStrings[connectionStringConfigName].ProviderName, profiler);
        }

        //********************************************************************************
        public XwDbCommand(string connectionStringConfigName, string host, string db, Profiler profiler = null)
        {
            string conn = ConfigurationManager.ConnectionStrings[connectionStringConfigName].ConnectionString;
            conn = conn.Replace("{HOST}", host).Replace("{SERVER}", host);
            conn = conn.Replace("{DB}", db).Replace("{DATABASE}", db);
            CreateConnection(conn, ConfigurationManager.ConnectionStrings[connectionStringConfigName].ProviderName, profiler);
        }

        //********************************************************************************
        public XwDbCommand(ConnectionStringSettings connection, Profiler profiler = null)
        {
            CreateConnection(connection.ConnectionString, connection.ProviderName, profiler);
        }

        //********************************************************************************
        public XwDbCommand(DbConnection connection, Profiler profiler = null)
        {
            _Connection = connection;
            _Profiler = profiler;
        }

        //********************************************************************************
        public void Make(MakeType type, string table, string where = "")
        {
            Maketype = type;
            MakeTable = table;
            MakeWhere = where;
        }

        //*************************************************************************************************
        private string MakeQuery()
        {
            string query = string.Empty;

            switch (Maketype)
            {
                case MakeType.REPLACE:
                    {
                        if (_Provider != XwDbProvider.MYSQL)
                            throw new Exception("REPLACE is implemented only for MySQL");

                        query = $"REPLACE INTO {MakeTable} SET ";
                        foreach (DbParameter parameter in _Command.Parameters)
                        {
                            string param = Regex.Replace(parameter.ParameterName, @"^\W", "", RegexOptions.IgnoreCase);
                            query += $"{param}=@{param},";
                        }

                        if (query.EndsWith(","))
                            query = query.Remove(query.Length - 1, 1);
                    }
                    break;
                case MakeType.UPSERT:
                    {
                        if (_Provider != XwDbProvider.MYSQL && _Provider != XwDbProvider.SQLITE)
                            throw new Exception("UPSERT is implemented only for MySQL and SQLite");

                        query = $"INSERT INTO {MakeTable} (";

                        foreach (DbParameter parameter in _Command.Parameters)
                        {
                            string param = Regex.Replace(parameter.ParameterName, @"^\W", "", RegexOptions.IgnoreCase);
                            query += $"{param},";
                        }

                        if (query.EndsWith(","))
                            query = query.Remove(query.Length - 1, 1);

                        query += ") VALUES (";

                        foreach (DbParameter parameter in _Command.Parameters)
                        {
                            string param = Regex.Replace(parameter.ParameterName, @"^\W", "", RegexOptions.IgnoreCase);
                            query += $"@{param},";
                        }

                        if (query.EndsWith(","))
                            query = query.Remove(query.Length - 1, 1);

                        query += ") ";

                        if (_Provider == XwDbProvider.MYSQL)
                        {
                            query += " ON DUPLICATE KEY UPDATE ";

                            foreach (DbParameter parameter in _Command.Parameters)
                            {
                                string param = Regex.Replace(parameter.ParameterName, @"^\W", "", RegexOptions.IgnoreCase);
                                query += $"{param}=@{param},";
                            }

                            if (query.EndsWith(","))
                                query = query.Remove(query.Length - 1, 1);
                        }

                        if (_Provider == XwDbProvider.SQLITE)
                        {
                            query.Replace("INSERT INTO", "INSERT OR REPLACE INTO");
                        }
                    }
                    break;
                case MakeType.INSERT:
                    {
                        query = $"INSERT INTO {MakeTable} (";

                        foreach (DbParameter parameter in _Command.Parameters)
                        {
                            string param = Regex.Replace(parameter.ParameterName, @"^\W", "", RegexOptions.IgnoreCase);
                            query += $"{param},";
                        }

                        if (query.EndsWith(","))
                            query = query.Remove(query.Length - 1, 1);

                        query += ") VALUES (";

                        foreach (DbParameter parameter in _Command.Parameters)
                        {
                            string param = Regex.Replace(parameter.ParameterName, @"^\W", "", RegexOptions.IgnoreCase);
                            query += $"@{param},";
                        }

                        if (query.EndsWith(","))
                            query = query.Remove(query.Length - 1, 1);

                        query += ")";
                    }
                    break;
                case MakeType.UPDATE:
                    {
                        query = $"UPDATE {MakeTable} SET ";

                        foreach (DbParameter parameter in _Command.Parameters)
                        {
                            string param = Regex.Replace(parameter.ParameterName, @"^\W", "", RegexOptions.IgnoreCase);
                            query += $"{param}=@{param},";
                        }

                        if (query.EndsWith(","))
                            query = query.Remove(query.Length - 1, 1);

                        query += $" WHERE {MakeWhere}";
                    }
                    break;
                default:
                    throw new Exception("Unknown MakeType");
            }
            
            return query;
        }

        //********************************************************************************
        public void ResetCommand()
        {
            MakeTable = string.Empty;
            
            if (_Command != null)
            {
                _Command.Dispose();
                _Command = null;
            }
            
            switch (_Provider)
            {
                case XwDbProvider.MSSQL:
                    {
                        _Command = new SqlCommand();
                    }
                    break;
                case XwDbProvider.MYSQL:
                    {
                        _Command = new MySqlCommand();
                    }
                    break;
                case XwDbProvider.SQLITE:
                    {
                        _Command = new SQLiteCommand();
                    }
                    break;
            }
        }
        
        //********************************************************************************
        public void ChangePassword(string password)
        {
            switch (_Provider)
            {
                case XwDbProvider.SQLITE:
                    {
                        Open();
                        ((SQLiteConnection)_Connection).ChangePassword(password);
                    }
                    break;
                default:
                    throw new Exception("Not implemented");
            }
        }

        //********************************************************************************
        private void CreateConnection(string connection, string providerName, Profiler profiler)
        {
            _Profiler = profiler;
            switch (providerName)
            {
                case "Data.SqlClient":
                    {
                        _Provider = XwDbProvider.MSSQL;
                        _Connection = new SqlConnection(connection);
                        StringLiteralPrefix = "N";
                    }
                    break;
                case "Data.MySqlClient":
                    {
                        _Provider = XwDbProvider.MYSQL;
                        _Connection = new MySqlConnection(connection);
                    }
                    break;
                case "Data.SQLite":
                    {
                        _Provider = XwDbProvider.SQLITE;
                        _Connection = new SQLiteConnection(connection);
                    }
                    break;
                default:
                    throw new Exception($"Connection String ProviderName [{providerName}] not supported");
            }

            ResetCommand();
            _Command.CommandTimeout = DefaultCommandTimeout;
        }

        //********************************************************************************
        private void Open()
        {
            if (_Connection != null)
            {
                switch (_Connection.State)
                {
                    case ConnectionState.Broken:
                        {
                            _Connection.Close();
                            _Connection.Open();
                        }
                        break;
                    case ConnectionState.Closed:
                        _Connection.Open();
                        break;
                    case ConnectionState.Connecting:
                    case ConnectionState.Executing:
                    case ConnectionState.Fetching:
                        throw new Exception("Connection busy");
                }
            }
        }

        #region CLEANUP
        //********************************************************************************
        ~XwDbCommand()
        {
            Dispose(false);
        }

        //********************************************************************************
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //********************************************************************************
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    Close(CloseByKilling);
                }
            }
        }
        
        //********************************************************************************
        public bool IsDisposed { get; protected set; }
        public void Close(bool kill = false)
        {
            if (_DataSet != null)
            {
                _DataSet.Dispose();
                _DataSet = null;
            }

            if (_DataReader != null)
            {
                _DataReader.Close();
                _DataReader = null;
            }

            if (kill)
            {
                try
                {
                    switch (_Provider)
                    {
                        case XwDbProvider.MYSQL:
                            ExecuteTX("kill connection_id();");
                            break;
                        default:
                            throw new Exception("Kill connection nos implemented for this provider");
                    }
                }
                catch
                { }
            }

            if (_Command != null)
            {
                _Command.Parameters.Clear();
                _Command.Dispose();
                _Command = null;
            }

            if (_Connection != null)
            {
                _Connection.Close();
                _Connection.Dispose();
                _Connection = null;
            }

            IsDisposed = true;
        }

        //********************************************************************************
        public void Kill()
        {
            Close(true);
        }
        #endregion

        //********************************************************************************
        public void AddParameter(string name, bool value, bool allowNull = false, bool nullIf = false)
        {
            if (allowNull && value == nullIf)
                AddParameter(name, null, typeof(bool));
            else
                AddParameter(name, value, typeof(bool));
        }

        //********************************************************************************
        public void AddParameter(string name, DateTime value, bool allowNull = true, string nullIf = "0001-01-01T00:00:00")
        {
            if (allowNull && value == DateTime.Parse(nullIf))
                AddParameter(name, null, typeof(System.DateTime));
            else
                AddParameter(name, value, typeof(System.DateTime));
        }

        //********************************************************************************
        public void AddParameter(string name, decimal value, bool allowNull = false, decimal nullIf = 0)
        {
            if (allowNull && value == nullIf)
                AddParameter(name, null, typeof(decimal));
            else
                AddParameter(name, value, typeof(decimal));
        }

        //********************************************************************************
        public void AddParameter(string name, double value, bool allowNull = false, double nullIf = 0.0)
        {
            if (allowNull && value == nullIf)
                AddParameter(name, null, typeof(double));
            else
                AddParameter(name, value, typeof(double));
        }

        //********************************************************************************
        public void AddParameter(string name, short value, bool allowNull = false, short nullIf = 0)
        {
            if (allowNull && value == nullIf)
                AddParameter(name, null, typeof(short));
            else
                AddParameter(name, value, typeof(short));
        }

        //********************************************************************************
        public void AddParameter(string name, int value, bool allowNull = false, int nullIf = 0)
        {
            if (allowNull && value == nullIf)
                AddParameter(name, null, typeof(int));
            else
                AddParameter(name, value, typeof(int));
        }

        //********************************************************************************
        public void AddParameter(string name, long value, bool allowNull = false, long nullIf = 0)
        {
            if (allowNull && value == nullIf)
                AddParameter(name, null, typeof(long));
            else
                AddParameter(name, value, typeof(long));
        }


        //********************************************************************************
        public void AddParameter(string name, Guid value, bool allowNull = false, string nullIf = "00000000-0000-0000-0000-000000000000")
        {
            if (allowNull && value == Guid.Parse(nullIf))
                AddParameter(name, null, typeof(System.Guid));
            else
                AddParameter(name, value, typeof(System.Guid));
        }

        //********************************************************************************
        public void AddParameter(string name, string value, bool allowNull = false, string nullIf = "")
        {
            if (allowNull && value == nullIf)
                AddParameter(name, null, typeof(string));
            else
                AddParameter(name, value, typeof(string));
        }

        //********************************************************************************
        public void AddParameter(string name, byte[] value)
        {
            AddParameter(name, value, typeof(byte));
        }

        //********************************************************************************
        private void AddParameter(string name, object value, Type type)
        {
            XwDbValue val = new XwDbValue(value, type);
            switch (_Provider)
            {
                case XwDbProvider.MSSQL:
                    {
                        if (_Command == null)
                            _Command = new SqlCommand();

                        if (Regex.IsMatch(name, @"^\w"))
                            name = "@" + name;

                        SqlParameter param = new SqlParameter(name, val.GetSqlType());
                        param.Value = val.ToDbValue();
                        _Command.Parameters.Add(param);
                    }
                    break;
                case XwDbProvider.MYSQL:
                    {
                        if (_Command == null)
                            _Command = new MySqlCommand();
                        MySqlParameter param = new MySqlParameter(name, val.GetMySqlType());
                        param.Value = val.ToDbValue();
                        _Command.Parameters.Add(param);
                    }
                    break;
                case XwDbProvider.SQLITE:
                    {
                        if (_Command == null)
                            _Command = new SQLiteCommand();

                        if (Regex.IsMatch(name, @"^\w"))
                            name = "@" + name;

                        SQLiteParameter param = new SQLiteParameter(name, val.GetSQLiteType());
                        param.Value = val.ToDbValue();
                        _Command.Parameters.Add(param);
                    }
                    break;
            }
        }
        
        //********************************************************************************
        private string FormatValue(object value, bool autoQuote = true)
        {
            if (value == null || value is DBNull)
                return "NULL";
            else if (value is bool)
                return (bool)value ? "1" : "0";
            else if (value is byte)
                return ((byte)value).ToString();
            else if (value is char)
                return $"{StringLiteralPrefix}'{value.ToString().Replace("'", "''")}'";
            else if (value is DateTime)
            {
                if (((DateTime)value) > DateTime.MinValue)
                {
                    if (autoQuote)
                        return string.Format("'{0}'", ((DateTime)value).ToString("s"));
                    else
                        return string.Format("{0}", ((DateTime)value).ToString("s"));
                }
                else
                    return "NULL";
            }
            else if (value is decimal)
                return ((decimal)value).ToString("G");
            else if (value is double)
                return ((double)value).ToString("G");
            else if (value is short)
                return ((short)value).ToString("G");
            else if (value is int)
                return ((int)value).ToString("G");
            else if (value is long)
                return ((long)value).ToString("G");
            else if (value is sbyte)
                return ((sbyte)value).ToString("G");
            else if (value is float)
                return ((float)value).ToString("G");
            else if (value is string)
            {
                if (autoQuote)
                    return $"{StringLiteralPrefix}'{value.ToString().Replace("'", "''")}'";
                else
                    return $"{value.ToString().Replace("'", "''")}";
            }
            else if (value is ushort)
                return ((ushort)value).ToString("G");
            else if (value is uint)
                return ((uint)value).ToString("G");
            else if (value is ulong)
                return ((ulong)value).ToString("G");
            else if (value is Guid)
            {
                if (((Guid)value) != Guid.Empty)
                {
                    if (autoQuote)
                        return string.Format("'{0}'", ((Guid)value).ToString("D"));
                    else
                        return string.Format("{0}", ((Guid)value).ToString("D"));
                }
                else
                    return "NULL";
            }
            else if (value is byte[])
            {
                StringBuilder oHexa = new StringBuilder("0x");
                byte[] oBytes = (byte[])(value);
                if (oBytes.Length == 0)
                    oHexa.Append("0");
                else
                    for (int b = 0; b < oBytes.Length; b++)
                        oHexa.AppendFormat("{0:X2}", oBytes[b]);
                return oHexa.ToString();
            }
            else
            {
                if (autoQuote)
                    return string.Format(StringLiteralPrefix + "'{0}'", value.ToString().Replace("'", "''"));
                else
                    return string.Format(StringLiteralPrefix + "{0}", value.ToString().Replace("'", "''"));
            }
        }

        //********************************************************************************
        public void ExecuteSP(string command, bool resetAfterExec = true)
        {
            _Command.CommandType = CommandType.StoredProcedure;
            if (_Profiler != null)
                _Profiler.Start($"DB:{command}");
            Execute(command, resetAfterExec);
            if (_Profiler != null)
                _Profiler.Stop($"DB:{command}");
        }

        //********************************************************************************
        public void ExecuteTX(string command, bool resetAfterExec = true)
        {
            _Command.CommandType = CommandType.Text;
            string commandBlock = command.Left(50).Replace("\\r", "").Replace("\\n", " ");
            if (_Profiler != null)
                _Profiler.Start($"DB:{commandBlock}");
            Execute(command, resetAfterExec);
            if (_Profiler != null)
                _Profiler.Stop($"DB:{commandBlock}");
        }

        //********************************************************************************
        public void ExecuteMK(bool resetAfterExec = true)
        {
            _Command.CommandType = CommandType.Text;
            string command = MakeQuery();
            string commandBlock = command.Left(50).Replace("\\r", "").Replace("\\n", " ");
            if (_Profiler != null)
                _Profiler.Start($"DB:{commandBlock}");
            Execute(command, resetAfterExec);
            if (_Profiler != null)
                _Profiler.Stop($"DB:{commandBlock}");
        }

        //********************************************************************************
        private void Execute(string command, bool resetAfterExec)
        {
            try
            {
                //??? before open...
                //I dont know why, so i commented it, let see
                //_Command.CommandText = command;
                //_Command.Connection = _Connection;

                Open();

                if (_DataSet != null)
                {
                    _DataSet.Dispose();
                    _DataSet = null;
                }

                if (_DataReader != null)
                {
                    _DataReader.Close();
                    _DataReader = null;
                }

                Stopwatch watch = new Stopwatch();
                watch.Start();

                _Command.Connection = _Connection;
                _Command.CommandText = command;

                DebugCommand = GetTextCommand();

                for (int r = 1; r <= 3; r++)
                {
                    try
                    {
                        switch (Mode)
                        {
                            case XwDbMode.DataSet:
                            {
                                switch (_Provider)
                                {
                                    case XwDbProvider.MSSQL:
                                    {
                                        SqlDataAdapter adapter = new SqlDataAdapter((SqlCommand)_Command);
                                        _DataSet = new DataSet();
                                        adapter.Fill(_DataSet);
                                    }
                                    break;
                                    case XwDbProvider.MYSQL:
                                    {
                                        MySqlDataAdapter adapter = new MySqlDataAdapter((MySqlCommand)_Command);
                                        _DataSet = new DataSet();
                                        adapter.Fill(_DataSet);
                                    }
                                    break;
                                    case XwDbProvider.SQLITE:
                                    {
                                        SQLiteDataAdapter adapter = new SQLiteDataAdapter((SQLiteCommand)_Command);
                                        _DataSet = new DataSet();
                                        adapter.Fill(_DataSet);
                                    }
                                    break;
                                }
                                CurrentDataSetIndex = -1;
                            }
                            break;
                            case XwDbMode.DataReader:
                            {
                                //switch just in case of future differences... maybe i will remove it if 
                                switch (_Provider)
                                {
                                    case XwDbProvider.MSSQL:
                                    {
                                        _DataReader = _Command.ExecuteReader();
                                    }
                                    break;
                                    case XwDbProvider.MYSQL:
                                    {
                                        _DataReader = _Command.ExecuteReader();
                                    }
                                    break;
                                    case XwDbProvider.SQLITE:
                                    {
                                        _DataReader = _Command.ExecuteReader();
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                        
                        break; //out of retry
                    }
                    catch(Exception ex)
                    {
                        if (r < 3)
                        {
                            if (ex.Message.Contains("was deadlocked") ||
                                ex.Message.Contains("client was unable to establish a connection") ||
                                ex.Message.Contains("timeout period elapsed") ||
                                ex.Message.Contains("wait timeout exceeded"))
                            {
                                Thread.Sleep(r * 1000);
                                continue;
                            }
                        }
                        throw ex;
                    }
                }

                watch.Stop();

                if (Debugger.IsAttached)
                {
                    string trace = "---------------Execution---------------\r\n";
                    trace += GetTextCommand();
                    trace += "\r\n----------------Message----------------\r\n";
                    trace += $"Time:{watch.Elapsed.ToString()}\r\n";
                    trace += "---------------------------------------";
                    Trace.WriteLine(trace);
                }
            }
            catch (Exception ex)
            {
                XwDbException dbex = new XwDbException(ex);
                dbex.Command = GetTextCommand();
                throw dbex;
            }
            finally
            {
                if (resetAfterExec)
                    ResetCommand();
            }
        }

        //********************************************************************************
        public string GetDebugCommand()
        {
            if (DebugCommand == string.Empty)
                return "DebugCommand is Empty, nothing was executed yet";

            return DebugCommand;
        }

        //********************************************************************************
        private string GetTextCommand()
        {
            string command = string.Empty;
            if (_Command.CommandType == CommandType.StoredProcedure)
            {
                switch (_Provider)
                {
                    case XwDbProvider.MSSQL:
                        {
                            command += "EXEC ";
                            command += ((_Command.CommandText == "") ? "[EXECUTE NOT CALLED]" : _Command.CommandText) + " ";
                            for (int i = 0; i < _Command.Parameters.Count; i++)
                            {
                                if (i > 0) command += ",";
                                command += $"{_Command.Parameters[i].ParameterName}={FormatValue(_Command.Parameters[i].Value)}";
                            }
                        }
                        break;
                    case XwDbProvider.MYSQL:
                        {
                            command += "CALL ";
                            command += ((_Command.CommandText == "") ? "[EXECUTE NOT CALLED]" : _Command.CommandText) + "(";
                            for (int i = 0; i < _Command.Parameters.Count; i++)
                            {
                                if (i > 0) command += ",";
                                command += $"{FormatValue(_Command.Parameters[i].Value)}";
                            }
                            command += ");";
                        }
                        break;
                    default:
                        command = $"COMMAND NOT GENERATED FOR PROVIDER: {_Provider}";
                        break;
                }
            }
            else
            {
                command = _Command.CommandText;
                foreach (DbParameter parameter in _Command.Parameters)
                {
                    command = command.Replace(parameter.ParameterName, FormatValue(parameter.Value));
                }
            }

            return command;
        }

        //********************************************************************************
        public bool ReturnedData
        {
            get
            {
                switch (Mode)
                {
                    case XwDbMode.DataSet:
                        {
                            if (_DataSet != null)
                                if (_DataSet.Tables.Count > 0)
                                    if (_DataSet.Tables[0].Rows.Count > 0)
                                        return true;
                        }
                        break;
                    case XwDbMode.DataReader:
                        {
                            return _DataReader.HasRows;
                        }
                }
                return false;
            }
        }

        //********************************************************************************
        public int RowCount
        {
            get
            {
                switch (Mode)
                {
                    case XwDbMode.DataSet:
                        {
                            if (_DataSet != null)
                                if (_DataSet.Tables.Count > 0)
                                    return _DataSet.Tables[0].Rows.Count;
                        }
                        break;
                    case XwDbMode.DataReader:
                        {
                            using (var dataTable = new DataTable())
                            {
                                Mode = XwDbMode.DataSet; //change modes
                                dataTable.Load(_DataReader);
                                _DataSet = new DataSet();
                                _DataSet.Tables.Add(dataTable);
                                return dataTable.Rows.Count;
                            }
                        }
                }
                return 0;
            }
        }
        
        //********************************************************************************
        public bool Read()
        {
            switch (Mode)
            {
                case XwDbMode.DataSet:
                    {
                        if (_DataSet == null)
                            throw new Exception("DataSet is NULL");

                        if (CurrentDataSetIndex + 1 < _DataSet.Tables[0].Rows.Count)
                        {
                            CurrentDataSetIndex++;
                            return true;
                        }
                        else
                            return false;
                    }
                case XwDbMode.DataReader:
                    {
                        if (_DataReader == null)
                            throw new Exception("DataReader is NULL");
                        return _DataReader.Read();
                    }
            }

            return false;
        }

        //********************************************************************************
        public DataSet GetDataSet()
        {
            return _DataSet;
        }

        //********************************************************************************
        public XwDbValue Value(string name)
        {
            return Value(GetOrdinal(name));
        }
        //********************************************************************************
        public XwDbValue Value(int index)
        {
            switch (Mode)
            {
                case XwDbMode.DataSet:
                    {
                        DataRow row = _DataSet.Tables[0].Rows[CurrentDataSetIndex];
                        return new XwDbValue(row[index]);
                    }
                case XwDbMode.DataReader:
                    {
                        return new XwDbValue(_DataReader.GetValue(index));
                    }
            }

            throw new Exception($"Unable to get Value for index [{index}]");
        }

        //********************************************************************************
        public int GetOrdinal(string name)
        {
            int index = -1;
            switch (Mode)
            {
                case XwDbMode.DataSet:
                    {
                        index = _DataSet.Tables[0].Columns.IndexOf(name);
                    }
                    break;
                case XwDbMode.DataReader:
                    {
                        index = _DataReader.GetOrdinal(name);
                    }
                    break;
            }

            if (index == -1)
                throw new Exception($"Column [{name}] not found");

            return index;
        }

        //********************************************************************************
        public bool TableExists(string tableName)
        {
            string command = string.Empty;
            switch (_Provider)
            {
                case XwDbProvider.SQLITE:
                    {
                        command = "SELECT name FROM sqlite_master WHERE type='table' AND name=@TABLENAME";
                    }
                    break;
                default:
                    throw new Exception("TableExists is not implemented for this provider");
            }

            ResetCommand();
            AddParameter("@TABLENAME", tableName);
            ExecuteTX(command, true);
            if (Read())
                return true;
            return false;
        }

        //********************************************************************************
        //Oh boy this is bad. Have to find another way to do this.
        public bool ColumnExists(string tableName, string columnName)
        {
            string command = string.Empty;
            switch (_Provider)
            {
                case XwDbProvider.SQLITE:
                    {
                        command = $"SELECT {columnName} FROM {tableName} LIMIT 0";
                    }
                    break;
                default:
                    throw new Exception("TableExists is not implemented for this provider");
            }

            ResetCommand();
            try
            {
                ExecuteTX(command, true);
                return true;
            }
            catch
            { }
            
            return false;
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************
    public class XwDbValue
    {
        private object _Value;
        private Type _Type;

        //********************************************************************************
        public XwDbValue(object value, Type type = null)
        {
            _Value = value;
            _Type = type ?? value.GetType();
        }

        //********************************************************************************
        public new Type GetType()
        {
            return _Type;
        }

        //********************************************************************************
        public SqlDbType GetSqlType()
        {
            if (_Type == typeof(Guid))
                return SqlDbType.UniqueIdentifier;
            else if (_Type == typeof(string))
                return SqlDbType.NVarChar;
            else if (_Type == typeof(double))
                return SqlDbType.Decimal;
            else if (_Type == typeof(int))
                return SqlDbType.Int;
            else if (_Type == typeof(long))
                return SqlDbType.BigInt;
            else if (_Type == typeof(DateTime))
                return SqlDbType.DateTime;
            else if (_Type == typeof(bool))
                return SqlDbType.Bit;
            else if (_Type == typeof(byte))
                return SqlDbType.Binary;

            throw new Exception($"Unknown data type {_Type}");
        }

        //********************************************************************************
        public MySqlDbType GetMySqlType()
        {
            if (_Type == typeof(Guid))
                return MySqlDbType.Guid;
            else if (_Type == typeof(string))
                return MySqlDbType.VarChar;
            else if (_Type == typeof(double))
                return MySqlDbType.Decimal;
            else if (_Type == typeof(int))
                return MySqlDbType.Int32;
            else if (_Type == typeof(long))
                return MySqlDbType.Int64;
            else if (_Type == typeof(DateTime))
                return MySqlDbType.DateTime;
            else if (_Type == typeof(bool))
                return MySqlDbType.Bit;
            else if (_Type == typeof(byte))
                return MySqlDbType.Binary;

            throw new Exception($"Unknown data type {_Type}");
        }

        //********************************************************************************
        public DbType GetSQLiteType()
        {
            if (_Type == typeof(Guid))
                return DbType.Guid;
            else if (_Type == typeof(string))
                return DbType.String;
            else if (_Type == typeof(double))
                return DbType.Double;
            else if (_Type == typeof(int))
                return DbType.Int32;
            else if (_Type == typeof(long))
                return DbType.Int64;
            else if (_Type == typeof(DateTime))
                return DbType.DateTime;
            else if (_Type == typeof(bool))
                return DbType.Boolean;
            else if (_Type == typeof(byte))
                return DbType.Binary;

            throw new Exception($"Unknown data type {_Type}");
        }


        //********************************************************************************
        public bool IsNULL()
        {
            if (_Value == null)
                return true;
            if (_Value == DBNull.Value)
                return true;
            return false;
        }

        //********************************************************************************
        public object ToDbValue()
        {
            if (_Value == null)
                return DBNull.Value;

            /*
            if (_Type == typeof(System.Guid) && (Guid)_Value == Guid.Empty)
                return DBNull.Value;

            if (_Type == typeof(System.DateTime) && (DateTime)_Value == DateTime.MinValue)
                return DBNull.Value;
            */

            return _Value;
        }

        //********************************************************************************
        public bool ToBoolean(bool defaultValue = false, bool convertNULLs = true)
        {
            object obj = ToDbValue();
            if (convertNULLs == true && obj == DBNull.Value)
                return defaultValue;
            return Convert.ToBoolean(obj);
        }
        
        //********************************************************************************
        public decimal ToDecimal(decimal defaultValue = 0, bool convertNULLs = true)
        {
            object obj = ToDbValue();
            if (convertNULLs == true && obj == DBNull.Value)
                return defaultValue;
            return Convert.ToDecimal(obj);
        }

        //********************************************************************************
        public double ToDouble(double defaultValue = 0.0, bool convertNULLs = true)
        {
            object obj = ToDbValue();
            if (convertNULLs == true && obj == DBNull.Value)
                return defaultValue;
            return Convert.ToDouble(obj);
        }

        //********************************************************************************
        public short ToInt16(short defaultValue = 0, bool convertNULLs = true)
        {
            object obj = ToDbValue();
            if (convertNULLs == true && obj == DBNull.Value)
                return defaultValue;
            return Convert.ToInt16(obj);
        }

        //********************************************************************************
        public int ToInt32(int defaultValue = 0, bool convertNULLs = true)
        {
            object obj = ToDbValue();
            if (convertNULLs == true && obj == DBNull.Value)
                return defaultValue;
            return Convert.ToInt32(obj);
        }

        //********************************************************************************
        public long ToInt64(long defaultValue = 0, bool convertNULLs = true)
        {
            object obj = ToDbValue();
            if (convertNULLs == true && obj == DBNull.Value)
                return defaultValue;
            return Convert.ToInt64(obj);
        }

        //********************************************************************************
        public Guid ToGuid(bool convertNULLs = true)
        {
            object obj = ToDbValue();
            if (convertNULLs == true && obj == DBNull.Value)
                return Guid.Empty;
            return new Guid(obj.ToString());
        }

        //********************************************************************************
        public DateTime ToDateTime(bool convertNULLs = true)
        {
            object obj = ToDbValue();
            if (convertNULLs == true && obj == DBNull.Value)
                return DateTime.MinValue;
            return Convert.ToDateTime(obj);
        }

        //********************************************************************************
        public string ToString(string defaultValue = "", bool convertNULLs = true)
        {
            object obj = ToDbValue();
            if (obj == DBNull.Value)
            {
                if (convertNULLs == true)
                    return defaultValue;
                else
                    return null;
            }
            return obj.ToString();
        }

        //********************************************************************************
        public byte[] ToByteArray()
        {
            object obj = ToDbValue();
            if (obj == DBNull.Value)
                return null;
            return (byte[])obj;
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class XwDbException : DbException
    {
        public string Command = string.Empty;

        //********************************************************************************
        public XwDbException(Exception innerException) : base(innerException.Message, innerException)
        {
            if (Debugger.IsAttached)
            {
                string trace = "---------------Execution---------------\r\n";
                trace += Command;
                trace += "---------------------------------------\r\n";
                Trace.WriteLine(trace);
            }
        }

        //********************************************************************************
        public XwDbException(string message) : base(message)
        {
            if (Debugger.IsAttached)
            {
                string trace = "---------------Execution---------------\r\n";
                trace += Command;
                trace += "---------------------------------------\r\n";
                Trace.WriteLine(trace);
            }
        }

        //********************************************************************************
        public override string ToString()
        {
            string output = string.Empty;
            output += "\r\n---------------------------------------------------------------- \r\n";
            output += base.ToString();
            output += "\r\n----------------------------- Command -------------------------- \r\n";
            output += Command;
            output += "\r\n---------------------------------------------------------------- \r\n";
            return output;
        }

    }
}
