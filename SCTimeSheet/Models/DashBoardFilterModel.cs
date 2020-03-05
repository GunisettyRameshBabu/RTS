using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCTimeSheet.Models
{
    public class DashBoardFilterModel
    {
        public long? GrantId { get; set; }

        public long? ProjectId { get; set; }

        public long[] projects { get; set; }

        public bool Filtered { get; set; }
    }
}