using Apps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.IDAL.WC
{
    public partial interface IWC_MessageResponseRepository
    {
        bool PostData(WC_MessageResponse model);

        List<P_WC_GetResponseContent_Result> GetResponseContent(string officalAccountId,string matchKey);

        List<WC_MessageResponse> GetSubscribeResponseContent(string officalAccountId);
    }
}
