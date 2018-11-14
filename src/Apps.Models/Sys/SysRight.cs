using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Models.Sys
{
    public partial class SysRightUserRight
    {
        public string ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string KeyCode { get; set; }
    }
    public partial class SysRightRoleRight
    {

    }
    public partial class SysRightModuleRight
    {

    }

    public partial class UserRight
    {
        public string Ids { get; set; }
        public string UserName { get; set; }
        public string RightList { get; set; }
    }
    public partial class RoleRight
    {
        public string Ids { get; set; }
        public string RoleName { get; set; }
        public string RightList { get; set; }
    }
}
