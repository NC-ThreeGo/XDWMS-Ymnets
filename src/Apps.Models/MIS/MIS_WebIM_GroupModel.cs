using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Apps.Models.MIS
{
    public partial class MIS_WebIM_GroupModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "*")]
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        [Required(ErrorMessage = "*")]
        public int Sort { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
