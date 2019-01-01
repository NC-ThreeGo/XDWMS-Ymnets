using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.WMS
{
    public partial class WMS_Product_EntryModel
    {
        public string PartCode { get; set; }

        public string PartName { get; set; }

        public string InvCode { get; set; }

        public string InvName { get; set; }
    }
}

