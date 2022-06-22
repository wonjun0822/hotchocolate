using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.Types;

using core_graph_v2.Data;
using core_graph_v2.Type;
using core_graph_v2.Services;
using core_graph_v2.Schema;

using System.Text;
using System.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace core_graph_v2
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("CommandConStr")),
                ServiceLifetime.Transient
            );

            services.AddDbContextFactory<AppDbContext>(lifetime: ServiceLifetime.Transient);
            services.AddTransient<Service>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = Configuration.GetSection("TokenSettings").GetValue<string>("Audience"),
                    ValidIssuer = Configuration.GetSection("TokenSettings").GetValue<string>("Issuer"),
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Configuration.GetSection("TokenSettings").GetValue<string>("Key"))),
                    RequireExpirationTime = true
                };
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
            });

            services.AddAuthorization();

            services
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .AddMutationType<Mutation>()
                .AddSubscriptionType<SubScription>()
                .AddType<ActionType>()
                .AddType<ActionCmtType>()
                .AddType<UploadType>()
                .AddAuthorization()
                .AddSorting()
                .AddInMemorySubscriptions()
                .AddHttpRequestInterceptor(
                    (context, executor, builder, ct) =>
                    {
                        context.Request.Headers.TryGetValue("Authorization", out var tmp);

                        string token = tmp.FirstOrDefault(x => x.StartsWith("Bearer", StringComparison.InvariantCultureIgnoreCase))?.Substring("Bearer".Length).Trim();

                        if (!string.IsNullOrEmpty(token))
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
                                RequireExpirationTime = true,
                            };

                            try
                            {
                                handler.ValidateToken(token, tokenValidation, out SecurityToken securityToken);
                            }

                            catch (SecurityTokenExpiredException)
                            {
                                throw new GraphQLRequestException(
                                    ErrorBuilder.New()
                                    .SetMessage("Invalid Token")
                                    .SetExtension("code", "EXPIRED_TOKEN")
                                    .Build()
                                );
                            }
                        }

                        return new System.Threading.Tasks.ValueTask();
                    }
                );

            services.AddHttpContextAccessor();

            //services.AddSha256DocumentHashProvider();

            //services.AddErrorFilter<ErrorFilter>();

            services.AddCors(options =>
            {
                options.AddPolicy("cors",
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebSockets();
            app.UseRouting();
            app.UseCors("cors");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL("/api");

                endpoints.MapBananaCakePop("/ui").WithOptions(new GraphQLToolOptions
                {
                    Enable = env.IsDevelopment(),
                    UseBrowserUrlAsGraphQLEndpoint = true
                });
            });
        }
    }
}
