using Apps.Models;
using Apps.Models.WC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.IBLL.WC
{
    public partial interface IWC_GroupBLL
    {
        List<WC_GroupModel> GetList(string currentOfficeAccountId);
    }
}
