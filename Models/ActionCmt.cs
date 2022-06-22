
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

using HotChocolate.AspNetCore.Authorization;

namespace core_graph_v2.Models
{
    [Authorize]
    public class ActionCmt
    {
        [Key]
        public int ActionCmtId { get; set; }

        public int ActionId { get; set; }
        public string Type { get; set; }
        public string CView { get; set; }
    }

    public class ActionCmtParameter
    {
        public int? ActionId { get; set; }
        public string Type { get; set; }
        public string CView { get; set; }
    }
}