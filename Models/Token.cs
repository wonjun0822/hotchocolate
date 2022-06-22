using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace core_graph_v2.Models
{
    public class Token
    {
        public string ip { get; set; }
        public string port { get; set; }
        public string id { get; set; }
        public string token { get; set; }
        public DateTime expiration { get; set; }
    }

    public record ReturnToken(string accessToken, string refreshToken);
}
