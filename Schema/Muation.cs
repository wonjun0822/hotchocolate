using core_graph_v2.Data;
using core_graph_v2.Models;

using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.Subscriptions;
using HotChocolate.Types;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

using Action = core_graph_v2.Models.Action;

namespace core_graph_v2.Schema
{
    public class Mutation
    {
        private readonly IConfiguration Configuration;

        public Mutation(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public ReturnToken Login(string id, [Service] IHttpContextAccessor httpContextAccessor, [Service] AppDbContext dbContext)
        {
            if (dbContext.User.Where(x => x.Id == id).FirstOrDefault() != null) {
                var securitykey = new SymmetricSecurityKey(Convert.FromBase64String(Configuration.GetSection("TokenSettings").GetValue<string>("Key")));
                var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

                var jwtToken = new JwtSecurityToken(
                    issuer: Configuration.GetSection("TokenSettings").GetValue<string>("Issuer"),
                    audience: Configuration.GetSection("TokenSettings").GetValue<string>("Audience"),
                    expires: DateTime.Now.AddMinutes(3),
                    signingCredentials: credentials,
                    claims: new Claim[] { 
                        new Claim(ClaimTypes.NameIdentifier, id),
                        new Claim(ClaimTypes.Role, "a"),
                        new Claim(ClaimTypes.Role, "b")
                    }
                );

                var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                var refreshToken = GenerateRefreshToken();

                string ip = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                string port = httpContextAccessor.HttpContext.Connection.RemotePort.ToString();

                var token = dbContext.Token.Where(x => x.ip == ip && x.port == port && x.id == id).FirstOrDefault();

                if (token == null)
                {
                    token = new Token
                    {
                        ip = ip,
                        port = port,
                        id = id,
                        token = refreshToken,
                        expiration = DateTime.Now.AddDays(7)
                    };

                    dbContext.Token.Add(token);
                }

                else
                {
                    token.token = refreshToken;
                    token.expiration = DateTime.Now.AddDays(7);
                }

                dbContext.SaveChanges();

                return new ReturnToken(accessToken, refreshToken);
            }

            else
            {
                throw new GraphQLRequestException(
                    ErrorBuilder.New()
                    .SetMessage("User information not found")
                    .SetExtension("code", "NOT_FOUND_USER")
                    .Build()
                );
            }
        }

        public ReturnToken ReissuanceToken(string accessToken, string refreshToken, [Service] IHttpContextAccessor httpContextAccessor, [Service] AppDbContext dbContext)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenValidation = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = Configuration.GetSection("TokenSettings").GetValue<string>("Audience"),
                ValidIssuer = Configuration.GetSection("TokenSettings").GetValue<string>("Issuer"),
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Configuration.GetSection("TokenSettings").GetValue<string>("Key"))),
                ValidateLifetime = false
            };
            
            ClaimsPrincipal claimsPrincipal = handler.ValidateToken(accessToken, tokenValidation, out SecurityToken securityToken);

            if (securityToken == null)
            {
                throw new GraphQLRequestException(
                    ErrorBuilder.New()
                    .SetMessage("Invalid Token")
                    .SetExtension("code", "INVALID_TOKEN")
                    .Build()
                );
            }

            else
            {
                string id = claimsPrincipal.Claims.ToList().Find(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                string ip = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                string port = httpContextAccessor.HttpContext.Connection.RemotePort.ToString();

                if (dbContext.Token.Where(x => x.ip == ip && x.port == port && x.id == id && x.token == refreshToken && x.expiration > DateTime.Now).Count() > 0)
                    return Login(id, httpContextAccessor, dbContext);

                else
                {
                    throw new GraphQLRequestException(
                        ErrorBuilder.New()
                        .SetMessage("Invalid Token")
                        .SetExtension("code", "INVALID_TOKEN")
                        .Build()
                    );
                }
            }
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomNumber);

                return Convert.ToBase64String(randomNumber);
            }
        }

        public bool CreateAction(Models.CreateAction param, [Service] AppDbContext dbContext, [Service] ITopicEventSender eventSender)
        {
            dbContext.Database.BeginTransaction();

            try
            {
                var action = new Action
                {
                    ActionGroupId = param.parameter.ActionGroupId,
                    ActionNo = param.parameter.ActionNo
                };

                dbContext.Action.Add(action);

                dbContext.SaveChanges();

                var actionCmt = new ActionCmt
                {
                    ActionId = action.ActionId,
                    Type = param.parameter.ActionCmtParameter.Type
                };

                dbContext.ActionCmt.Add(actionCmt);

                dbContext.SaveChanges();

                FileUpload(param.parameter.Files, "M");

                dbContext.Database.CommitTransaction();

                eventSender.SendAsync(nameof(SubScription.ActionAdded), action).GetAwaiter();

                return true;
            }

            catch
            {
                dbContext.Database.RollbackTransaction();

                return false;
            }

            //return new ReturnAction(action);
        }

        public string FileUpload(List<IFile> files, string type)
        {
            try
            {
                if (files.Count > 0)
                {
                    foreach (IFile file in files)
                    {
                        if (file != null)
                        {
                            using (Stream stream = file.OpenReadStream())
                            {
                                string path = @"E:\UploadFile\" + file.Name;

                                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                                {
                                    stream.CopyTo(fileStream);
                                }
                            }
                        }
                    }
                }

                return type;
            }

            catch
            {
                return "error";
            }
        }
    }
}