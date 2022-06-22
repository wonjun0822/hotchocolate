using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace core_graph_v2.Models
{
    public class ClashCheck
    {
        [Key]
        public string CheckItem { get; set; }

        public int ClashesCnt { get; set; }
        public int UndefinedCnt { get; set; }
        public int EditCnt { get; set; }
        public int NoneCnt { get; set; }
        public int SevereCnt { get; set; }
        public int OptionalCnt { get; set; }
        public int BadPartCnt { get; set; }
        public int DelayedPartCnt { get; set; }

        public string LastUpdateDt { get; set; }
    }
}
