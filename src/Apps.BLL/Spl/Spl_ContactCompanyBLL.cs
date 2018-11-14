using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using Apps.Models.Spl;

namespace Apps.BLL.Spl
{
    public  partial class Spl_ContactCompanyBLL
    {

        public override List<Spl_ContactCompanyModel> CreateModelList(ref IQueryable<Spl_ContactCompany> queryData)
        {

            List<Spl_ContactCompanyModel> modelList = (from r in queryData
                                              select new Spl_ContactCompanyModel
                                              {
                                                  Id = r.Id,
                                                  Code = r.Code,
                                                  Name = r.Name,
                                                  Phone = r.Phone,
                                                  ContactCompanyCategoryId = r.ContactCompanyCategoryId,
                                                  Remark = r.Remark,
                                                  Enable = r.Enable,
                                                  CreateTime = r.CreateTime,
                                                  ContactCompanyCategoryName = r.Spl_ContactCompanyCategory.Name,
                                              }).ToList();
            return modelList;
        }
    }
 }

