using Apps.Common;
using Apps.IBLL;
using Apps.Models;
using Apps.WebApi.Core;
using Unity.Attributes;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Security;

namespace Apps.WebApi.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AccountController : ApiController
    {

        [Dependency]
        public IAccountBLL accountBLL { get; set; }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="passsword">密码</param>
        /// <returns></returns>
        [HttpGet]
        public object Login(string userName, string password)
        {
            SysUser user = accountBLL.Login(userName, ValueConvert.MD5(password));
            if (user == null)
            {
                return Json(JsonHandler.CreateMessage(0, "用户名或密码错误"));
            }
            else if (!Convert.ToBoolean(user.State))//被禁用
            {
                return Json(JsonHandler.CreateMessage(0, "账户被系统禁用"));
            }
            FormsAuthenticationTicket token = new FormsAuthenticationTicket(0, userName, DateTime.Now,
                            DateTime.Now.AddHours(1), true, string.Format("{0}&{1}", userName, password),
                            FormsAuthentication.FormsCookiePath);
            //返回登录结果、用户信息、用户验证票据信息
            var Token = FormsAuthentication.Encrypt(token) ;
            //将身份信息保存在session中，验证当前请求是否是有效请求
            HttpContext.Current.Session[userName] = Token;
            return Json(JsonHandler.CreateMessage(1, Token));
        }

    }
}
