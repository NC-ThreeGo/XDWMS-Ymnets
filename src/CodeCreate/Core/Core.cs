using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using CommonData.Data.Core;
using CommonData.Data.Core.SQLCore;

namespace CodeCreate.Core
{
    public static class Core
    {
        public static IList<string> GetSource()
        {
            DataTable tableDataSouce=SqlClientFactory.Instance.CreateDataSourceEnumerator().GetDataSources();
            IList<string> listServerName = new List<string>();
            for (int i = 0; i < tableDataSouce.Rows.Count; i++)
            {
                listServerName.Add(tableDataSouce.Rows[i]["ServerName"].ToString()+"\\"+tableDataSouce.Rows[i]["InstanceName"].ToString());
            }
            return listServerName;
        }


        public static DataTable GetDataBase(string servername, string userid, string password)
        {
            using (IDbProvider provider = new SqlProvider())
            {
                if (string.IsNullOrEmpty(userid) && string.IsNullOrEmpty(password))
                {
                    provider.ConnectionString = "server=" + servername + ";database=master;Integrated Security=true";
                }
                else
                {
                    provider.ConnectionString = "server=" + servername + ";database=master;uid=" + userid + ";pwd=" + password;
                }
                IBaseHelper baseHelper = new BaseHelper();
                string sql="select name from sysdatabases where dbid>4";
                return baseHelper.ExecuteTable(provider,sql);
            }
        }

        public static DataTable GetTable(string servername, string databasename, string userid, string password)
        {
            using (IDbProvider provider = new SqlProvider())
            {
                if (string.IsNullOrEmpty(userid) && string.IsNullOrEmpty(password))
                {
                    provider.ConnectionString = "server=" + servername + ";database="+databasename+";Integrated Security=true";
                }
                else
                {
                    provider.ConnectionString = "server=" + servername + ";database="+databasename+";uid=" + userid + ";pwd=" + password;
                }
                IBaseHelper baseHelper = new BaseHelper();
                string sql = "select name from sysobjects where type='U'";
                return baseHelper.ExecuteTable(provider, sql);
            }
        }
    }
}
