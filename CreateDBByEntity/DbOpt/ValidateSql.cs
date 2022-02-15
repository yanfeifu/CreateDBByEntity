using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace CreateDBByEntity.DbOpt
{
    public class ValidateSql
    {
        /// <summary>
        /// 验证SQL语句（只验证不执行SQL）
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static bool CheckedSql(string sqlText, SqlConnection conn)
        {
            bool bResult;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SET PARSEONLY ON";
            cmd.ExecuteNonQuery();
            try
            {
                cmd.CommandText = sqlText;
                cmd.ExecuteNonQuery();
                bResult = true;
            }
            catch (Exception ex)
            {
                bResult = false;
            }
            finally
            {
                cmd.CommandText = "SET PARSEONLY OFF";
                cmd.ExecuteNonQuery();
            }

            return bResult;
        }
    }
}
