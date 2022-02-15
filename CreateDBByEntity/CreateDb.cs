using CreateDBByEntity.DbOpt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CreateDBByEntity
{
    class CreateDb
    {
        /// <summary>
        /// [key]特性必须写在其他特性前
        /// [Required(AllowEmptyStrings = false/true)]特性必须放在最后
        /// </summary>

        static readonly List<string> m_Models = new List<string>();

        public static void CreateDatabase(string db)
        {
            if (!SqlBasicOpt.IsDBExist(db))
            {
                string createDbStr = "Create DATABASE " + db;
                SqlBasicOpt.ExecuteNonQuery(createDbStr);
            }
        }

        /// <summary>
        /// CodeFirst 创建表
        /// </summary>
        /// <returns></returns>
        public static void CreateTable(string db)
        {
            List<string> sqlTextList = GroupSqlText(db);
            foreach (var sqlText in sqlTextList)
            {
                string[] strs = sqlText.Split(new char[2] { ' ', '(' });  // 获取表名称
                if (SqlBasicOpt.ExecuteSql(sqlText))
                {
                    Console.WriteLine(strs[4].ToString() + "创建成功\n");
                }
                else
                {
                    Console.WriteLine(strs[4].ToString() + "创建失败\n");
                }
            }
        }

        /// <summary>
        /// 添加注释
        /// </summary>
        /// <param name="dbName"></param>
        public static void AddComment(string dbName)
        {
            List<List<string>> sqlTextList = GroupCommentSql(dbName);
            for (int i = 0; i < sqlTextList.Count; i += 2)
            {
                for (int j = 0; j < sqlTextList[i].Count; j++)
                {
                    if (!SqlBasicOpt.ExecuteSql(sqlTextList[i][j]))
                    {
                        SqlBasicOpt.ExecuteSql(sqlTextList[i + 1][j]);
                    }
                }
            }
        }

        /// <summary>
        /// 获取model名称，model属性和特性
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, PropertyInfo[]> ModelProperties()
        {
            // key：类名，PropertyInfo[]：类成员和特性
            Dictionary<string, PropertyInfo[]> keyValuePairs = new Dictionary<string, PropertyInfo[]>();
            PropertyInfo[] properties;
            #region sys表
            properties = typeof(TestEntity.Test_Table).GetProperties();
            keyValuePairs.Add("Test_Table", properties);
            #endregion

            return keyValuePairs;
        }

        /// <summary>
        /// 所有表创建的SQL
        /// </summary>
        /// <returns></returns>
        private static List<string> GroupSqlText(string db)
        {
            List<string> sqlTextList = new List<string>();
            Dictionary<string, PropertyInfo[]> keyValuePairs = ModelProperties();
            // 遍历数据库表
            foreach (KeyValuePair<string, PropertyInfo[]> pair in keyValuePairs)
            {
                if (SqlBasicOpt.IsTableExists(pair.Key))
                {
                    Console.WriteLine(pair.Key + "表在数据库中已存在\n");
                    continue;
                }
                string singleSqlText = SingleSqlText(db, pair);
                sqlTextList.Add(singleSqlText);
            }
            return sqlTextList;
        }

        /// <summary>
        /// 单个表创建的SQL
        /// </summary>
        /// <param name="pair">表字段特性</param>
        /// <returns></returns>
        private static string SingleSqlText(string db, KeyValuePair<string, PropertyInfo[]> pair)
        {
            string sqlText = "USE " + db + " ";
            sqlText += "CREATE TABLE " + pair.Key + "(";
            string IdPropertyStr = "";
            string otherPropertyStr = "";
            try
            {
                foreach (var property in pair.Value)
                {
                    foreach (var attr in property.CustomAttributes)
                    {
                        // 所有特性值拼接必须按照SQL正确写法来拼接
                        // key
                        if ("KeyAttribute" == attr.AttributeType.Name)
                        {
                            IdPropertyStr += property.Name + " nvarchar(100) primary key,";
                            break;
                        }
                        // 字段类型
                        if (0 != attr.NamedArguments.Count
                            && "TypeName" == attr.NamedArguments[0].MemberName)
                        {
                            var dataType = attr.NamedArguments[0].TypedValue.Value;
                            otherPropertyStr += property.Name + " " + dataType;
                        }
                        // 是否null
                        if (0 != attr.NamedArguments.Count
                            && "AllowEmptyStrings" == attr.NamedArguments[0].MemberName)
                        {
                            var dataType = attr.NamedArguments[0].TypedValue.Value;
                            string str = dataType.ToString();
                            if ("False" == dataType.ToString())
                            {
                                otherPropertyStr += " not null";
                            }
                        }

                    }
                    if (new[] { "CreateTime", "UpdateTime" }.Contains(property.Name))
                    {
                        otherPropertyStr += " not null default current_timestamp";
                    }
                    otherPropertyStr += ",";
                }
                sqlText += IdPropertyStr + otherPropertyStr + ")";
                sqlText = sqlText.Replace(",,", ","); // 获取字段（key）时，会多一个逗号，需删除
                return sqlText;
            }
            catch (Exception)
            {
                return sqlText;
            }
        }

        /// <summary>
        /// 所有表字段的添加注释sql和更新注释sql
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        private static List<List<string>> GroupCommentSql(string dbName)
        {
            List<List<string>> sqlTextList = new List<List<string>>(); // 新增注释集合和更新注释集合
            Dictionary<string, PropertyInfo[]> keyValuePairs = ModelProperties();
            // 遍历数据库表
            foreach (KeyValuePair<string, PropertyInfo[]> pair in keyValuePairs)
            {
                List<List<string>> singleSqlText = SingleCommentSql(dbName, pair);
                sqlTextList.AddRange(singleSqlText);
            }
            return sqlTextList;
        }

        /// <summary>
        /// 表字段的添加注释sql和更新注释sql
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="pair">表字段特性</param>
        /// <returns></returns>
        private static List<List<string>> SingleCommentSql(string dbName, KeyValuePair<string, PropertyInfo[]> pair)
        {
            List<List<string>> sqlTextList = new List<List<string>>();
            List<string> addSqlTexts = new List<string>();
            List<string> updateSqlTexts = new List<string>();
            // 字段
            foreach (var property in pair.Value)
            {
                string addSqlText = string.Empty;
                string updateSqlText = string.Empty;
                // 属性
                foreach (var attr in property.CustomAttributes)
                {
                    if (0 != attr.NamedArguments.Count && "DisplayAttribute" == attr.AttributeType.Name)
                    {
                        addSqlText = "EXEC sp_addextendedproperty 'MS_Description', N'";
                        string commentText = attr.NamedArguments[0].TypedValue.Value.ToString();
                        addSqlText += commentText + "', 'SCHEMA', N'dbo', 'TABLE', N'" + pair.Key + "', 'COLUMN', N'" + property.Name + "'";
                        addSqlTexts.Add(addSqlText);
                    }
                    if (0 != attr.NamedArguments.Count && "DisplayAttribute" == attr.AttributeType.Name)
                    {
                        updateSqlText = "EXEC sp_updateextendedproperty 'MS_Description', N'";
                        string commentText = attr.NamedArguments[0].TypedValue.Value.ToString();
                        updateSqlText += commentText + "', 'SCHEMA', N'dbo', 'TABLE', N'" + pair.Key + "', 'COLUMN', N'" + property.Name + "'";
                        updateSqlTexts.Add(updateSqlText);
                        break;
                    }
                }
            }
            sqlTextList.Add(addSqlTexts);
            sqlTextList.Add(updateSqlTexts);
            return sqlTextList;
        }
    }
}
