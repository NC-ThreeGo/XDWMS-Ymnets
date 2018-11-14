using System;
using System.Linq;
using System.Data;
using Apps.Models;
using Apps.IDAL;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;

namespace Apps.DAL
{
    public class WebpartRepository : IDisposable, IWebpartRepository
    {
        DBContainer db;
        public WebpartRepository(DBContainer context)
        {
            this.db = context;
        }

        public DBContainer Context
        {
            get { return db; }
        }
        /// <summary>
        /// 获取最新信息
        /// </summary>
        /// <param name="top"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<P_Sys_WebPart_Result> GetPartData3(int top, string userId)
        {
            
             return Context.P_WebPart_GetInfo(top, userId).AsQueryable().ToList();
        }
        /// <summary>
        /// 获取最新共享
        /// </summary>
        /// <param name="top"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        //public List<P_Mis_FileGetMyReadFile_Result> GetPartData8(int top, string userId)
        //{
        //    using (SysEntities db = SysDb.getDB())
        //    {
        //        return db.P_WebPart_GetShareFileByUserId(userId, top).AsQueryable().ToList();
        //    }
        //}
        /// <summary>
        /// 新建或修改HTML
        /// </summary>
        /// <param name="html"></param>
        public int SaveHtml(string userId,string html)
        {
            SysUserConfig ss = new SysUserConfig();
            ss.Id = "webpart";
            ss.UserId = userId;
            ss.Value = html;
            ss.Type = "webpart";
            ss.State = true;
            ss.Name = "webpart";

            string strSql = @"if(select COUNT(*) from SysUserConfig where id='webpart' and UserId=@UsrId)=0
                                begin
                                insert into SysUserConfig values('webpart', 'webpart',@Html, 'webpart', 1, @UsrId)
                                end
                                else
                                begin
                                update SysUserConfig set Value = @Html where id = 'webpart' and UserId = @UsrId
                                end";
            SqlParameter[] para = new SqlParameter[]
             {
                     new SqlParameter("@Html",html),
                     new SqlParameter("@UsrId",userId),

             };
            return Context.Database.ExecuteSqlCommand(strSql,para);

        }

        public SysUserConfig GetByIdAndUserId(string id, string userId)
        {

            return Context.SysUserConfig.SingleOrDefault(a => a.Id == id && a.UserId == userId);
        }

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
