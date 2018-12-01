using Apps.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.IBLL
{
    public interface IBaseBLL<T> : IDisposable
    {
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="queryStr"></param>
        /// <returns></returns>
        List<T> GetList(ref GridPager pager, string queryStr);


        /// <summary>
        /// 根据用户获取数据
        /// </summary>
        /// <param name="pager">分页</param>
        /// <param name="userId">用户ID</param>
        /// <param name="queryStr">查询条件</param>
        /// <returns></returns>
        List<T> GetListByUserId(ref GridPager pager,string userId, string queryStr);
        /// <summary>
        /// 根据上级获取数据
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="queryStr"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        List<T> GetListByParentId(ref GridPager pager, string queryStr,object parentId);
        bool Create(ref ValidationErrors errors, T model);
        bool Delete(ref ValidationErrors errors, object id);
        bool Delete(ref ValidationErrors errors, object[] deleteCollection);
        bool Edit(ref ValidationErrors errors, T model);
        T GetById(object id);
        bool IsExists(object id);
        void SaveImportData(IEnumerable<T> personList);
        bool CheckImportData(string fileName, List<T> personList, ref ValidationErrors errors);
        bool Check(ref ValidationErrors errors, object id,int flag);
    }
}
