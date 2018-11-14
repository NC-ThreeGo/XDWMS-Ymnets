using Apps.Common;
using Apps.Models.Sys;
using Apps.Web.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Apps.Web.Areas.Demo.Controllers
{
    public class BaseDemoController : BaseController
    {
        // GET: Demo/BaseDemo
        public ActionResult Index()
        {
            AccountModel account = new AccountModel();
            account.Id = "admin";
            account.TrueName = "测试";
            account.Photo = "/upload/201610/29/201610291136503066.jpg";
            account.UserName = "admin";
            Session["Account"] = account;

            return View();
        }

        //datagrid模拟数据
        [HttpPost]
        public JsonResult GetListDemo(GridPager pager, string queryStr)
        {
            List<DataSample> list = new List<DataSample>();
            list.Add(new DataSample() { Id = "100001", Name = "第一条", Enable = true, CreateTime = DateTime.Now.AddDays(1) });
            list.Add(new DataSample() { Id = "100002", Name = "第二条", Enable = false, CreateTime = DateTime.Now.AddDays(2) });
            list.Add(new DataSample() { Id = "100003", Name = "第三条", Enable = true, CreateTime = DateTime.Now.AddDays(3) });
            list.Add(new DataSample() { Id = "100004", Name = "第四条", Enable = true, CreateTime = DateTime.Now.AddDays(4) });
            list.Add(new DataSample() { Id = "100005", Name = "第五条", Enable = true, CreateTime = DateTime.Now.AddDays(5) });
            list.Add(new DataSample() { Id = "100006", Name = "第六条", Enable = true, CreateTime = DateTime.Now.AddDays(6) });
            list.Add(new DataSample() { Id = "100007", Name = "第七条", Enable = false, CreateTime = DateTime.Now.AddDays(7) });
            list.Add(new DataSample() { Id = "100008", Name = "第八条", Enable = false, CreateTime = DateTime.Now.AddDays(8) });
            list.Add(new DataSample() { Id = "100009", Name = "第九条", Enable = true, CreateTime = DateTime.Now.AddDays(9) });
            list.Add(new DataSample() { Id = "100010", Name = "第十条", Enable = false, CreateTime = DateTime.Now.AddDays(10) });
            GridRows<DataSample> grs = new GridRows<DataSample>();
            grs.rows = list;
            grs.total = 10;

            return Json(grs);
        }

        [HttpPost]
        public JsonResult GetListDemoCombo(GridPager pager, string queryStr)
        {
            List<DataSample> list = new List<DataSample>();
            list.Add(new DataSample() { Id = "100001", Name = "第一条", Enable = true, CreateTime = DateTime.Now.AddDays(1) });
            list.Add(new DataSample() { Id = "100002", Name = "第二条", Enable = false, CreateTime = DateTime.Now.AddDays(2) });
            list.Add(new DataSample() { Id = "100003", Name = "第三条", Enable = true, CreateTime = DateTime.Now.AddDays(3) });
            list.Add(new DataSample() { Id = "100004", Name = "第四条", Enable = true, CreateTime = DateTime.Now.AddDays(4) });
            list.Add(new DataSample() { Id = "100005", Name = "第五条", Enable = true, CreateTime = DateTime.Now.AddDays(5) });
            list.Add(new DataSample() { Id = "100006", Name = "第六条", Enable = true, CreateTime = DateTime.Now.AddDays(6) });
            list.Add(new DataSample() { Id = "100007", Name = "第七条", Enable = false, CreateTime = DateTime.Now.AddDays(7) });
            list.Add(new DataSample() { Id = "100008", Name = "第八条", Enable = false, CreateTime = DateTime.Now.AddDays(8) });
            list.Add(new DataSample() { Id = "100009", Name = "第九条", Enable = true, CreateTime = DateTime.Now.AddDays(9) });
            list.Add(new DataSample() { Id = "100010", Name = "第十条", Enable = false, CreateTime = DateTime.Now.AddDays(10) });

            return Json(list);
        }
    }
    //datagrid模拟数据
    public class DataSample
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Enable { get; set; }
        public DateTime CreateTime { get; set; }
    }

}