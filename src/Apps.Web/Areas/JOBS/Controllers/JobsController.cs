using System;
using System.Web.Mvc;
using Apps.Common;
using Apps.Jobs;
using Unity.Attributes;
using Apps.BLL;
using System.Linq;
using Apps.Web.Core;
using Apps.Locale;
using Apps.IBLL;
using Apps.Models.JOB;
using Apps.IBLL.JOB;

namespace Apps.Web.Areas.JOBS.Controllers
{
    public class JobsController : BaseController
    {
        [Dependency]
        public IJOB_TASKJOBSBLL m_BLL { get; set; }
        [Dependency]
        public IJOB_TASKJOBS_LOGBLL m_LogBLL { get; set; }
        //错误集合
        ValidationErrors validationErrors = new ValidationErrors();
        //
        // GET: /JOBS/Jobs/
        [SupportFilter(ActionName = "Create")]
        public ActionResult CustomProcedureJob()
        {
            ViewBag.perm = GetPermission();
            DateTime now = DateTime.Now;
            JobModel model = new JobModel()
            {
                id = "",
                taskName = "",
                taskTitle = "",
                creator = GetUserId(),
                //简单任务
                startDate = now,
                endDate = now,
                intervalCount = 10,
                intervalType = "m",
                repeatCount = 0,
                repeatForever = false,
                //复杂任务
                seconds = "0",
                minutes = "1",
                hours = "*",
                dayOfMonth = "*",
                month = "*",
                dayOfWeek = "?",
            };
            return View(model);
        }
        [HttpPost]
        public JsonResult CustomProcedureJob(string task)
        {
            try
            {
                //提交任务
                JobsTools.CreateCustomProcedureJob(ref validationErrors, task);//带命名空间的任务处理类名称
                ////
                if (validationErrors.Count > 0)
                {
                    return Json(JsonHandler.CreateMessage(0, validationErrors.Error));
                }

                return Json(JsonHandler.CreateMessage(1, "任务已提交"));
            }
            catch (Exception ex)
            {
                return Json(JsonHandler.CreateMessage(0, ex.Message));
            }

        }
        [SupportFilter(ActionName = "Create")]
        public ActionResult CustomAssemblyJob()
        {
            ViewBag.perm = GetPermission();
            DateTime now = DateTime.Now;
            JobModel model = new JobModel()
            {
                id = "",
                taskName = "",
                taskTitle = "",
                creator = GetUserId(),
                //简单任务
                startDate = now,
                endDate = now,
                intervalCount = 10,
                intervalType = "m",
                repeatCount = 0,
                repeatForever = false,
                //复杂任务
                seconds = "0",
                minutes = "1",
                hours = "*",
                dayOfMonth = "*",
                month = "*",
                dayOfWeek = "?",
            };
            return View(model);
        }
        [HttpPost]
        public JsonResult CustomAssemblyJob(string task,string assembly,string id,string title)
        {
            try
            {
                //提交任务
                //"Apps.Jobs.MIS.DiscussPostNewTopicsJob"
                JobsTools.CreateTaskJob(ref validationErrors, task, assembly, id, title);
                ////
                if (validationErrors.Count > 0)
                {
                    return Json(JsonHandler.CreateMessage(0, validationErrors.Error));
                }

                return Json(JsonHandler.CreateMessage(1, "任务已提交"));
            }
            catch (Exception ex)
            {
                return Json(JsonHandler.CreateMessage(0, ex.Message));
            }

        }

        public ActionResult Scheduler(string taskName, string id, string taskTitle, string memo)
        {
            if (string.IsNullOrEmpty(taskName))
            {
                taskName = "";
            }
            if (string.IsNullOrEmpty(id))
            {
                id = "";
            }
            if (string.IsNullOrEmpty(taskTitle))
            {
                taskTitle = "";
            }
            DateTime now = DateTime.Now;
            JobModel model = new JobModel()
            {
                id = id,
                taskName = taskName,
                taskTitle = taskTitle,
                creator = GetUserId(),
                //简单任务
                startDate = now,
                endDate = now,
                intervalCount = 10,
                intervalType = "m",
                repeatCount = 0,
                repeatForever = false,
                //复杂任务
                seconds = "0",
                minutes = "1",
                hours = "*",
                dayOfMonth = "*",
                month = "*",
                dayOfWeek = "?",
            };
            return View(model);
        }
        [HttpPost]
        public JsonResult Scheduler(string task, string taskName, string id)
        {
            try
            {
                if (string.IsNullOrEmpty(taskName))
                {
                    return Json(JsonHandler.CreateMessage(0, "任务名称不能为空"));

                }

                if (!taskName.Contains("App.Jobs."))
                {
                    return Json(JsonHandler.CreateMessage(0, "任务名称未包含命名空间App.Jobs"));
                }
                if (string.IsNullOrEmpty(id))
                {
                    return Json(JsonHandler.CreateMessage(0, "任务ID不能为空"));

                }
                //提交任务
                JobsTools.CreateTaskJob(ref validationErrors, task, "App.Jobs.MIS.CreateInfo", id);//带命名空间的任务处理类名称
                ////
                if (validationErrors.Count > 0)
                {
                    return Json(JsonHandler.CreateMessage(0, validationErrors.Error));
                }

                return Json(JsonHandler.CreateMessage(1, "任务已提交"));
            }
            catch (Exception ex)
            {
                return Json(JsonHandler.CreateMessage(0, ex.Message));
            }

        }
        [HttpPost]
        public JsonResult CloseTaskJob(string taskName, string id)
        {
            try
            {
                if (string.IsNullOrEmpty(taskName))
                {
                    return Json(JsonHandler.CreateMessage(0, "任务名称不能为空"));

                }

                if (!taskName.Contains("App.Jobs."))
                {
                    return Json(JsonHandler.CreateMessage(0, "任务名称未包含命名空间App.Jobs"));
                }
                if (string.IsNullOrEmpty(id))
                {
                    return Json(JsonHandler.CreateMessage(0, "任务ID不能为空"));

                }
                //提交任务
                JobsTools.CloseTaskJob(ref validationErrors, taskName, id);//带命名空间的任务处理类名称
                ////
                if (validationErrors.Count > 0)
                {
                    return Json(JsonHandler.CreateMessage(0, validationErrors.Error));
                }

                return Json(JsonHandler.CreateMessage(1, "任务已提交"));
            }
            catch (Exception ex)
            {
                return Json(JsonHandler.CreateMessage(0, ex.Message));
            }

        }
        //详细主页
        [SupportFilter(ActionName = "Index")]
        public ActionResult Index()
        {
            ViewBag.perm = GetPermission();
            return View();
        }
        private string CreateActions(string sno)
        {
            string script;
            script = "<span title='删除' class='fa fa-trash' onclick=\"Delete('" + sno + "');\"></span>";
            script += "&nbsp;<span  title='暂停' class='fa fa-pause btnlrm' onclick=\"Pause('" + sno + "');\"></span>";
            script += "&nbsp;<span  title='重启' class='fa fa-play btnlrm' onclick=\"Resume('" + sno + "');\"></span>";
            script += "&nbsp;<span  title='清空日志' class='fa fa-trash-o ' onclick=\"DeleteLog('" + sno + "');\"></span>";
            return script;

        }

        //JQGrid获取
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetList(GridPager pager, string querystr)
        {

            try
            {
                var colList = m_BLL.GetListByCustom(ref pager, querystr);
                var jsonData = new
                {
                    total = pager.totalRows,
                    rows = (from r in colList
                            select new JOB_TASKJOBSModel()
                            {
                                sno = r.sno,
                                action = CreateActions(r.sno),//操作
                                taskName = r.taskName,
                                Id = r.Id,
                                taskTitle =r.taskTitle,
                                taskCmd =  r.taskCmd,
                                crtDt = r.crtDt,
                                state = r.state,
                                stateTitle = r.state == null ? "" : r.state == 0 ? "准备" : r.state == 1 ? "成功" : r.state == 2 ? "关闭" : r.state == 3 ? "挂起" : r.state == 4 ? "重启" : "",
                                procName = r.procName,
                                procParams = r.procParams,
                                creator =r.creator
                               

                            }).ToArray()

                };
                return Json(jsonData);

            }
            catch (Exception ex)
            {
                LogHandler.WriteServiceLog(GetUserId(), "读取出错,查询参数:" + querystr + " 错误：" + ex.Message, "失败", "创建", "任务管理");
                return null;
            }

        }
        //JQGrid获取
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetTaskJobsLogList(GridPager pager, string querystr)
        {

            try
            {
                var colList = m_LogBLL.GetListBySno(ref pager, querystr);
                if (colList == null)
                {
                    return null;
                }

                var jsonData = new
                {
                    total = pager.totalRows,
                    rows = (from r in colList
                            select new JOB_TASKJOBS_LOGModel()
                            {
                               itemID = r.itemID,//ID
                               sno = r.sno,
								//输出结果列表
                               taskName=r.taskName.ToString(),
                               Id =r.Id.ToString(),
                               executeDt= r.executeDt,
                               executeStep=r.executeStep,
                               result=r.result


                            }).ToArray()

                };

             
                return Json(jsonData);

            }
            catch (Exception ex)
            {
                LogHandler.WriteServiceLog(GetUserId(), "读取出错,查询参数:" + querystr + " 错误：" + ex.Message, "失败", "创建", "任务管理");
                return null;
            }

        }
        //新增
        [SupportFilter]
        public ActionResult Create()
        {
            return View();
        }

        //创建提交
        [HttpPost]
        [SupportFilter]
        public JsonResult Create(JOB_TASKJOBSModel model)
        {
            try
            {
                string mes = "";
                if (!ValidateSQL(model.taskCmd,ref mes))
                {
                    return Json(JsonHandler.CreateMessage(0,"为了方便测试安全"+ mes));
                }

                if (model == null || !ModelState.IsValid)
                {
                    return Json(JsonHandler.CreateMessage(0, "上传参数错误"));
                }


                //新增
                m_BLL.Create(ref validationErrors, model);
                //写日志
                if (validationErrors.Count > 0)
                {
                    //错误写入日志
                    LogHandler.WriteServiceLog(GetUserId(), "创建任务管理ID:" + model.sno, "失败", "创建", "任务管理");
                    return Json(JsonHandler.CreateMessage(0, validationErrors.Error));
                }
                //成功写入日志
                LogHandler.WriteServiceLog(GetUserId(), "创建任务管理ID:" + model.sno, "成功", "创建", "任务管理");
                return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
            }
            catch
            {
                return Json(JsonHandler.CreateMessage(1, Resource.InsertFail));
            }
        }
        //修改
        [SupportFilter]
        public ActionResult Edit(string sno)
        {
            if (!ModelState.IsValid)
            {

                return null;
            }
            JOB_TASKJOBSModel model = m_BLL.GetModelById(sno);

            return View(model);

        }
        //修改
        [HttpPost]
        [SupportFilter]
        public ActionResult Edit(JOB_TASKJOBSModel model)
        {
            try
            {
                if (model == null || !ModelState.IsValid)
                {
                    return Json(JsonHandler.CreateMessage(0, "上传参数错误"));
                }

                m_BLL.Edit(ref validationErrors, model);
                //写日志
                if (validationErrors.Count > 0)
                {
                    //错误写入日志
                    LogHandler.WriteServiceLog(GetUserId(), "编辑任务管理ID:" + model.sno, "失败", "编辑", "任务管理");
                    return Json(JsonHandler.CreateMessage(0, validationErrors.Error));
                }
                //成功写入日志
                LogHandler.WriteServiceLog(GetUserId(), "编辑任务管理ID:" + model.sno, "成功", "编辑", "任务管理");
                return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
            }
            catch
            {
                return Json(JsonHandler.CreateMessage(0, Resource.UpdateFail));
            }

        }
        //详细
        [SupportFilter]
        public ActionResult Details(string sno)
        {
            if (!ModelState.IsValid)
            {

                return null;
            }
            JOB_TASKJOBSModel model = m_BLL.GetModelById(sno);

            return View(model);

        }

        // 删除 
        [HttpPost]
        [SupportFilter(ActionName = "Delete")]
        public JsonResult Delete(string sno)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(JsonHandler.CreateMessage(0, "上传参有错误"));
                }
                if (string.IsNullOrEmpty(sno))
                {
                    return Json(JsonHandler.CreateMessage(0, "请选择删除记录"));
                }



                if (m_BLL.DeleteJob(ref validationErrors, sno))
                {
                    //关闭任务
                    JobsTools.CloseTaskJob(ref validationErrors, sno);
                }
                //写日志
                if (validationErrors.Count > 0)
                {
                    //错误写入日志
                    LogHandler.WriteServiceLog(GetUserId(), "删除任务管理ID:" + sno, "失败", "删除", "任务管理");
                    return Json(JsonHandler.CreateMessage(0, validationErrors.Error));
                }
                //成功写入日志
                LogHandler.WriteServiceLog(GetUserId(), "删除任务管理ID:" + sno, "成功", "删除", "任务管理");
                return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
            }
            catch
            {
                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));
            }
        }
        // 删除日志
        [HttpPost]
        [SupportFilter(ActionName = "Delete")]
        public JsonResult DeleteLog(string sno)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(JsonHandler.CreateMessage(0, "上传参有错误"));
                }
                if (string.IsNullOrEmpty(sno))
                {
                    return Json(JsonHandler.CreateMessage(0, "请选择删除记录"));
                }


                m_LogBLL.DeleteBySno(ref validationErrors, sno);

                //写日志
                if (validationErrors.Count > 0)
                {
                    //错误写入日志
                    LogHandler.WriteServiceLog(GetUserId(), "删除任务日志ID:" + sno, "失败", "删除", "任务管理");
                    return Json(JsonHandler.CreateMessage(0, validationErrors.Error));
                }
                //成功写入日志
                LogHandler.WriteServiceLog(GetUserId(), "删除任务日志ID:" + sno, "成功", "删除", "任务管理");
                return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
            }
            catch
            {
                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));
            }
        }

        // 清空离线任务
        [HttpPost]
        [SupportFilter(ActionName = "Delete")]
        public JsonResult ClearUnTrackTask()
        {
            try
            {

                int taskNum = JobsTools.ClearUnTrackTask(ref validationErrors);

                //写日志
                if (validationErrors.Count > 0)
                {
                    //错误写入日志
                    LogHandler.WriteServiceLog(GetUserId(), "清空未被跟踪的任务" + validationErrors.Error, "失败", "删除", "任务管理");
                    return Json(JsonHandler.CreateMessage(0, validationErrors.Error));
                }
                //成功写入日志
                LogHandler.WriteServiceLog(GetUserId(), "清空未被跟踪的任务", "成功", "删除", "任务管理");
                string msg = "";
                if (taskNum < 1)
                {
                    msg = "没有离线任务需要清理";
                }
                else
                {
                    msg = "共有" + taskNum + "个离线任务被清理";
                }


                return Json(JsonHandler.CreateMessage(1, msg));
            }
            catch
            {
                return Json(JsonHandler.CreateMessage(0, "清空未被跟踪的任务异常"));
            }
        }


        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        public JsonResult Resume(string sno)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(JsonHandler.CreateMessage(0, "上传参有错误"));
                }
                if (string.IsNullOrEmpty(sno))
                {
                    return Json(JsonHandler.CreateMessage(0, "请选择任务记录"));
                }


                //重启任务
                JobsTools.ResumeTaskJob(ref validationErrors, sno);
                //写日志
                if (validationErrors.Count > 0)
                {
                    //错误写入日志
                    LogHandler.WriteServiceLog(GetUserId(), "重启任务管理Id:" + sno, "失败", "重启", "任务管理");
                    return Json(JsonHandler.CreateMessage(0, validationErrors.Error));
                }
                //成功写入日志
                LogHandler.WriteServiceLog(GetUserId(), "重启任务管理Id:" + sno, "成功", "重启", "任务管理");
                return Json(JsonHandler.CreateMessage(1, "任务成功重启"));
            }
            catch
            {
                return Json(JsonHandler.CreateMessage(0, "任务重启失败"));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        public JsonResult Pause(string sno)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(JsonHandler.CreateMessage(0, "上传参有错误"));
                }
                if (string.IsNullOrEmpty(sno))
                {
                    return Json(JsonHandler.CreateMessage(0, "请选择任务记录"));
                }


                //重启任务
                JobsTools.PauseTaskJob(ref validationErrors, sno);
                //写日志
                if (validationErrors.Count > 0)
                {
                    //错误写入日志
                    LogHandler.WriteServiceLog(GetUserId(), "重启任务管理Id:" + sno, "失败", "重启", "任务管理");
                    return Json(JsonHandler.CreateMessage(0, validationErrors.Error));
                }
                //成功写入日志
                LogHandler.WriteServiceLog(GetUserId(), "重启任务管理Id:" + sno, "成功", "重启", "任务管理");
                return Json(JsonHandler.CreateMessage(1, "任务已暂停"));
            }
            catch
            {
                return Json(JsonHandler.CreateMessage(0, "任务暂停失败"));
            }
        }

    }
}
