using core_graph_v2.Data;
using core_graph_v2.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Action = core_graph_v2.Models.Action;

namespace core_graph_v2.Services
{
    public class Service
    {
        private readonly AppDbContext _dbContext;
          
        public Service(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Action> GetAction()
        {
            return _dbContext.Action;
        }

        public IQueryable<ActionCmt> GetActionCmt()
        {
            return _dbContext.ActionCmt;
        }

        public IQueryable<ClashCheck> GetClashSummary(string jobNo)
        {
            List<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@JobNo", Value = jobNo },
                new SqlParameter { ParameterName = "@ProjFileId", Value = 0 }
            };

            return _dbContext.ClashCheck.FromSqlRaw<ClashCheck>("exec usp_ClashCheck_FindBySearch @JobNo, @ProjFileId", param.ToArray()).ToList().AsQueryable();
        }

        //public void Login(string role)
        //{
        //    var claims = new List<Claim>();

        //    claims.Add(new Claim(ClaimTypes.Role, role));

        //    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //    var principal = new ClaimsPrincipal(identity);

        //    var props = new AuthenticationProperties();

        //    HttpContext context = null;
            
        //    context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props).Wait();
        //}
    }
}