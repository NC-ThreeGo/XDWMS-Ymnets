using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Apps.Models.Sys
{
    public partial class SysRoleModel
    {

        public override string Id { get; set; }
        [NotNullExpression]
        [Display(Name = "角色名称")]
        public override string Name { get; set; }
        
        [Display(Name = "说明")]
        public override string Description { get; set; }
        [Display(Name = "创建时间")]
        public override DateTime CreateTime { get; set; }
        [Display(Name = "创建人")]
        public override string CreatePerson { get; set; }
        [Display(Name = "拥有的用户")]
        public  string UserName { get; set; }//拥有的用户

        public  string Flag { get; set; }//用户分配角色
    }
}
