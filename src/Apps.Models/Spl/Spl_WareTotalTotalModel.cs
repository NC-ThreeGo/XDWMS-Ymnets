//只有当启用父表时才必要复制
using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.Spl
{
    public partial class Spl_WareTotalTotalModel
    {
        public string WareDetailsName { get; set; }
        public string WarehouseName { get; set; }
    }
}

