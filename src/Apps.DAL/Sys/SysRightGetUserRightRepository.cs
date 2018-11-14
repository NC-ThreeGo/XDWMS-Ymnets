using System;
using Apps.Models;
using System.Collections.Generic;
using System.Linq;
using Apps.IDAL;
using Apps.IDAL.Sys;

namespace Apps.DAL.Sys
{
    public class SysRightGetUserRightRepository : ISysRightGetUserRightRepository,IDisposable
    {
         DBContainer db;
         public SysRightGetUserRightRepository(DBContainer context)
        {
            this.db = context;
        }

        public DBContainer Context
        {
            get { return db; }
        }
        public List<P_Sys_GetRightByUser_Result> GetList(string userId)
        {
            var userRight = Context.P_Sys_GetRightByUser(userId).ToList();
                return userRight;
        }

        public void Dispose()
        { }
    }
 
}
