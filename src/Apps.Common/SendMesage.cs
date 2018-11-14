using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Apps.Common
{
    public class SendMesage
    {
        /// <summary>
        /// 发送手机短信
        /// </summary>
        /// <param name="number">手机号码</param>
        /// <param name="mes">手机信息</param>
        public static void Send(string number, string mes)
        {
            string url = "http://202.85.222.10:8686/SMSPortal/send?spid=dx021&spno=0058&pwd=9366311&dt='" + number + "'&msg='" + mes + "'&code=UTF-8";
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream resStream = response.GetResponseStream();
            StreamReader sr = new StreamReader(resStream, System.Text.Encoding.Default);
            sr.ReadToEnd();
            resStream.Close();
            sr.Close();
        }
    }
}
