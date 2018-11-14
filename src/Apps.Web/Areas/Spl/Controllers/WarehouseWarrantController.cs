
using System.Collections.Generic;
using System.Linq;
using Apps.Web.Core;
using Apps.IBLL.Spl;
using Apps.Locale;
using System.Web.Mvc;
using Apps.Common;
using Apps.IBLL;
using Apps.Models.Spl;
using Unity.Attributes;
using Apps.Models.Sys;
using System;
using Apps.Models;
using Apps.BLL.Sys;
using Newtonsoft.Json;
using System.Data;
using Newtonsoft.Json.Linq;

namespace Apps.Web.Areas.Spl.Controllers
{
    public class WarehouseWarrantController : BaseController
    {
        [Dependency]
        public ISpl_WarehouseWarrantBLL m_BLL { get; set; }
        [Dependency]
        public ISpl_WarehouseWarrantDetailsBLL detailsBLL { get; set; }
        [Dependency]
        public ISpl_WarehouseBLL WarehouseBLL { get; set; }
        [Dependency]
        public ISpl_InOutCategoryBLL InOutCategoryBLL { get; set; }

        [Dependency]
        public ISpl_WareDetailsBLL wareDetailsBLL { get; set; }

        private SysConfigModel siteConfig = new SysConfigBLL().loadConfig(Utils.GetXmlMapPath("Configpath"));

        ValidationErrors errors = new ValidationErrors();

        [SupportFilter]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetList(GridPager pager, string queryStr)
        {
            List<Spl_WarehouseWarrantModel> list = m_BLL.GetList(ref pager, queryStr,GetUserId());
            GridRows<Spl_WarehouseWarrantModel> grs = new GridRows<Spl_WarehouseWarrantModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }

        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetDetailsList(GridPager pager, string queryStr, string warrantId)
        {
            List<Spl_WarehouseWarrantDetailsModel> list = detailsBLL.GetList(ref pager, queryStr, warrantId);
            GridRows<Spl_WarehouseWarrantDetailsModel> grs = new GridRows<Spl_WarehouseWarrantDetailsModel>();

            List<Spl_WarehouseWarrantDetailsModel> footerList = new List<Spl_WarehouseWarrantDetailsModel>();
            footerList.Add(new Spl_WarehouseWarrantDetailsModel()
            {
                WareDetailsName = "<div style='text-align:right;color:#444'>合计：</div>",
                oper = "<a class='fa fa-plus color-green' href='javascript:append()'><a>",
                WarehouseId = "合计",
                Price = 0,
                Quantity = 0,
                TotalPrice = 0,
            });

            grs.rows = list;
            grs.footer = footerList;
            grs.total = pager.totalRows;
            return Json(grs);
        }

        [HttpPost]
        public JsonResult GetComboxDataByWarehouse()
        {
            List<Spl_WarehouseModel> list = WarehouseBLL.GetList(ref setNoPagerAscById, "");
            var json = (from r in list
                        select new Spl_ProductCategoryModel()
                        {
                            Id = r.Id,
                            Name = r.Name
                        }).ToArray();

            return Json(json);
        }


        #region 创建


        [SupportFilter(ActionName = "Create")]
        public ActionResult WareDetails()
        {
            CommonHelper commonHelper = new CommonHelper();
            ViewBag.GetWareCateogryTree = commonHelper.GetWareCateogryTree(true);
            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Create")]
        public JsonResult WareDetailsGetList(GridPager pager, string queryStr, string category)
        {
            List<Spl_WareDetailsModel> list = wareDetailsBLL.GetList(ref pager, queryStr, category);
            GridRows<Spl_WareDetailsModel> grs = new GridRows<Spl_WareDetailsModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }

        [SupportFilter]
        public ActionResult Create()
        {
            ViewBag.Warehouse = new SelectList(WarehouseBLL.GetList(ref setNoPagerAscById, "", GetUserId()), "Id", "Name");
            ViewBag.InOutCategory = new SelectList(InOutCategoryBLL.GetList(ref setNoPagerAscById, "入库"), "Id", "Name");
            AccountModel accountModel = GetAccount();
            Spl_WarehouseWarrantModel model = new Spl_WarehouseWarrantModel()
            {
                Id = "RKD" + DateTime.Now.ToString("yyyyMMddHHmmssff"),
                Handler = accountModel.TrueName,
                HandlerName = accountModel.TrueName,

            };
            ViewBag.Checker = new SelectList(WarehouseBLL.GetList(ref setNoPagerAscById, "", GetUserId()).GroupBy(p=>new { p.Id,p.ContactPerson}).Select(g=>new { Id=g.Key.Id, ContactPerson=g.Key.ContactPerson }), "Id", "ContactPerson");
 
            return View(model);
        }

        [HttpPost]
        [SupportFilter]
        [ValidateInput(false)]
        public JsonResult Create(Spl_WarehouseWarrantModel model, string inserted)
        {
            var detailsList = JsonHandler.DeserializeJsonToList<Spl_WarehouseWarrantDetailsModel>(inserted);
            if (detailsList != null && detailsList.Count != 0)
            {
                //model.Id = ResultHelper.NewId;
                model.CreateTime = ResultHelper.NowTime;
                model.CreatePerson = GetUserId();
                model.Checker = GetUserId();//修改于2018年3月19
                //计算总价
                model.PriceTotal = detailsList.Sum(a => a.Quantity * a.Price);
                if (model != null && ModelState.IsValid)
                {

                    if (m_BLL.Create(ref errors, model))
                    {

                        var detailsResultList = new List<Spl_WarehouseWarrantDetailsModel>();
                        //新加

                        foreach (var r in detailsList)
                        {
                            //过滤无效数据
                            if (string.IsNullOrEmpty(r.WareDetailsId))
                            {
                                continue;
                            }
                            Spl_WarehouseWarrantDetailsModel entity = new Spl_WarehouseWarrantDetailsModel();
                            entity.Id = "";
                            entity.WareDetailsId = r.WareDetailsId;
                            entity.WarehouseId = model.WarehouseId;
                            entity.WarehouseWarrantId = model.Id;
                            entity.Quantity = r.Quantity;
                            entity.Price = r.Price;
                            entity.TotalPrice = r.Quantity * r.Price;
                            entity.Defined = string.IsNullOrWhiteSpace(r.Defined) ? "" : r.Defined;
                            entity.CreateTime = ResultHelper.NowTime;
                            detailsResultList.Add(entity);
                        }

                        try
                        {
                            m_BLL.SaveData(detailsResultList);
                            LogHandler.WriteServiceLog(GetUserId(), "保存成功", "成功", "保存", "Spl_WarehouseWarrant");
                            return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                        }
                        catch (Exception ex)
                        {
                            LogHandler.WriteServiceLog(GetUserId(), ex.Message, "失败", "保存", "Spl_WarehouseWarrant");
                            return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ex.Message));
                        }
                    }
                    else
                    {
                        string ErrorCol = errors.Error;
                        LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",InTime" + model.InTime + "," + ErrorCol, "失败", "创建", "Spl_WarehouseWarrant");
                        return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
                    }
                }
            }
            return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ":没有明细"));
        }
        #endregion


        #region 打印
        [SupportFilter(ActionName ="Index")]
        public ActionResult Print(string id)
        {
            ViewBag.ComName = siteConfig.webcompany;
            Spl_WarehouseWarrantModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetPrintDetailsList(string warrantId)
        {
            List<Spl_WarehouseWarrantDetailsModel> list = detailsBLL.GetList(ref setNoPagerAscById, "", warrantId);
            GridRows<Spl_WarehouseWarrantDetailsModel> grs = new GridRows<Spl_WarehouseWarrantDetailsModel>();

            List<Spl_WarehouseWarrantDetailsModel> footerList = new List<Spl_WarehouseWarrantDetailsModel>();
            footerList.Add(new Spl_WarehouseWarrantDetailsModel()
            {
                WareDetailsName = "<div style='text-align:right;color:#444'>合计：</div>",
                oper = "<a class='fa fa-plus color-green' href='javascript:append()'><a>",
                WarehouseId = "合计",
                Price = list.Sum(a => a.Price),
                Quantity = list.Sum(a => a.Quantity),
                TotalPrice = list.Sum(a=>a.TotalPrice),
            });

            grs.rows = list;
            grs.footer = footerList;
            grs.total = list.Count();
            return Json(grs);
        }

        #endregion

        #region 修改
        [SupportFilter]
        public ActionResult Edit(string id)
        {
            Spl_WarehouseWarrantModel entity = m_BLL.GetById(id);
            ViewBag.Warehouse = new SelectList(WarehouseBLL.GetList(ref setNoPagerAscById, "", GetUserId()), "Id", "Name", entity.WarehouseId);
            ViewBag.InOutCategory = new SelectList(InOutCategoryBLL.GetList(ref setNoPagerAscById, "入库"), "Id", "Name", entity.InOutCategoryId);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        [ValidateInput(false)]
        public JsonResult Edit(Spl_WarehouseWarrantModel model, string inserted)
        {
            var detailsList = JsonHandler.DeserializeJsonToList<Spl_WarehouseWarrantDetailsModel>(inserted);
            if (detailsList != null && detailsList.Count != 0)
            {
                //计算总价
                model.PriceTotal = detailsList.Sum(a => a.Quantity * a.Price);
                model.ModifyTime = DateTime.Now;
                if (model != null && ModelState.IsValid)
                {

                    if (m_BLL.Edit(ref errors, model))
                    {

                        var detailsResultList = new List<Spl_WarehouseWarrantDetailsModel>();
                        //新加

                        foreach (var r in detailsList)
                        {
                            //过滤无效数据
                            if (string.IsNullOrEmpty(r.WareDetailsId))
                            {
                                continue;
                            }
                            Spl_WarehouseWarrantDetailsModel entity = new Spl_WarehouseWarrantDetailsModel();
                            entity.Id = r.Id;
                            entity.WareDetailsId = r.WareDetailsId;
                            entity.WarehouseId = model.WarehouseId;
                            entity.WarehouseWarrantId = model.Id;
                            entity.Quantity = r.Quantity;
                            entity.Price = r.Price;
                            entity.TotalPrice = r.Quantity * r.Price;
                            entity.Defined = string.IsNullOrWhiteSpace(r.Defined) ? "" : r.Defined;
                            entity.CreateTime = ResultHelper.NowTime;
                            detailsResultList.Add(entity);
                        }

                        try
                        {
                            m_BLL.SaveEditData(detailsResultList, model.Id);
                            LogHandler.WriteServiceLog(GetUserId(), "保存成功", "成功", "保存", "Spl_WarehouseWarrant");
                            return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                        }
                        catch (Exception ex)
                        {
                            LogHandler.WriteServiceLog(GetUserId(), ex.Message, "失败", "保存", "Spl_WarehouseWarrant");
                            return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ex.Message));
                        }
                    }
                    else
                    {
                        string ErrorCol = errors.Error;
                        LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",InTime" + model.InTime + "," + ErrorCol, "失败", "创建", "Spl_WarehouseWarrant");
                        return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
                    }
                }
            }
            return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ":没有明细"));
        }
        #endregion

        #region 详细
        [SupportFilter]
        public ActionResult Details(string id)
        {
            Spl_WarehouseWarrantModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public JsonResult Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "Spl_WarehouseWarrant");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "Spl_WarehouseWarrant");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));
            }


        }
        #endregion
        #region  审查
        [HttpPost]
        [SupportFilter]
        public JsonResult Check(string Id)
        {

            if (!string.IsNullOrWhiteSpace(Id))
            {

                if (m_BLL.Check(ref errors, Id, 1, GetUserId()))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + Id, "成功", "审核", "信息中心");
                    return Json(JsonHandler.CreateMessage(1, Resource.CheckSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + Id + "," + ErrorCol, "失败", "审核", "信息中心");
                    return Json(JsonHandler.CreateMessage(0, Resource.CheckFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.CheckFail));
            }
        }

        #endregion
        #region 导出
        private JArray GetExportData()
        {
            List<Spl_WarehouseWarrantModel> list = m_BLL.GetList(ref setNoPagerAscById, "");
            JArray jObjects = new JArray();

            foreach (var item in list)
            {
                var jo = new JObject();
                jo.Add("Id", item.Id);
                jo.Add("入库时间", item.InTime);
                jo.Add("经手人", item.Handler);
                jo.Add("说明", item.Remark);
                jo.Add("总价", item.PriceTotal);
                jo.Add("状态", item.State);
                jo.Add("审查人", item.Checker);
                jo.Add("审查时间", item.CheckTime);
                jo.Add("创建时间", item.CreateTime);
                jo.Add("制单人", item.CreatePerson); 
                jo.Add("修改人", item.ModifyPerson);
                jo.Add("修改时间", item.ModifyTime);
                jo.Add("单据确认",item.Confirmation);
                jo.Add("入库类别",item.InOutCategoryName);
                jo.Add("所属仓库",item.WarehouseName);
                jObjects.Add(jo);
            }
            return jObjects;
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData()
        {
            List<Spl_WarehouseWarrantModel> list = m_BLL.GetList(ref setNoPagerAscById, "");
            if (list.Count().Equals(0))
            {
                return Json(JsonHandler.CreateMessage(0, "没有可以导出的数据"));
            }
            else
            {
                return Json(JsonHandler.CreateMessage(1, "可以导出"));
            }
        }

        [SupportFilter]
        public ActionResult Export()
        {
            var exportSpource = this.GetExportData();
            var dt = JsonConvert.DeserializeObject<DataTable>(exportSpource.ToString());

            var exportFileName = string.Concat(
                "入库单信息",
                DateTime.Now.ToString("yyyyMMddHHmmss"),
                ".xlsx");

            return new ExportExcelResult
            {
                SheetName = "入库单",
                FileName = exportFileName,
                ExportData = dt
            };
        }
        #endregion
    }
}
