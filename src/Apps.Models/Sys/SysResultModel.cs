using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Apps.Models.Sys
{
    public partial class GetRoleByUserIdResultModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreatePerson { get; set; }
        public string flag { get; set; }
    }
}
