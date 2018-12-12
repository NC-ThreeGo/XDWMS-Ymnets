using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.WMS
{
    public partial class WMS_ReportParamModel
    {
        [Display(Name = "类别名称")]
        public string ReportName { get; set; }

        public static List<ParamType> GetParamType()
        {
            List<ParamType> reportTypes = new List<ParamType>();
            reportTypes.Add(new ParamType() { TypeCode = "varchar", TypeName = "varchar" });
            reportTypes.Add(new ParamType() { TypeCode = "nvarchar", TypeName = "nvarchar" });
            reportTypes.Add(new ParamType() { TypeCode = "text", TypeName = "text" });
            reportTypes.Add(new ParamType() { TypeCode = "datetime", TypeName = "datetime" });
            reportTypes.Add(new ParamType() { TypeCode = "int", TypeName = "int" });

            return reportTypes;
        }

        public static List<ParamElement> GetParamElement()
        {
            List<ParamElement> reportTypes = new List<ParamElement>();
            reportTypes.Add(new ParamElement() { ElementCode = "文本框", ElementName = "文本框" });
            reportTypes.Add(new ParamElement() { ElementCode = "文本域", ElementName = "文本域" });
            reportTypes.Add(new ParamElement() { ElementCode = "下拉框", ElementName = "下拉框" });
            reportTypes.Add(new ParamElement() { ElementCode = "时间框", ElementName = "时间框" });
            reportTypes.Add(new ParamElement() { ElementCode = "日期框", ElementName = "日期框" });

            return reportTypes;
        }
    }

    public class ParamType
    {
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
    }

    public class ParamElement
    {
        public string ElementCode { get; set; }
        public string ElementName { get; set; }
    }
}

