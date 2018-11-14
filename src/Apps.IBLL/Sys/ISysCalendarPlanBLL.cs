using System;
using Apps.Common;
using System.Collections.Generic;
using Apps.Models.Sys;
namespace Apps.IBLL.Sys
{
	public partial interface ISysCalendarPlanBLL
	{
        List<SysCalendarPlanModel> GetList(ref GridPager pager, DateTime start,DateTime end,string userId);

    }
}
