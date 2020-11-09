using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApi.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using AutoMapper;
using ChatApi.Common;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace ChatApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureEntityFramework(services);


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     SaveSigninToken = true,//保存token,后台验证token是否生效(重要)
                     ValidateIssuer = true,//是否验证Issuer
                     ValidateAudience = true,//是否验证Audience
                     ValidateLifetime = true,//是否验证失效时间
                     ValidateIssuerSigningKey = true,//是否验证SecurityKey
                     ValidAudience = AppConfig.Secret.Audience,//Audience
                     ValidIssuer = AppConfig.Secret.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfig.Secret.JWT))
                 };
                 options.Events = new JwtBearerEvents()
                 {
                     OnMessageReceived = (context) =>
                     {

                         if (!context.HttpContext.Request.Path.HasValue)
                         {
                             return Task.CompletedTask;
                         }
                         //重点在于这里；判断是Signalr的路径
                         var token = "";

                         var headAuthorization = context.HttpContext.Request.Headers["Authorization"];
                         if (headAuthorization.Count > 0)
                         {
                             token = headAuthorization.ToString().Replace("Bearer ", "").Trim();
                         }

                         if (!String.IsNullOrWhiteSpace(context.HttpContext.Request.Query["access_token"]))
                         {
                             token = context.HttpContext.Request.Query["access_token"].ToString();
                         }

                         if (!string.IsNullOrWhiteSpace(token))
                         {
                             var path = context.HttpContext.Request.Path;
                             if (!(string.IsNullOrWhiteSpace(token)) && path.StartsWithSegments("/chatHub"))
                             {
                                 context.Token = token;
                                 return Task.CompletedTask;
                             }
                         }

                         return Task.CompletedTask;

                     },
                     OnChallenge = context =>
                     {
                         context.HandleResponse();
                         context.Response.Clear();
                         context.Response.ContentType = "application/json";
                         context.Response.StatusCode = 401;

                         context.Response.WriteAsync(JsonConvert.SerializeObject(new { message = "授权未通过", status = false, code = 401 }));
                         return Task.CompletedTask;
                     }
                 };
             });


            //跨域
            string corsUrls = Configuration["CorsUrls"];
            if (string.IsNullOrEmpty(corsUrls))
            {
                throw new Exception("请配置跨请求的前端Url");
            }
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins(corsUrls.Split(",")).AllowCredentials()
                        .AllowAnyHeader().AllowAnyMethod();
                    });
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ChatApi", Version = "v1" });
                var security = new Dictionary<string, IEnumerable<string>>
                { { AppConfig.Secret.Issuer, new string[] { } }};
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT授权token前面需要加上字段Bearer与一个空格,如Bearer token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.ResolveConflictingActions(apidescreption => apidescreption.First());
                ////注释
                //var xmlCommentsPaths = GetXmlCommentsPaths();
                //foreach (var item in xmlCommentsPaths) c.IncludeXmlComments(item);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            })
               .AddControllers()
               .ConfigureApiBehaviorOptions(options =>
               {
                   options.SuppressConsumesConstraintForFormFileParameters = true;
                   options.SuppressInferBindingSourcesForParameters = true;
                   options.SuppressModelStateInvalidFilter = true;
                   options.SuppressMapClientErrors = true;
                   options.ClientErrorMapping[404].Link =
                       "https://*/404";
               });
            services.AddAutoMapper(typeof(Startup));
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles().UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true
            });
            app.UseDefaultFiles();
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(
            //    Path.Combine(Directory.GetCurrentDirectory(), @"Upload")),
            //    //配置访问虚拟目录时文件夹别名
            //    RequestPath = "/Upload",
            //    OnPrepareResponse = (Microsoft.AspNetCore.StaticFiles.StaticFileResponseContext staticFile) =>
            //    {
            //        //可以在此处读取请求的信息进行权限认证
            //        //  staticFile.File
            //        //  staticFile.Context.Response.StatusCode;
            //    }
            //});

            app.ConvertExtensionConfigure();


            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatApi");
            });

            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=ApiHome}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/chatHub");
            });
            //app.UseSignalR(routes =>
            //{
            //    routes.MapHub<ChatHub>("/chatHub");
            //});
            //GlobalHubServer.Init(service);
        }
        public static void ConfigureEntityFramework(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(
                options => options.UseMySql(GetConnectionString(),
                    mysqlOptions =>
                    {
                        mysqlOptions.MaxBatchSize(AppConfig.EfBatchSize);
                        //mysqlOptions.ServerVersion(AppConfig.ServerVersion);
                        if (AppConfig.EfRetryOnFailure > 0)
                        {
                            mysqlOptions.EnableRetryOnFailure(AppConfig.EfRetryOnFailure, TimeSpan.FromSeconds(5), null);
                        }
                    }
            ));
        }
        private static string GetConnectionString()
        {
            var csb = new MySqlConnectionStringBuilder(AppConfig.ConnectionString);

            if (AppConfig.EfDatabase != null)
            {
                csb.Database = AppConfig.EfDatabase;
            }

            return csb.ConnectionString;
        }

    }
}
