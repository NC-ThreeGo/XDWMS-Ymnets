using Apps.BLL.Core;
using Apps.Common;
using Apps.Models;
using Apps.Models.WC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.BLL.WC
{
    public partial class WC_UserBLL
    {
        public override List<WC_UserModel> CreateModelList(ref IQueryable<WC_User> queryData)
        {

            List<WC_UserModel> modelList = (from r in queryData
                                            select new WC_UserModel
                                            {
                                                Id = r.Id,
                                                OpenId = r.OpenId,
                                                NickName = r.NickName,
                                                Sex = r.Sex,
                                                Language = r.Language,
                                                City = r.City,
                                                Province = r.Province,
                                                Country = r.Country,
                                                HeadImgUrl = r.HeadImgUrl,
                                                SubscribeTime = r.SubscribeTime,
                                                UnionId = r.UnionId,
                                                Remark = r.Remark,
                                                GroupId = r.WC_Group.Name,
                                                TagidList = r.TagidList,
                                                Subscribe = r.Subscribe,
                                                OfficalAccountId = r.OfficalAccountId,

                                            }).ToList();

            return modelList;
        }

    }
}
