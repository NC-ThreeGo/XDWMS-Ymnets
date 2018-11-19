using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Common
{
    public enum SequenceType
    {
        [Description("常量")]
        Constant = 1,

        [Description("GUID")]
        Guid = 2,

        [Description("自定义时间")]
        CustomerTime = 3,

        [Description("流水号")]
        Sequence = 4,

        [Description("每日流水号")]
        SequenceOfDay = 5
    }
}
