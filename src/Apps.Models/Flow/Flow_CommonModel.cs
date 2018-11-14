using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Models.Flow
{
    public partial class FlowStateCountModel
    {
        public int requestCount { get; set; }
        public int passCount { get; set; }

        public int rejectCount { get; set; }

        public int closedCount { get; set; }

        public int processCount { get; set; }
    }
}
