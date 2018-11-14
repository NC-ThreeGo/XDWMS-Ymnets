using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Common
{
    ///<summary>
    ///Easyui列头过滤比较操作符
    ///</summary>
    public enum DataFliterOperatorTypeEnum
    {
        ///<summary> 
        ///等于
        ///</summary> 
        equal= 0, 
        ///<summary> 
        ///小于等于
        ///</summary> 
        lessorequal = 1,
        ///<summary> 
        ///小于
        ///</summary> 
        less = 2,
        ///<summary> 
        ///大于
        ///</summary> 
        greater = 3,
        ///<summary>
        ///大于等于
        ///</summary>
        greaterorequal = 4,
        ///<summary> 
        ///不等于
        ///</summary> 
        notequal = 5,
        ///<summary> 
        ///包含
        ///</summary> 
        contains = 6
    }
}
