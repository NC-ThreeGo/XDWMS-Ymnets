using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apps.IBLL;
using Apps.BLL.Core;
using Unity.Attributes;
using Apps.IDAL;
using Apps.Models;
using Apps.Common;
namespace Apps.BLL
{
    public class AccountBLL:IAccountBLL
    {
        [Dependency]
        public IAccountRepository accountRepository { get; set; }
        public SysUser Login(string username, string pwd)
        {
            return accountRepository.Login(username, pwd);
         
        }
    }
}
