using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Apps.Common;
using Unity.Attributes;
using Apps.IBLL;
using Apps.Models.Sys;
using Apps.Web.Core;
using Apps.IBLL.Sys;

namespace Apps.Web.Controllers
{
    public class SysHelperController : Controller
    {
        //
        // GET: /SysHelper/
        [Dependency]
        public ISysStructBLL structBLL { get; set; }
        [Dependency]
        public ISysUserBLL sysUserBLL { get; set; }
        [Dependency]
        public ISysPositionBLL sysPosBLL { get; set; }

        public ActionResult Index()
        {
            return View();
        }
        #region 上传图片
        //上传图片
        public ActionResult UpLoadImg(string id = "1")
        {
            ViewBag.Dif = id;
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Upload(HttpPostedFileBase fileData)
        {
            if (fileData != null)
            {
                try
                {
                    // 文件上传后的保存路径
                    string filePath = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    string fileName = Path.GetFileName(fileData.FileName);// 原始文件名称
                    string fileExtension = Path.GetExtension(fileName); // 文件扩展名
                    string saveName = ResultHelper.NewId + fileExtension; // 保存文件名称

                    fileData.SaveAs(filePath + saveName);

                    return Json(new { Success = true, FileName = fileName, SaveName = saveName, FilePath = "/Uploads/" + saveName }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {

                return Json(new { Success = false, Message = "请选择要上传的文件！" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        //导出时候读取报表
        public ActionResult ReportControl()
        {
            return View();
        }
        //万能查询
        public ActionResult Query()
        {
            return View();
        }



        #region 获取人员选择表
        public ActionResult UserLookUp()
        {
            if (Session["Account"] != null)
            {
                CommonHelper commonHelper = new CommonHelper();
                ViewBag.StructTree = commonHelper.GetStructTree(true);
                return View();
            }
            else
            {
                return View();
            }
        }
        public JsonResult GetUserListByDep(GridPager pager, string depId, string queryStr)
        {
            if (Session["Account"] != null)
            {
                if (string.IsNullOrWhiteSpace(depId))
                    return Json(0);
                var userList = sysUserBLL.GetUserByDepId(ref pager, depId, queryStr);

                var jsonData = new
                {
                    total = pager.totalRows,
                    rows = (
                        from r in userList
                        select new SysUserModel()
                        {
                            Id = r.Id,
                            UserName = r.UserName,
                            TrueName = r.TrueName,
                            DepName = structBLL.GetById(r.DepId).Name,
                            PosName = sysPosBLL.GetById(r.PosId).Name,
                            Flag = "<input type='checkbox' id='cb_" + r.Id + "' onclick='SetValue(\"" + r.Id + "\",\"" + r.TrueName + "\")'>",
                        }
                    ).ToArray()
                };
                return Json(jsonData);
            }
            else
            {
                return Json(0);
            }
        }
        #endregion

        #region 获取人员选择表单选
        public ActionResult UserSingleLookUp()
        {
            if (Session["Account"] != null)
            {
                CommonHelper commonHelper = new CommonHelper();
                ViewBag.StructTree = commonHelper.GetStructTree(true);
                return View();
            }
            else
            {
                return View();
            }
        }


        public JsonResult GetUserSingleListByDep(GridPager pager, string depId, string queryStr)
        {
            if (Session["Account"] != null)
            {
                if (string.IsNullOrWhiteSpace(depId))
                    return Json(0);
                var userList = sysUserBLL.GetUserByDepId(ref pager, depId, queryStr);

                var jsonData = new
                {
                    total = pager.totalRows,
                    rows = (
                        from r in userList
                        select new SysUserModel()
                        {
                            Id = r.Id,
                            UserName = r.UserName,
                            TrueName = r.TrueName,
                            DepName = structBLL.GetById(r.DepId).Name,
                            PosName = sysPosBLL.GetById(r.PosId).Name,
                            Flag = "<input type='radio' name='selUserGroup' id='cb_" + r.Id + "' onclick='SetValue(\"" + r.Id + "\",\"" + r.TrueName + "\")'>",
                        }
                    ).ToArray()
                };
                return Json(jsonData);
            }
            else
            {
                return Json(0);
            }
        }
        #endregion


        #region 获取部门选择表多选
        public ActionResult DepMulLookUp()
        {
            if (Session["Account"] != null)
            {
                CommonHelper commonHelper = new CommonHelper();
                ViewBag.StructMulTree = commonHelper.GetStructMulTree();
                return View();
            }
            else
            {
                return View();
            }

        }
        #endregion

        #region 获取部门单选
        public ActionResult DepLookUp()
        {
            CommonHelper commonHelper = new CommonHelper();
            ViewBag.StructTree = commonHelper.GetStructTree(false);
            return View();
        }
        #endregion

        #region 获取职位选择表
        public ActionResult PosMulLookUp()
        {
            CommonHelper commonHelper = new CommonHelper();
            ViewBag.StructTree = commonHelper.GetStructTree(false);
            return View();
        }
        public JsonResult GetPosListByDep(GridPager pager, string depId)
        {
            if (Session["Account"] != null)
            {
                var userList = sysPosBLL.GetPosListByDepId(ref pager, depId);

                var jsonData = new
                {
                    total = pager.totalRows,
                    rows = (
                        from r in userList
                        select new SysPositionModel()
                        {
                            Id = r.Id,
                            Name = r.Name,
                            DepName = r.DepName,
                            Flag = "<input type='checkbox' id='cb_" + r.Id + "' onclick='SetValue(\"" + r.Id + "\",\"" + r.Name + "\")'>",
                        }
                    ).ToArray()
                };
                return Json(jsonData);
            }
            else
            {
                return Json(0);
            }
        }
        #endregion

        #region 获取职位选择表
        public ActionResult PosLookUp()
        {
            CommonHelper commonHelper = new CommonHelper();
            ViewBag.StructTree = commonHelper.GetStructTree(false);
            return View();
        }

        #endregion
    }
}
