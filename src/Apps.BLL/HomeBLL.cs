using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Attributes;
using Apps.IBLL;
using Apps.Models;
using Apps.IDAL;
using Apps.Models.Sys;
using Apps.BLL.Core;
namespace Apps.BLL
{

    public class HomeBLL : IHomeBLL
    {
        [Dependency]
        public IHomeRepository HomeRepository { get; set; }
        public List<SysModuleModel> GetMenuByPersonId(string personId, string moduleId)
        {
            IQueryable<SysModule> queryData=HomeRepository.GetMenuByPersonId(personId, moduleId);
            return CreateModelList(ref queryData);
        }

        private List<SysModuleModel> CreateModelList(ref IQueryable<SysModule> queryData)
        {
            List<SysModuleModel> modelList = (from r in queryData
                                              select new SysModuleModel
                                              {
                                                  Id = r.Id,
                                                  Name = r.Name,
                                                  EnglishName = r.EnglishName,
                                                  ParentId = r.ParentId,
                                                  Url = r.Url,
                                                  Iconic = r.Iconic,
                                                  Sort = r.Sort,
                                                  Remark = r.Remark,
                                                  Enable = r.Enable,
                                                  CreatePerson = r.CreatePerson,
                                                  CreateTime = r.CreateTime,
                                                  IsLast = r.IsLast
                                              }).ToList();
            return modelList;
        }
    }
}
