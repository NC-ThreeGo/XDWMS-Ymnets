using System.Collections.Generic;
using Apps.Web.Core;
using Apps.IBLL.Spl;
using Apps.Locale;
using System.Web.Mvc;
using Apps.Common;
using Apps.Models.Spl;
using Unity.Attributes;
using Apps.Models.Common;


namespace Apps.Web.Areas.Spl.Controllers
{
    public class ProductController : BaseController
    {
        [Dependency]
        public ISpl_ProductBLL m_BLL { get; set; }
        ValidationErrors errors = new ValidationErrors();

        #region SQL样例柱状图

        public ActionResult Index2()
        {
            return View();
        }

        public JsonResult GetList2(GridPager pager, string queryStr)
        {
            List<ProductPillarModel> list = m_BLL.GetListByPillar(ref pager, queryStr);
            GridRows<ProductPillarModel> grs = new GridRows<ProductPillarModel>();
            grs.rows = list;
            grs.total = pager.totalRows;

            return Json(grs);
        }

        public JsonResult GetOptionByBarChart2(GridPager pager, string queryStr)
        {
            List<ProductPillarModel> list = m_BLL.GetListByPillar(ref pager, queryStr);
            List<decimal?> costPrice = new List<decimal?>();
            list.ForEach(a => costPrice.Add(a.Price));
            List<decimal?> numbers = new List<decimal?>();
            list.ForEach(a => numbers.Add(a.Number));
            List<string> names = new List<string>();
            list.ForEach(a => names.Add(a.Name + "(" + a.Color + ")"));
            List<ChartSeriesModel> seriesList = new List<ChartSeriesModel>();
            ChartSeriesModel series1 = new ChartSeriesModel()
            {
                name = "价格",
                type = "bar",
                data = costPrice
            };
            ChartSeriesModel series2 = new ChartSeriesModel()
            {
                name = "销量",
                type = "bar",
                data = numbers
            };
            seriesList.Add(series1);
            seriesList.Add(series2);
            var option = new
            {
                title = new { text = "销量和价格对照表" },
                tooltip = new { },
                legend = new { data = new string[] { "价格", "销量" } },
                xAxis = new { data = names },
                yAxis = new { },
                series = seriesList
            };
            return Json(option);
        }


        #endregion



        [SupportFilter]
        public ActionResult Index()
        {

            return View();
        }



        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetList(GridPager pager, string queryStr)
        {
            List<Spl_ProductModel> list = m_BLL.GetList(ref pager, queryStr);
            GridRows<Spl_ProductModel> grs = new GridRows<Spl_ProductModel>();
            grs.rows = list;
            grs.total = pager.totalRows;

            return Json(grs);
        }

        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetOptionByBarChart(GridPager pager, string queryStr)
        {
            List<Spl_ProductModel> list = m_BLL.GetList(ref pager, queryStr);
            List<decimal?> costPrice = new List<decimal?>();
            list.ForEach(a => costPrice.Add(a.CostPrice));
            List<decimal?> price = new List<decimal?>();
            list.ForEach(a => price.Add(a.Price));
            List<string> names = new List<string>();
            list.ForEach(a => names.Add(a.Name));
            List<ChartSeriesModel> seriesList = new List<ChartSeriesModel>();
            ChartSeriesModel series1 = new ChartSeriesModel()
            {
                name = "成本价",
                type = "bar",
                data = costPrice
            };
            ChartSeriesModel series2 = new ChartSeriesModel()
            {
                name = "零售价",
                type = "bar",
                data = price
            };
            seriesList.Add(series1);
            seriesList.Add(series2);
            var option = new
            {
                title = new { text = "成本价零售价对照表" },
                tooltip = new { },
                legend = new { data = "成本价零售价对照表" },
                xAxis = new { data = names },
                yAxis = new { },
                series = seriesList
            };
            return Json(option);
        }

        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(Spl_ProductModel model)
        {
            model.Id = ResultHelper.NewId;
            model.CreateTime = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name, "成功", "创建", "Spl_Product");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "创建", "Spl_Product");
                    return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail));
            }
        }
        #endregion

        #region 修改
        [SupportFilter]
        public ActionResult Edit(string id)
        {

            Spl_ProductModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(Spl_ProductModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name, "成功", "修改", "Spl_Product");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",Name" + model.Name + "," + ErrorCol, "失败", "修改", "Spl_Product");
                    return Json(JsonHandler.CreateMessage(0, Resource.EditFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.EditFail));
            }
        }
        #endregion

        #region 详细
        [SupportFilter]
        public ActionResult Details(string id)
        {

            Spl_ProductModel entity = m_BLL.GetById(id);
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
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "Spl_Product");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "Spl_Product");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));
            }


        }
        #endregion
    }
}
