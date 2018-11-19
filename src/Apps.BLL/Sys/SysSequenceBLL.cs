using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System;
using Apps.Models.Sys;

namespace Apps.BLL.Sys
{
    public  partial class SysSequenceBLL
    {

        public override List<SysSequenceModel> CreateModelList(ref IQueryable<SysSequence> queryData)
        {

            List<SysSequenceModel> modelList = (from r in queryData
                                              select new SysSequenceModel
                                              {
                                                  CurrentValue = r.CurrentValue,
                                                  FirstLength = r.FirstLength,
                                                  FirstRule = r.FirstRule,
                                                  FirstType = r.FirstType,
                                                  FirstTypeEnum = (SequenceType)r.FirstType,
                                                  FirstTypeEnumText = ((SequenceType)r.FirstType).ToString(),
                                                  FourLength = r.FourLength,
                                                  FourRule = r.FourRule,
                                                  FourType = r.FourType,
                                                  Id = r.Id,
                                                  JoinChar = r.JoinChar,
                                                  Remark = r.Remark,
                                                  Sample = r.Sample,
                                                  SecondLength = r.SecondLength,
                                                  SecondRule = r.SecondRule,
                                                  SecondType = r.SecondType,
                                                  SecondTypeEnum = (SequenceType)r.SecondType,
                                                  SecondTypeEnumText = ((SequenceType)r.SecondType).ToString(),
                                                  SN = r.SN,
                                                  TabName = r.TabName,
                                                  ThirdLength = r.ThirdLength,
                                                  ThirdRule = r.ThirdRule,
                                                  ThirdType = r.ThirdType,
                                              }).ToList();
            return modelList;
        }
    }
 }

