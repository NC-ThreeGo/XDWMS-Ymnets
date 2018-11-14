//只有当启用父表时才必要复制
using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.Spl
{
    public partial class Spl_WarehouseWarrantDetailsOUTModel
    {
        public string WarehouseName { get; set; }
        public string WareDetailsName { get; set; }
        public string WareDetailsCode { get; set; }
        public string WareDetailsUnit { get; set; }

        public string WareDetailsCategory { get; set; }

        public string WareDetailsVender { get; set; }

        public string WareDetailsBrand { get; set; }
        public string WareDetailsSize { get; set; }
        public string oper { get; set; }
    }
}

