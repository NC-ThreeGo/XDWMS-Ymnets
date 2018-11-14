using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Common
{
    /// <summary>
    /// Easyui列头过滤属性
    /// </summary>
    public class DataFilterModel
    {
        public string field { set; get; }
        public string op { set; get; }
        public string value { set; get; }
    } 
}
