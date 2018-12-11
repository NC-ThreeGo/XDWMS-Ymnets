using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.WMS
{
    public partial class WMS_ReportModel
    {
        public static List<ReportType> GetReportType()
        {
            List<ReportType> reportTypes = new List<ReportType>();
            reportTypes.Add(new ReportType() { Type = 1, Name = "单据" });
            reportTypes.Add(new ReportType() { Type = 2, Name = "报表" });

            return reportTypes;
        }

        public static List<DataSourceType> GetDataSourceType()
        {
            List<DataSourceType> dataSourceTypes = new List<DataSourceType>();
            dataSourceTypes.Add(new DataSourceType() { Type = 1, Name = "SQL语句" });
            dataSourceTypes.Add(new DataSourceType() { Type = 2, Name = "存储过程" });

            return dataSourceTypes;
        }
    }

    public class ReportType
    {
        public int Type { get; set; }
        public string Name { get; set; }
    }

    public class DataSourceType
    {
        public int Type { get; set; }
        public string Name { get; set; }
    }
}

