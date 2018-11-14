using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.Models;
using Apps.IDAL;

namespace Apps.DAL
{
    public class HomeRepository :  IHomeRepository,IDisposable
    {
         DBContainer db;
         public HomeRepository(DBContainer context)
        {
            this.db = context;
        }

        public DBContainer Context
        {
            get { return db; }
        }
        public IQueryable<SysModule> GetMenuByPersonId(string personId, string moduleId)
        {
          
                var menus =
                (
                    from m in Context.SysModule
                    join rl in Context.SysRight
                    on m.Id equals rl.ModuleId
                    join r in
                        (from r in Context.SysRole
                         from u in r.SysUser
                         where u.Id == personId
                         select r)
                    on rl.RoleId equals r.Id
                    where rl.Rightflag == true
                    where m.ParentId == moduleId
                    where m.Id != "0" && m.Enable
                    select m
                          ).Distinct().OrderBy(a => a.Sort);
                return menus;
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
