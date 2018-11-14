using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Models.Spl
{
    public partial class Spl_WarehouseWarrantDetailsModel
    {
        public string WarehouseName { get; set; }
        public string WareDetailsName { get; set; }

        public string WareDetailsCode { get; set;}
        public string WareDetailsUnit { get; set; }

        public string WareDetailsCategory { get; set; }

        public string WareDetailsVender { get; set; }

        public string WareDetailsBrand { get; set; }
        public string WareDetailsSize{ get; set; }
        public string oper { get; set; }
    }
}
