using Apps.Models;
using Apps.Models.WMS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;

namespace Apps.DAL.WMS
{
    public partial class WMS_ReportRepository
    {
        public DataSet GetDataSource(WMS_ReportModel report, List<WMS_ReportParamModel> listParam)
        {
            DataSet ds = new DataSet();

            DbCommand command = command = Context.Database.Connection.CreateCommand();

            if (report.DataSourceType == 1) //SQL语句
            {
                command.CommandType = CommandType.Text;
            }
            else //存储过程
            {
                command.CommandType = CommandType.StoredProcedure;
            }
            command.CommandText = report.DataSource;

            if (listParam != null)
            {
                foreach (var item in listParam)
                {
                    DbParameter param = command.CreateParameter();
                    param.ParameterName = item.ParamCode;
                    if (item.ParamType == "datetime" || item.ParamType == "date")
                    {
                        param.DbType = DbType.DateTime;
                        param.Value = string.IsNullOrEmpty(item.DefaultValue) ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : item.DefaultValue;
                    }
                    else if (item.ParamType == "int")
                    {
                        param.DbType = DbType.Int32;
                        param.Value = string.IsNullOrEmpty(item.DefaultValue) ? "0" : item.DefaultValue;
                    }
                    else
                    {
                        param.DbType = DbType.String;
                        param.Size = 20;
                        param.Value = string.IsNullOrEmpty(item.DefaultValue) ?  "" : item.DefaultValue;
                    }
                    command.Parameters.Add(param);
                }
            }

            using (DbDataAdapter adapter = SqlClientFactory.Instance.CreateDataAdapter())
            {
                adapter.SelectCommand = command;
                adapter.Fill(ds);
            }
            return ds;
        }
    }
}
