
using System;
using Apps.Common;
using System.Collections.Generic;
using Apps.Models.Spl;
using System.Linq;
using Apps.Models.Sys;

namespace Apps.IBLL.Spl
{
	public partial interface ISpl_WarehouseBLL
	{
        List<Spl_WarehouseModel> GetList(ref GridPager pager, string queryStr, string sysUserId);
        IQueryable<GetRoleByUserIdResultModel> GetRoleByUserId(ref GridPager pager, string roleId);
        bool UpdateSysRoleSpl_Warehouse(string userId, string[] roleIds);

        string GetRefSysRole(string userId);
    }
}
