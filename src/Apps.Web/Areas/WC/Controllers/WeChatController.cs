using System.Threading.Tasks;
using System.Web.Mvc;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MvcExtension;
using Senparc.Weixin.MP;
using Apps.Web.Areas.WC.Core;
using Apps.Models.WC;
using Unity.Attributes;
using Apps.IBLL.WC;

namespace Apps.Web.Areas.WC.Controllers
{
    public class WeChatController : Controller
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

        [HttpGet]
        [ActionName("Index")]
        public Task<ActionResult> Get(string signature, string timestamp, string nonce, string echostr)
        {
            return Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(Request["id"]))
                {
                    return "非法路径请求！";
                }
                WC_OfficalAccountsModel model = account_BLL.GetById(Request["id"]);
                if (CheckSignature.Check(signature, timestamp, nonce,model.Token))
                {
                    return echostr; //返回随机字符串则表示验证通过
                }
                else
                {
                    return "failed:" + signature + "," + CheckSignature.GetSignature(timestamp, nonce, model.Token) + "。" +
                        "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。";
                }
            }).ContinueWith<ActionResult>(task => Content(task.Result));
        }


        /// <summary>
        /// 最简化的处理流程
        /// </summary>
        [HttpPost]
        [ActionName("Index")]
        public Task<ActionResult> Post(PostModel postModel)
        {
            return Task.Factory.StartNew<ActionResult>(() =>
            {
               
                //没有参数，带有id才能知道是访问哪个公众号
                if (string.IsNullOrEmpty(Request["id"]))
                {
                    return new WeixinResult("非法路径请求！");
                }
                WC_OfficalAccountsModel model = account_BLL.GetById(Request["id"]);
                if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, model.Token))
                {
                    return new WeixinResult("参数错误！");
                }
                postModel.Token = model.Token;
                postModel.EncodingAESKey = model.OfficalKey; //根据自己后台的设置保持一致
                postModel.AppId = model.AppId; //根据自己后台的设置保持一致

                var messageHandler = new CustomMessageHandler(Request.InputStream, postModel, Request["id"], 10);

                messageHandler.Execute(); //执行微信处理过程

                return new FixWeixinBugWeixinResult(messageHandler);

            }).ContinueWith<ActionResult>(task => task.Result);
        }

    }
}