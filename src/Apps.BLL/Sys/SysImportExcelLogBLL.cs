using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System;
using Apps.Models.Sys;

namespace Apps.BLL.Sys
{
    public  partial class SysImportExcelLogBLL
    {

        public override List<SysImportExcelLogModel> CreateModelList(ref IQueryable<SysImportExcelLog> queryData)
        {

            List<SysImportExcelLogModel> modelList = (from r in queryData
                                              select new SysImportExcelLogModel
                                              {
                                                  CreateBy = r.CreateBy,
                                                  Id = r.Id,
                                                  ImportFileName = r.ImportFileName,
                                                  ImportFilePathUrl = r.ImportFilePathUrl,
                                                  ImportStatus = r.ImportStatus,
                                                  ImportTable = r.ImportTable,
                                                  ImportTime = r.ImportTime,
                                              }).ToList();
            return modelList;
        }
    }
 }

