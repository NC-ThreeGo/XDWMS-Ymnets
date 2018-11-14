using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.Models;
using Apps.IDAL;

namespace Apps.DAL
{
    public class AccountRepository : IAccountRepository,IDisposable
    {
        DBContainer db;
        public AccountRepository(DBContainer context)
        {
            this.db = context;
        }

        public DBContainer Context
        {
            get { return db; }
        }
        public SysUser Login(string username, string pwd)
        {
              SysUser user = Context.SysUser.SingleOrDefault(a => a.UserName == username && a.Password == pwd);
              return user;
        }
        public void Dispose()
        { 
            
        }
    }
}
