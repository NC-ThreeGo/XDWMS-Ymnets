using System;
using Unity.Attributes;
using Apps.BLL.Core;
using Apps.IBLL;
using Apps.Common;
using Apps.Models;
using Apps.IDAL;
using System.Collections.Generic;

namespace Apps.BLL
{
    public class WebpartBLL :  IWebpartBLL
    {

        [Dependency]
        public IWebpartRepository webPartRepository { get; set; }

        /// <summary>
        /// 获取最新信息
        /// </summary>
        /// <param name="top"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<P_Sys_WebPart_Result> GetPartData3(int top, string userId)
        {
            return webPartRepository.GetPartData3(top, userId);
        }
        /// <summary>
        /// 获取最新文件信息
        /// </summary>
        /// <param name="top"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        //public List<P_Mis_FileGetMyReadFile_Result> GetPartData8(int top, string userId)
        //{
        //    return webPartRepository.GetPartData8(top, userId);
        //}
        /// <summary>
        /// 保存HTML
        /// </summary>
        /// <param name="html"></param>
        public bool SaveHtml(ref ValidationErrors errors, string userId, string html)
        {
            try
            {
                if (webPartRepository.SaveHtml(userId, html) > 0)
                {
                    return true;
                }
                else
                {
                    errors.Add("保存失败！");
                    return false;
                }
            }
            catch(Exception ex)
            {
                errors.Add(ex.Message);
                ExceptionHander.WriteException(ex);
                return false;
            }

        }
        public SysUserConfig GetByIdAndUserId(string id, string userId)
        {
            return webPartRepository.GetByIdAndUserId(id, userId);
        }
    }
}
