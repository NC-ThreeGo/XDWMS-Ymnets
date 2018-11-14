using System.Threading.Tasks;
using System.Web.Mvc;

using Senparc.Weixin.MP.MvcExtension;
using Senparc.Weixin.MP;
using Apps.Web.Areas.WC.Core;
using Apps.Models.WC;
using Unity.Attributes;
using Apps.IBLL.WC;
using System;
using System.IO;
using Senparc.Weixin.QY.Entities;

namespace Apps.Web.Areas.WC.Controllers
{
    public class WeChatQYController : Controller
    {

        [Dependency]
        public IWC_OfficalAccountsBLL account_BLL { get; set; }
        //public static readonly string Token ="WeixinToken";//与微信公众账号后台的Token设置保持一致，区分大小写。
        //public static readonly string EncodingAESKey = "dEq1BjMgmkEyOvva8pQfFwX95hBLOYKpAzBJ5y9pdSK";//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        //public static readonly string AppId = "wx3c0afacbd4edc8f5";//与微信公众账号后台的AppId设置保持一致，区分大小写。

        // GET: WC/WeChat
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 微信后台验证地址（使用Get），微信企业后台应用的“修改配置”的Url填写如：http://sdk.weixin.senparc.com/qy
        /// </summary>
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(string msg_signature = "", string timestamp = "", string nonce = "", string echostr = "")
        {
            WC_OfficalAccountsModel model = account_BLL.GetById(Request["id"]);
            //return Content(echostr); //返回随机字符串则表示验证通过
            var verifyUrl = Senparc.Weixin.QY.Signature.VerifyURL(model.Token, model.OfficalKey, model.AppId, msg_signature, timestamp, nonce,
                echostr);
            if (verifyUrl != null)
            {
                return Content(verifyUrl); //返回解密后的随机字符串则表示验证通过
            }
            else
            {
                return Content("如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }




        /// <summary>
        /// 微信后台验证地址（使用Post），微信企业后台应用的“修改配置”的Url填写如：http://sdk.weixin.senparc.com/qy
        /// </summary>
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(PostModel postModel)
        {
            //没有参数，带有id才能知道是访问哪个公众号
            if (string.IsNullOrEmpty(Request["id"]))
            {
                return new WeixinResult("非法路径请求！");
            }

            var maxRecordCount = 10;

            WC_OfficalAccountsModel model = account_BLL.GetById(Request["id"]);
            postModel.CorpId = model.AppId;
            postModel.EncodingAESKey = model.OfficalKey;
            postModel.Token = model.Token;
            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var messageHandler = new QyCustomMessageHandler(Request.InputStream, postModel,model.Id, maxRecordCount);

            if (messageHandler.RequestMessage == null)
            {
                //验证不通过或接受信息有错误
            }

            try
            {
                //测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。
                messageHandler.RequestDocument.Save(Server.MapPath("~/App_Data/Qy/" + DateTime.Now.Ticks + "_Request_" + messageHandler.RequestMessage.FromUserName + ".txt"));
                //执行微信处理过程
                messageHandler.Execute();
                //测试时可开启，帮助跟踪数据
                messageHandler.ResponseDocument.Save(Server.MapPath("~/App_Data/Qy/" + DateTime.Now.Ticks + "_Response_" + messageHandler.ResponseMessage.ToUserName + ".txt"));
                messageHandler.FinalResponseDocument.Save(Server.MapPath("~/App_Data/Qy/" + DateTime.Now.Ticks + "_FinalResponse_" + messageHandler.ResponseMessage.ToUserName + ".txt"));

                //自动返回加密后结果
                return new FixWeixinBugWeixinResult(messageHandler);//为了解决官方微信5.0软件换行bug暂时添加的方法，平时用下面一个方法即可
            }
            catch (Exception ex)
            {
                using (TextWriter tw = new StreamWriter(Server.MapPath("~/App_Data/Qy_Error_" + DateTime.Now.Ticks + ".txt")))
                {
                    tw.WriteLine("ExecptionMessage:" + ex.Message);
                    tw.WriteLine(ex.Source);
                    tw.WriteLine(ex.StackTrace);
                    //tw.WriteLine("InnerExecptionMessage:" + ex.InnerException.Message);

                    if (messageHandler.FinalResponseDocument != null)
                    {
                        tw.WriteLine(messageHandler.FinalResponseDocument.ToString());
                    }
                    tw.Flush();
                    tw.Close();
                }
                return Content("");
            }
        }

    }
}