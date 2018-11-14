using Apps.IDAL;
using Apps.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;
namespace Apps.DAL
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        DBContainer db;
        public BaseRepository(DBContainer context)
        {
            this.db = context;
        }

        public DBContainer Context
        {
            get { return db; }
        }

        public virtual bool Create(T model)
        {
            db.Set<T>().Add(model);
            return db.SaveChanges() > 0;
        }

        public virtual bool Edit(T model)
        {
            if (db.Entry<T>(model).State == EntityState.Modified)
            {
                return db.SaveChanges() > 0;
            }
            else if (db.Entry<T>(model).State == EntityState.Detached)
            {
                try
                {
                    db.Set<T>().Attach(model);
                    db.Entry<T>(model).State = EntityState.Modified;
                }
                catch (InvalidOperationException)
                {
                    //T old = Find(model._ID);
                    //db.Entry<old>.CurrentValues.SetValues(model);
                    return false;
                }
                return db.SaveChanges() > 0;
            }
            return false;
        }

        public virtual bool Delete(T model)
        {
            db.Set<T>().Remove(model);
            return db.SaveChanges() > 0;
        }

        public virtual int Delete(params object[] keyValues)
        {
            foreach (var item in keyValues)
            {
                T model = GetById(item);
                if (model != null)
                {
                    db.Set<T>().Remove(model);
                }
            }
            return db.SaveChanges();
        }

        public virtual T GetById(params object[] keyValues)
        {
            return db.Set<T>().Find(keyValues);
        }
        public virtual T GetSingleWhere(Expression<Func<T, bool>> whereLambda)
        {
            return db.Set<T>().FirstOrDefault(whereLambda);
        }
        public virtual IQueryable<T> GetList()
        {
            return db.Set<T>();
        }

        public virtual IQueryable<T> GetList(Expression<Func<T, bool>> whereLambda)
        {
            return db.Set<T>().Where(whereLambda);
        }

        public virtual IQueryable<T> GetCacheList(Expression<Func<T, bool>> whereLambda)
        {
            return db.Set<T>().Where(whereLambda).FromCache().AsQueryable();
        }

        // m_Rep.BatchDelete(a=>a.CreateTime>DateTime.Now);
        public virtual int BatchDelete(Expression<Func<T, bool>> whereLambda)
        {
            return db.Set<T>().Where(whereLambda).Delete();
        }
        public virtual int BatchDelete()
        {
            return db.Set<T>().Delete();
        }

        public virtual int BatchUpdate(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> updateLambda)
        {
            return db.Set<T>().Where(whereLambda).Update(updateLambda);
        }
        public virtual int BatchUpdate(Expression<Func<T, T>> updateLambda)
        {
            return db.Set<T>().Update(updateLambda);
        }
        public virtual IQueryable<T> GetList<S>(int pageSize, int pageIndex, out int total
            , Expression<Func<T, bool>> whereLambda, bool isAsc, Expression<Func<T, bool>> orderByLambda)
        {
            var queryable = db.Set<T>().Where(whereLambda);
            total = queryable.Count();
            if (isAsc)
            {
                queryable = queryable.OrderBy(orderByLambda).Skip<T>(pageSize * (pageIndex - 1)).Take<T>(pageSize);
            }
            else
            {
                queryable = queryable.OrderByDescending(orderByLambda).Skip<T>(pageSize * (pageIndex - 1)).Take<T>(pageSize);
            }
            return queryable;
        }

        public virtual bool IsExist(object id)
        {
            return GetById(id) != null;
        }

        public int ExecuteSqlCommand(string sql)
        {
            return Context.Database.ExecuteSqlCommand(sql);
        }

        public int ExecuteSqlCommand(string sql, params SqlParameter[] sp)
        {
            return Context.Database.ExecuteSqlCommand(sql, sp);
        }

        /// <summary>
        /// 异步执行一条SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public Task<int> ExecuteSqlCommandAsync(string sql)
        {
            return Context.Database.ExecuteSqlCommandAsync(sql);
        }

        public DbRawSqlQuery<T> SqlQuery(string sql)
        {
            return db.Database.SqlQuery<T>(sql);
        }
        /// <summary>
        /// 查询一条语句返回结果集
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DbRawSqlQuery<T> SqlQuery(string sql, params object[] paras)
        {
            return db.Database.SqlQuery<T>(sql, paras);
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }


        //1、 Finalize只释放非托管资源；
        //2、 Dispose释放托管和非托管资源；
        //3、 重复调用Finalize和Dispose是没有问题的；
        //4、 Finalize和Dispose共享相同的资源释放策略，因此他们之间也是没有冲突的。
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {

            if (disposing)
            {
                Context.Dispose();
            }
        }
    }
}
