using Apps.Models;
using Apps.Common;
using System.Collections.Generic;
namespace Apps.IBLL
{
   public partial interface IWebpartBLL
    {
        SysUserConfig GetByIdAndUserId(string id, string userId);
        List<P_Sys_WebPart_Result> GetPartData3(int top, string userId);
        //System.Collections.Generic.List<P_Mis_FileGetMyReadFile_Result> GetPartData8(int top, string userId);
        bool SaveHtml(ref ValidationErrors errors, string userId, string html);
    }
}
