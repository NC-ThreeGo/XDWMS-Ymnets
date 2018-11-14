using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
using System.Collections.Generic;
namespace Apps.Models.Flow
{
    public partial class Flow_TypeModel
    {
        public List<Flow_FormModel> formList { get; set; }

    }
}
