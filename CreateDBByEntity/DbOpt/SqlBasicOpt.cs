using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CreateDBByEntity.DbOpt
{
    public class SqlBasicOpt
    {
        private static readonly object sqlLock = new object();

        private static SqlConnection _connection;

        /// <summary>
        /// 连接属性
        /// </summary>
        public static SqlConnection Connection
        {
            get
            {
                try
                {
                    if (_connection == null)
                    {
                        // 从app.config读取
                        //string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStr"].ToString();
                        string connStr = @"Server=10.190.44.138;Database=;User Id=sa;Password=wrsjhhe";
                        _connection = new SqlConnection(connStr);
                        _connection.Open();
                    }
                    else if (_connection.State == ConnectionState.Broken)
                    {
                        _connection.Close();
                        _connection.Open();
                    }
                    else if (_connection.State == ConnectionState.Closed)
                    {
                        _connection.Open();
                    }
                    return _connection;
                }
                catch (Exception)
                {
                    if (_connection != null)
                    {
                        _connection.Close();
                        _connection.Dispose();
                    }
                    return null;
                }
            }
        }

        /// <summary>
        /// 重置连接
        /// </summary>
        public static void ResetConnection()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }

        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="str">执行字符串</param>
        /// <returns></returns>
        public static DataSet GetDataSet(string str)
        {
            lock (sqlLock)
            {
                try
                {
                    SqlDataAdapter sda = new SqlDataAdapter(str, Connection);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    return ds;
                }
                catch (Exception)
                {
                    ResetConnection();
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取表格
        /// </summary>
        /// <param name="str">执行字符串</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string str)
        {
            return GetDataSet(str).Tables[0];
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ExecuteSql(string str)
        {
            lock (sqlLock)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Connection;
                SqlTransaction trans = Connection.BeginTransaction();
                try
                {
                    if (cmd.Connection != null && cmd.Connection.State == ConnectionState.Open)
                    {
                        cmd.Transaction = trans;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = str;
                        cmd.ExecuteNonQuery();
                        trans.Commit();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (SqlException)
                {
                    trans.Rollback();//事物回滚
                    return false;
                }
            }
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="str"></param>
        public static void ExecuteNonQuery(string str)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = str;
                cmd.ExecuteNonQuery();
            }
            catch
            {
                ResetConnection();
            }
        }

        /// <summary>
        /// 判断数据库是否存在
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static bool IsDBExist(string db)
        {
            string createDbStr = " select * from master.dbo.sysdatabases where name " + "= '" + db + "'";
            DataTable dt = GetDataTable(createDbStr);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool IsTableExists(string tableName)
        {
            string sqlText = "if object_id(N'";
            sqlText += tableName;
            sqlText += "',N'U') is not null SELECT 'ok' ELSE SELECT ''";
            if (null == Connection || !ValidateSql.CheckedSql(sqlText, Connection))
            {
                Console.WriteLine("连接数据库失败");
                return false;
            }
            try
            {
                SqlCommand command = Connection.CreateCommand();
                command.CommandText = sqlText;
                string result = command.ExecuteScalar().ToString();
                return string.IsNullOrEmpty(result) ? false : true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 增删改
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns></returns>
        //public static void ExecuteSql(string sqlText)
        //{
        //    //SqlConnection conn = ConnectDB();
        //    //if (null == conn || !ValidateSql.CheckedSql(sqlText, conn))
        //    //{
        //    //    Console.WriteLine("连接数据库失败");
        //    //    return false;
        //    //}
        //    try
        //    {
        //        SqlCommand command = new SqlCommand();
        //        command.Connection = Connection;
        //        command.CommandType = CommandType.Text;
        //        command.CommandText = sqlText;
        //        SqlDataReader reader = command.ExecuteReader();
        //        reader.Close();
        //    }
        //    catch (Exception)
        //    {

        //    }
        //}
    }
}
