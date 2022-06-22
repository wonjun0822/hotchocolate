using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

using HotChocolate.Types;
using HotChocolate.AspNetCore.Authorization;

namespace core_graph_v2.Models
{
    public class Action
    {
        [Key]
        public int ActionId { get; set; }

        public int ActionGroupId { get; set; }

        public string ActionNo { get; set; }

        public ICollection<ActionCmt> ActionCmt { get; set; } = new List<ActionCmt>();
    }

    public class ActionParameter
    {
        //[GraphQLName("GroupId")]
        public int ActionGroupId { get; set; }

        public string ActionNo { get; set; }

        public ActionCmtParameter ActionCmtParameter { get; set; }

        public List<IFile> Files { get; set; }
    }

    public record CreateAction(
        ActionParameter parameter
    );

    public record ReturnAction(Action action);
}