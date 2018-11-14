using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Apps.IDAL
{
    public interface IBaseRepository<T> : IDisposable
    {
        bool Create(T model);
        bool Edit(T model);
        bool Delete(T model);
        /// <summary>
        /// 按主键删除
        /// </summary>
        /// <param name="keyValues"></param>
        int Delete(params object[] keyValues);
        T GetById(params object[] keyValues);

        T GetSingleWhere(Expression<Func<T, bool>> whereLambda);
        /// <summary>
        /// 获得所有数据
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetList();
        /// <summary>
        /// 根据表达式获取数据
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        IQueryable<T> GetList(Expression<Func<T, bool>> whereLambda);
        /// <summary>
        /// 带二级缓存的查询
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        IQueryable<T> GetCacheList(Expression<Func<T, bool>> whereLambda);
        IQueryable<T> GetList<S>(int pageSize, int pageIndex, out int total
            , Expression<Func<T, bool>> whereLambda, bool isAsc, Expression<Func<T, bool>> orderByLambda);
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="whereLambda">删除条件</param>
        /// <returns></returns>
        int BatchDelete(Expression<Func<T, bool>> whereLambda);
        /// <summary>
        /// 批量删除整张表数据
        /// </summary>
        /// <returns></returns>
        int BatchDelete();
        /// <summary>
        /// 批量
        /// </summary>
        /// <param name="whereLambda">表达式</param>
        /// <param name="updateLambda">m_Rep.BatchUpdate(a=>a.Age==36,a=>new SysSample() { Age = 37});</param>
        /// <returns></returns>
        int BatchUpdate(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> updateLambda);
        /// <summary>
        /// 整表更新
        /// </summary>
        /// <param name="updateLambda">m_Rep.BatchUpdate(a=>a.Age==36,a=>new SysSample() { Age = 37});</param>
        /// <returns></returns>
        int BatchUpdate(Expression<Func<T, T>> updateLambda);
        bool IsExist(object id);
        #region 执行数据库语句
        /// <summary>
        /// 执行一条SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        int ExecuteSqlCommand(string sql);
        int ExecuteSqlCommand(string sql, params SqlParameter[] sp);
        Task<int> ExecuteSqlCommandAsync(string sql);
        DbRawSqlQuery<T> SqlQuery(string sql);
        DbRawSqlQuery<T> SqlQuery(string sql, params object[] paras);
        #endregion
        int SaveChanges();
    }
}
