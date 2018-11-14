using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Models.Sys
{
    public class SysApiControllerModel
    {
        public string Id { get; set; }
        public string ControllerName { get; set; }

        public bool Enable { get; set; }
        public DateTime CreateTime { get; set; }
        public List<SysApiActionModel> ActionList {get;set;}
    }

    public class SysApiActionModel
    {

    }
}
