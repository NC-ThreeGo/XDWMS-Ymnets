using System;
using Apps.Models;
using System.Collections.Generic;
namespace Apps.IDAL
{
    public interface IWebpartRepository
    {
        SysUserConfig GetByIdAndUserId(string id, string userId);
        List<P_Sys_WebPart_Result> GetPartData3(int top, string userId);
        //System.Collections.Generic.List<P_Mis_FileGetMyReadFile_Result> GetPartData8(int top, string userId);
        int SaveHtml(string userId, string html);
    }
}
