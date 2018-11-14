using Apps.BLL.Core;
using Apps.Common;
using Apps.Locale;
using Apps.Models;
using Apps.Models.Enum;
using Apps.Models.WC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Apps.BLL.WC
{
   public partial class WC_MessageResponseBLL
    {
        public bool PostData(ref ValidationErrors errors, WC_MessageResponseModel model)
        {
            try
            {

                WC_MessageResponse entity = new WC_MessageResponse();

                if (IsExists(model.Id))
                {
                    entity = m_Rep.GetById(model.Id);
                }

                entity.Id = model.Id;
                entity.OfficalAccountId = model.OfficalAccountId;
                entity.MessageRule = model.MessageRule;
                entity.Category = model.Category;
                entity.MatchKey = model.MatchKey;
                entity.TextContent = model.TextContent;
                entity.ImgTextContext = model.ImgTextContext;
                entity.ImgTextUrl = model.ImgTextUrl;
                entity.ImgTextLink = model.ImgTextLink;
                entity.MeidaUrl = model.MeidaUrl;
                entity.Enable = model.Enable;
                entity.IsDefault = model.IsDefault;
                entity.Remark = model.Remark;
                entity.CreateTime = model.CreateTime;
                entity.CreateBy = model.CreateBy;
                entity.Sort = model.Sort;
                entity.ModifyTime = model.ModifyTime;
                entity.ModifyBy = model.ModifyBy;
                if (m_Rep.PostData(entity))
                {
                    return true;
                }
                else
                {
                    errors.Add(Resource.NoDataChange);
                    return false;
                }

            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }
        }

        public List<WC_MessageResponseModel> GetList(ref GridPager pager, Expression<Func<WC_MessageResponse, bool>> predicate, string queryStr)
        {

            IQueryable<WC_MessageResponse> queryData = null;
            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryData = m_Rep.GetList( a => a.MatchKey.Contains(queryStr));
            }
            else
            {
                queryData = m_Rep.GetList();
            }
            queryData = queryData.Where(predicate.Compile()).AsQueryable();
            pager.totalRows = queryData.Count();
            //排序
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }

        public List<WC_MessageResponseModel> GetListProperty(ref GridPager pager, Expression<Func<WC_MessageResponse, bool>> predicate)
        {

            IQueryable<WC_MessageResponse> queryData = null;
            queryData = m_Rep.GetList().Where(predicate.Compile()).AsQueryable();

            IQueryable<WC_MessageResponseModel> keys = (from r in queryData group r by new { r.MatchKey,r.Category } into g
                                                        select new WC_MessageResponseModel() {
                                                            MatchKey = g.Key.MatchKey,
                                                            Category = (Int32)g.Key.Category,
                                                            CreateTime = (DateTime)g.Max(p=>p.CreateTime)
                                                        });
            pager.totalRows = keys.Count();

            keys = LinqHelper.SortingAndPaging(keys, pager.sort, pager.order, pager.page, pager.rows);

            return keys.ToList();
        }


        /// <summary>
        /// 获取消息自动回复的信息
        /// </summary>
        /// <param name="officalAccountId">请求的公众号</param>
        /// <param name="matchKey">关键字</param>
        /// <returns></returns>
        public WC_MessageResponseModel GetAutoReplyMessage(string officalAccountId, string matchKey)
        {
            IQueryable<WC_MessageResponse> queryable = m_Rep.GetList();
            //从数据库获取一条记录来回复,完全匹配
            WC_MessageResponse entity = queryable.Where(a => a.OfficalAccountId == officalAccountId 
            && a.MessageRule != (int)WeChatRequestRuleEnum.Default 
            && a.MessageRule != (int)WeChatRequestRuleEnum.Subscriber 
            && a.MessageRule != (int)WeChatRequestRuleEnum.Location 
            && a.Category == (int)WeChatReplyCategory.Equal
            && a.MatchKey == matchKey
            ).FirstOrDefault();
            //如果没有符合要求的回复，那么使用包含匹配
            if (entity == null)
            {
               entity = queryable.Where(a => a.OfficalAccountId == officalAccountId
               && a.MessageRule != (int)WeChatRequestRuleEnum.Default
               && a.MessageRule != (int)WeChatRequestRuleEnum.Subscriber
               && a.MessageRule != (int)WeChatRequestRuleEnum.Location
               && a.Category == (int)WeChatReplyCategory.Contain
               && a.MatchKey.Contains(matchKey)
               ).FirstOrDefault();
            }

            //如果都没有，使用默认回复
            if (entity == null)
            {
                entity = queryable.Where(a => a.OfficalAccountId == officalAccountId 
                && a.MessageRule == (int)WeChatRequestRuleEnum.Default 
                && a.IsDefault).FirstOrDefault();
            }
            
            if (entity != null)
            {
                return this.GetById(entity.Id);
            }
            else
            {
                return null;
            }
           
        }

       public List<P_WC_GetResponseContent_Result> GetResponseContent(string officalAccountId, string matchKey)
       {
            return m_Rep.GetResponseContent(officalAccountId,matchKey);
       }

    }
}
