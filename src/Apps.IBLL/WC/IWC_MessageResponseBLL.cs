using Apps.Common;
using Apps.Models;
using Apps.Models.WC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Apps.IBLL.WC
{
    public partial interface IWC_MessageResponseBLL
    {
        bool PostData(ref ValidationErrors errors, WC_MessageResponseModel model);
        List<WC_MessageResponseModel> GetList(ref GridPager pager, Expression<Func<WC_MessageResponse, bool>> predicate, string queryStr);
        List<WC_MessageResponseModel> GetListProperty(ref GridPager pager, Expression<Func<WC_MessageResponse, bool>> predicate);
        WC_MessageResponseModel GetAutoReplyMessage(string officalAccountId, string matchKey);
        List<P_WC_GetResponseContent_Result> GetResponseContent(string officalAccountId, string matchKey);
    }
}
