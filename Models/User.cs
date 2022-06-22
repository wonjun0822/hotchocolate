using System.ComponentModel.DataAnnotations;

namespace core_graph_v2.Models
{
    public class User
    {
        [Key]
        public int Idx {get; set;}

        [Required]
        public string Id { get; set; }
        public string Password { get; set; }
    }
}
