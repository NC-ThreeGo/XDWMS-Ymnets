using System;
using System.Collections.Generic;
using System.Text;

namespace Apps.Models.Sys
{
    /// <summary>
    /// 站点配置实体类
    /// </summary>
    [Serializable]
    public partial class SysConfigModel
    {
        public SysConfigModel()
        { }
        private string _webname = "";
        private string _webcompany = "";
        private string _weburl = "";
        private string _webtel = "";
        private string _webmail = "";
        private string _webcrod = "";
        private string _webtitle = "";
        private string _webkeyword = "";
        private string _webdescription = "";
        private string _webcopyright = "";
        private string _webpath = "";
        private string _webmanagepath = "";
        private int _webstatus = 1;
        private string _webclosereason = "";
        private string _webcountcode = "";



        //===============IM JOB LOG Ex Config===============
        private int _webimstatus = 1;
        private int _refreshcurrentwin = 10000;
        private int _refreshrecentcontact = 30000;
        private int _refreshnewmessage = 20000;

        private int _logstatus = 1;
        private int _exceptionstatus = 1;
        private int _globalexceptionstatus = 1;
        private string _globalexceptionurl = "/SysException/Error";
        private int _issinglelogin = 0;

        private int _taskstatus = 1;
        //===============IM JOB LOG Ex Config===============
        private string _emailstmp = "";
        private int _emailport = 25;
        private string _emailfrom = "";
        private string _emailusername = "";
        private string _emailpassword = "";
        private string _emailnickname = "";

        private string _attachpath = "";
        private string _attachextension = "";
        private int _attachsave = 1;
        private int _attachfilesize = 0;
        private int _attachimgsize = 0;
        private int _attachimgmaxheight = 0;
        private int _attachimgmaxwidth = 0;
        private int _thumbnailheight = 0;
        private int _thumbnailwidth = 0;
        private int _watermarktype = 0;
        private int _watermarkposition = 9;
        private int _watermarkimgquality = 80;
        private string _watermarkpic = "";
        private int _watermarktransparency = 10;
        private string _watermarktext = "";
        private string _watermarkfont = "";
        private int _watermarkfontsize = 12;

        //===============当前模板配置信息===============
        private string _templateskin = "blue";
        //==============系统安装时配置信息==============
        private string _sysdatabaseprefix = "Sys_";
        private string _sysencryptstring = "Apps";

        //微信配置1.中转服务器修改和新建公众号会自动配置此节 2.一般作为资源的地址，如文章

        private string _weChatApiUrl= "http://ymnets.imwork.net/WC/WeChat/?id=";
        private string _weChatSiteUrl = "http://ymnets.imwork.net";
        //微信支付V3
        private string _tenPayV3_MchId;//TenPayV3_MchId
        private string _tenPayV3_Key;//dEq1BjMgmkEyOvva8pQfFwX95hBLOYKpAzBJ5y9pdSK
        private string _tenPayV3_AppId;//wx3c0afacbd4edc8f5
        private string _tenPayV3_AppSecret;//8a6a708dd22891a02e45482d59efb0ad
        private string _tenPayV3_tenpayNotify;//TenPayV3_tenpayNotify



        /// <summary>
        /// 站点名称
        /// </summary>
        public string webname
        {
            get { return _webname; }
            set { _webname = value; }
        }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string webcompany
        {
            get { return _webcompany; }
            set { _webcompany = value; }
        }
        /// <summary>
        /// 网站域名
        /// </summary>
        public string weburl
        {
            get { return _weburl; }
            set { _weburl = value; }
        }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string webtel
        {
            get { return _webtel; }
            set { _webtel = value; }
        }

        /// <summary>
        /// 管理员邮箱
        /// </summary>
        public string webmail
        {
            get { return _webmail; }
            set { _webmail = value; }
        }
        /// <summary>
        /// 网站备案号
        /// </summary>
        public string webcrod
        {
            get { return _webcrod; }
            set { _webcrod = value; }
        }
        /// <summary>
        /// 网站首页标题
        /// </summary>
        public string webtitle
        {
            get { return _webtitle; }
            set { _webtitle = value; }
        }
        /// <summary>
        /// 页面关健词
        /// </summary>
        public string webkeyword
        {
            get { return _webkeyword; }
            set { _webkeyword = value; }
        }
        /// <summary>
        /// 页面描述
        /// </summary>
        public string webdescription
        {
            get { return _webdescription; }
            set { _webdescription = value; }
        }
        /// <summary>
        /// 网站版权信息
        /// </summary>
        public string webcopyright
        {
            get { return _webcopyright; }
            set { _webcopyright = value; }
        }
        /// <summary>
        /// 网站安装目录
        /// </summary>
        public string webpath
        {
            get { return _webpath; }
            set { _webpath = value; }
        }
        /// <summary>
        /// 网站管理目录
        /// </summary>
        public string webmanagepath
        {
            get { return _webmanagepath; }
            set { _webmanagepath = value; }
        }
        /// <summary>
        /// 是否关闭网站
        /// </summary>
        public int webstatus
        {
            get { return _webstatus; }
            set { _webstatus = value; }
        }
        /// <summary>
        /// 关闭原因描述
        /// </summary>
        public string webclosereason
        {
            get { return _webclosereason; }
            set { _webclosereason = value; }
        }
        /// <summary>
        /// 网站统计代码
        /// </summary>
        public string webcountcode
        {
            get { return _webcountcode; }
            set { _webcountcode = value; }
        }
       

  
        /// <summary>
        /// 后台管理日志
        /// </summary>
        public int logstatus
        {
            get { return _logstatus; }
            set { _logstatus = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int exceptionstatus
        {
            get { return _exceptionstatus; }
            set { _exceptionstatus = value; }
        }
        /// <summary>
        /// STMP服务器
        /// </summary>
        public string emailstmp
        {
            get { return _emailstmp; }
            set { _emailstmp = value; }
        }
        /// <summary>
        /// SMTP端口
        /// </summary>
        public int emailport
        {
            get { return _emailport; }
            set { _emailport = value; }
        }
        /// <summary>
        /// 发件人地址
        /// </summary>
        public string emailfrom
        {
            get { return _emailfrom; }
            set { _emailfrom = value; }
        }
        /// <summary>
        /// 邮箱账号
        /// </summary>
        public string emailusername
        {
            get { return _emailusername; }
            set { _emailusername = value; }
        }
        /// <summary>
        /// 邮箱密码
        /// </summary>
        public string emailpassword
        {
            get { return _emailpassword; }
            set { _emailpassword = value; }
        }
        /// <summary>
        /// 发件人昵称
        /// </summary>
        public string emailnickname
        {
            get { return _emailnickname; }
            set { _emailnickname = value; }
        }
        /// <summary>
        /// 附件上传目录
        /// </summary>
        public string attachpath
        {
            get { return _attachpath; }
            set { _attachpath = value; }
        }
        /// <summary>
        /// 附件上传类型
        /// </summary>
        public string attachextension
        {
            get { return _attachextension; }
            set { _attachextension = value; }
        }
        /// <summary>
        /// 附件保存方式
        /// </summary>
        public int attachsave
        {
            get { return _attachsave; }
            set { _attachsave = value; }
        }
        /// <summary>
        /// 文件上传大小
        /// </summary>
        public int attachfilesize
        {
            get { return _attachfilesize; }
            set { _attachfilesize = value; }
        }
        /// <summary>
        /// 图片上传大小
        /// </summary>
        public int attachimgsize
        {
            get { return _attachimgsize; }
            set { _attachimgsize = value; }
        }
        /// <summary>
        /// 图片最大高度(像素)
        /// </summary>
        public int attachimgmaxheight
        {
            get { return _attachimgmaxheight; }
            set { _attachimgmaxheight = value; }
        }
        /// <summary>
        /// 图片最大宽度(像素)
        /// </summary>
        public int attachimgmaxwidth
        {
            get { return _attachimgmaxwidth; }
            set { _attachimgmaxwidth = value; }
        }
        /// <summary>
        /// 生成缩略图高度(像素)
        /// </summary>
        public int thumbnailheight
        {
            get { return _thumbnailheight; }
            set { _thumbnailheight = value; }
        }
        /// <summary>
        /// 生成缩略图宽度(像素)
        /// </summary>
        public int thumbnailwidth
        {
            get { return _thumbnailwidth; }
            set { _thumbnailwidth = value; }
        }
        /// <summary>
        /// 图片水印类型
        /// </summary>
        public int watermarktype
        {
            get { return _watermarktype; }
            set { _watermarktype = value; }
        }
        /// <summary>
        /// 图片水印位置
        /// </summary>
        public int watermarkposition
        {
            get { return _watermarkposition; }
            set { _watermarkposition = value; }
        }
        /// <summary>
        /// 图片生成质量
        /// </summary>
        public int watermarkimgquality
        {
            get { return _watermarkimgquality; }
            set { _watermarkimgquality = value; }
        }
        /// <summary>
        /// 图片水印文件
        /// </summary>
        public string watermarkpic
        {
            get { return _watermarkpic; }
            set { _watermarkpic = value; }
        }
        /// <summary>
        /// 水印透明度
        /// </summary>
        public int watermarktransparency
        {
            get { return _watermarktransparency; }
            set { _watermarktransparency = value; }
        }
        /// <summary>
        /// 水印文字
        /// </summary>
        public string watermarktext
        {
            get { return _watermarktext; }
            set { _watermarktext = value; }
        }
        /// <summary>
        /// 文字字体
        /// </summary>
        public string watermarkfont
        {
            get { return _watermarkfont; }
            set { _watermarkfont = value; }
        }
        /// <summary>
        /// 文字大小(像素)
        /// </summary>
        public int watermarkfontsize
        {
            get { return _watermarkfontsize; }
            set { _watermarkfontsize = value; }
        }
        /// <summary>
        /// 当前模板
        /// </summary>
        public string templateskin
        {
            get { return _templateskin; }
            set { _templateskin = value; }
        }
        /// <summary>
        /// 数据库表前缀
        /// </summary>
        public string sysdatabaseprefix
        {
            get { return _sysdatabaseprefix; }
            set { _sysdatabaseprefix = value; }
        }
        /// <summary>
        /// 加密字符串
        /// </summary>
        public string sysencryptstring
        {
            get { return _sysencryptstring; }
            set { _sysencryptstring = value; }
        }

       /// <summary>
        /// 即时通讯系统
       /// </summary>
        public int webimstatus
        {
            get { return _webimstatus; }
            set { _webimstatus = value; }
        }
        /// <summary>
        /// 刷新当前聊天窗口时间
        /// </summary>
        public int refreshcurrentwin
        {
            get { return _refreshcurrentwin; }
            set { _refreshcurrentwin = value; }
        }
        /// <summary>
        /// 刷新最近联系人列表
        /// </summary>
        public int refreshrecentcontact
        {
            get { return _refreshrecentcontact; }
            set { _refreshrecentcontact = value; }
        }
        /// <summary>
        /// 刷新是否有新短信息来
        /// </summary>
        public int refreshnewmessage
         {
             get { return _refreshnewmessage; }
             set { _refreshnewmessage = value; }
        }
        /// <summary>
        /// 任务调度系统
        /// </summary>
        public int taskstatus
        {
            get { return _taskstatus; }
            set { _taskstatus = value; }
        }

        /// <summary>
        /// 是否使用全局异常处理
        /// </summary>
        public int globalexceptionstatus
        {
            get { return _globalexceptionstatus; }
            set { _globalexceptionstatus = value; }
        }
        /// <summary>
        /// 错误跳转的页面
        /// </summary>
        public string globalexceptionurl
        {
            get { return _globalexceptionurl; }
            set { _globalexceptionurl = value; }
        }
        /// <summary>
        /// 是否启用单机登录
        /// </summary>
        public int issinglelogin
         {
             get { return _issinglelogin; }
             set { _issinglelogin = value; }
        }

        public string TenPayV3_MchId
        {
            get
            {
                return _tenPayV3_MchId;
            }

            set
            {
                _tenPayV3_MchId = value;
            }
        }

        public string TenPayV3_Key
        {
            get
            {
                return _tenPayV3_Key;
            }

            set
            {
                _tenPayV3_Key = value;
            }
        }

        public string TenPayV3_AppId
        {
            get
            {
                return _tenPayV3_AppId;
            }

            set
            {
                _tenPayV3_AppId = value;
            }
        }

        public string TenPayV3_AppSecret
        {
            get
            {
                return _tenPayV3_AppSecret;
            }

            set
            {
                _tenPayV3_AppSecret = value;
            }
        }

        public string TenPayV3_tenpayNotify
        {
            get
            {
                return _tenPayV3_tenpayNotify;
            }

            set
            {
                _tenPayV3_tenpayNotify = value;
            }
        }


        public string WeChatApiUrl
        {
            get
            {
                return _weChatApiUrl;
            }

            set
            {
                _weChatApiUrl = value;
            }
        }
        public string WeChatQYApiUrl { get; set; }
        
        public string WeChatSiteUrl
        {
            get
            {
                return _weChatSiteUrl;
            }

            set
            {
                _weChatSiteUrl = value;
            }
        }
    }
}
