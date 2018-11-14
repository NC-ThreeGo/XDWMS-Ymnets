using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.Flow
{
    public partial class Flow_FormContentModel
    {
        public string UserName { get; set; }
        
        public string DepName { get; set; }
        public string PosName { get; set; }
        public string TrueName { get; set;}
        public  string CurrentStep{ get; set; }
        public  string StepCheckId { get; set; }
        public  string Action { get; set; }

        public int CurrentState { get; set; }
        //是否是外部链接
        public bool IsExternal { get; set; }
        //外部的链接
        public string ExternalURL { get; set; }
        //外部的表，每加一个需要在这里加一个
        public Flow_ExternalModel externalModel { get; set; }

    }
}
