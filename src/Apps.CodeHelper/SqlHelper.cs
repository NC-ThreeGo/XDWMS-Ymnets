using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
namespace Apps.CodeHelper
{
    public class SqlHelper
    {


        public static string ExecteSqlGetValue(string sql,string connection)
        {

            SqlConnection con = new SqlConnection(connection);
            con.Open();
            SqlCommand cmd = new SqlCommand(sql, con);

            string value = Convert.ToString(cmd.ExecuteScalar());
            con.Close();
            return value;

        }

        /// <summary>
        /// 获取局域网内的所有数据库服务器名称
        /// </summary>
        /// <returns>服务器名称数组</returns>
        public static List<string> GetSqlServerNames()
        {
            DataTable dataSources = SqlClientFactory.Instance.CreateDataSourceEnumerator().GetDataSources();

            DataColumn column = dataSources.Columns["InstanceName"];
            DataColumn column2 = dataSources.Columns["ServerName"];

            DataRowCollection rows = dataSources.Rows;
            List<string> Serverlist = new List<string>();
            string array = string.Empty;
            for (int i = 0; i < rows.Count; i++)
            {
                string str2 = rows[i][column2] as string;
                string str = rows[i][column] as string;
                if (((str == null) || (str.Length == 0)) || ("MSSQLSERVER" == str))
                {
                    array = str2;
                }
                else
                {
                    array = str2 + @"/" + str;
                }

                Serverlist.Add(array);
            }

            Serverlist.Sort();

            return Serverlist;
        }

        public static Dictionary<string, string> GetAllTableName(string connection)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string cmdStirng = "select * from sysobjects where xtype='u' or xtype='v' order by name";
            SqlConnection connect = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand(cmdStirng, connect);
            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                    IDataReader dr = cmd.ExecuteReader();
                    dic.Clear();
                    while (dr.Read())
                    {
                        dic.Add(dr["name"].ToString(), dr["name"].ToString());
                    }
                    dr.Close();
                }

            }
            catch
            {
                //MessageBox.Show(e.Message);
            }
            finally
            {
                if (connect != null && connect.State == ConnectionState.Open)
                {
                    connect.Dispose();
                }
            }
            return dic;
        }

        /// <summary>
        /// 查询sql中的非系统库
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<string> databaseList(string connection)
        {
            List<string> getCataList = new List<string>();
            string cmdStirng = "select name from sys.databases where database_id > 4";
            SqlConnection connect = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand(cmdStirng, connect);
            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                    IDataReader dr = cmd.ExecuteReader();
                    getCataList.Clear();
                    while (dr.Read())
                    {
                        getCataList.Add(dr["name"].ToString());
                    }
                    dr.Close();
                }

            }
            catch
            {
                //MessageBox.Show(e.Message);
            }
            finally
            {
                if (connect != null && connect.State == ConnectionState.Open)
                {
                    connect.Dispose();
                }
            }
            return getCataList;
        }

        /// <summary>
        /// 获取列名
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetTables(string connection)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            SqlConnection objConnetion = new SqlConnection(connection);
            try
            {
                if (objConnetion.State == ConnectionState.Closed)
                {
                    objConnetion.Open();
                    DataTable objTable = objConnetion.GetSchema("Tables");
                    foreach (DataRow row in objTable.Rows)
                    {
                        dic.Add(row[2].ToString(), row[2].ToString());
                    }
                }
            }
            catch
            {

            }
            finally
            {
                if (objConnetion != null && objConnetion.State == ConnectionState.Closed)
                {
                    objConnetion.Dispose();
                }

            }
            return dic;
        }

        /// <summary>
        /// 获取字段
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static List<string> GetColumnField(string connection, string TableName)
        {
            List<string> Columnlist = new List<string>();
            SqlConnection objConnetion = new SqlConnection(connection);
            try
            {
                if (objConnetion.State == ConnectionState.Closed)
                {
                    objConnetion.Open();
                }

                SqlCommand cmd = new SqlCommand("Select Name FROM SysColumns Where id=Object_Id('" + TableName + "')", objConnetion);
                SqlDataReader objReader = cmd.ExecuteReader();

                while (objReader.Read())
                {
                    Columnlist.Add(objReader[0].ToString());

                }
            }
            catch
            {

            }
            objConnetion.Close();
            return Columnlist;
        }

        public static List<CompleteField> GetColumnCompleteField(string connection, string TableName)
        {
            List<CompleteField> list = new List<CompleteField>();
            SqlConnection objConnetion = new SqlConnection(connection);
            try
            {
                if (objConnetion.State == ConnectionState.Closed)
                {
                    objConnetion.Open();
                }
                string sqlStr="SELECT "
                +"  c.name," 
                +"  c.user_type_id," 
                +"  c.max_length," 
                +"  c.is_nullable," 
                +"  remark = ex.value" 
                +"  FROM  " 
                +"     sys.columns c  " 
                +"  LEFT OUTER JOIN  " 
                +"     sys.extended_properties ex  " 
                +"  ON  " 
                +"     ex.major_id = c.object_id " 
                +"     AND ex.minor_id = c.column_id  " 
                +"     AND ex.name = 'MS_Description'  " 
                +"  WHERE  " 
                +"     OBJECTPROPERTY(c.object_id, 'IsMsShipped')=0  " 
                +"      AND OBJECT_NAME(c.object_id) = '" + TableName + "' " 
                +"  ORDER  " 
                +"     BY OBJECT_NAME(c.object_id), c.column_id";
                //Select Name,xtype,length,isnullable FROM SysColumns Where id=Object_Id('" + TableName + "')
                SqlCommand cmd = new SqlCommand(sqlStr, objConnetion);
                SqlDataReader objReader = cmd.ExecuteReader();

                while (objReader.Read())
                {
                    list.Add(new CompleteField() { name = objReader[0].ToString(), xType = objReader[1].ToString(), length = objReader[2].ToString(), isNullAble = objReader[3].ToString() ,remark = (objReader[4]==null?"":objReader[4].ToString())});
                }
            }
            catch
            {

            }
            objConnetion.Close();
            return list;
        }

        public static string GetType(string xtype)
        {
           switch(xtype)
           {
             case "34": return "image";
             case "35": return "string";
             case "36": return "uniqueidentifier";
             case "48": return "tinyint";
             case "52": return "smallint";
             case "56": return "int";
             case "58": return "smalldatetime";
             case "59": return "real";
             case "60": return "money";
             case "61": return "DateTime";
             case "62": return "float";
             case "98": return "sql_variant";
             case "99": return "ntext";
             case "104": return "bool";
             case "106": return "decimal";
             case "108": return " numeric";
             case "122": return "smallmoney";
             case "127": return "bigint";
             case "165": return "varbinary";
             case "167": return "string";
             case "173": return "binary";
             case "175": return "char";
             case "189": return "timestamp";
             case "231": return "string";
             case "239": return "nchar";
                   default: return "";
           }

        }
    }

    public class CompleteField
    {
       public string name { set; get; }
       public string xType { set; get; }
       public string length { set; get; }
       public string isNullAble { set; get; }
       public string remark { set; get; }
    }
}
