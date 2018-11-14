using Apps.Models;
using Apps.Models.WC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.BLL.WC
{
    public partial class WC_GroupBLL
    {
        public List<WC_GroupModel> GetList(string currentOfficeAccountId)
        {
            IQueryable<WC_Group> queryData = null;
            queryData = m_Rep.GetList(a=>a.OfficalAccountId == currentOfficeAccountId);
            return CreateModelList(ref queryData);
        }
    }
}
