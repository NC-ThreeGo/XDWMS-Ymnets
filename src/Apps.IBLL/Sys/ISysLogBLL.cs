using System;
using System.Collections.Generic;
using Apps.Common;
using Apps.Models;
using Apps.Models.Sys;
namespace Apps.IBLL.Sys
{
   public partial interface ISysLogBLL
    {
       List<SysLogModel> GetListByUser(ref GridPager pager, string queryStr, string userId);
    }
}
