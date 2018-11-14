using Apps.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Apps.Web.Areas.Flow.Controllers
{
    public class UploadController : Controller
    {


        //[HttpPost]
        //public JsonResult SingleFileUpload(string isWater,string isThumbnail,string path, HttpPostedFileBase file)
        //{

        //    UpLoad upFiles = new UpLoad();
        //    string msg = upFiles.fileSaveAs(file, isThumbnail, isWater, path);

        //    var fileName = file.FileName;
        //    var filePath = Server.MapPath(string.Format("~/{0}", "File"));
        //    file.SaveAs(Path.Combine(filePath, fileName));
        //    return Json("1");
        //}

        //private void SingleFile(HttpContext context)
        //{
        //    string _refilepath = ContextRequest.GetQueryString("ReFilePath"); //取得返回的对象名称
        //    string _upfilepath = ContextRequest.GetQueryString("UpFilePath"); //取得上传的对象名称
        //    string _delfile = ContextRequest.GetString(_refilepath);
        //    HttpPostedFile _upfile = context.Request.Files[_upfilepath];
        //    bool _iswater = false; //默认不打水印
        //    bool _isthumbnail = false; //默认不生成缩略图
        //    bool _isimage = false;

        //    if (ContextRequest.GetQueryString("IsWater") == "1")
        //        _iswater = true;
        //    if (ContextRequest.GetQueryString("IsThumbnail") == "1")
        //        _isthumbnail = true;
        //    if (ContextRequest.GetQueryString("IsImage") == "1")
        //        _isimage = true;

        //    if (_upfile == null)
        //    {
        //        context.Response.Write("{\"msg\": 0, \"msgbox\": \"请选择要上传文件！\"}");
        //        return;
        //    }
        //    UpLoad upFiles = new UpLoad();
        //    string msg = upFiles.fileSaveAs(_upfile, _isthumbnail, _iswater, _isimage);
        //    //删除已存在的旧文件
        //    //Utils.DeleteUpFile(_delfile);
        //    //返回成功信息
        //    context.Response.Write(msg);
        //    context.Response.End();
        //}

    }
}