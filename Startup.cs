using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutomateBussiness.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using AutomateBussiness.Models;
using AutomateBussiness.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Azure.SignalR;
namespace AutomateBussiness
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
            services.AddCors();
            

            var appSettingSection = Configuration.GetSection("AppSettings");
            var appSettings = appSettingSection.Get<AppSettings>();
            //AppSetting is Model
            services.Configure<AppSettings>(appSettingSection);
    
            services.AddDbContext<AutomateBussinessContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            //services.AddAuthorization(options =>
            //{
            //    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
            //        CookieAuthenticationDefaults.AuthenticationScheme,
            //        JwtBearerDefaults.AuthenticationScheme);
            //    defaultAuthorizationPolicyBuilder =
            //        defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
            //    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            //});


            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF32.GetBytes(Configuration["Jwt:Key"])),
                        ClockSkew = TimeSpan.Zero
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/AutomateHub")))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                               
                });

            services.AddIdentity<AccountViewModel, IdentityRole>(options => {
                //password option >>> services.Configure<IdentityOptions>(options =>
                options.Password.RequiredLength = 7;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireNonAlphanumeric = true;

            })
                .AddEntityFrameworkStores<AutomateBussinessContext>()
                .AddDefaultTokenProviders();


            //services.ConfigureApplicationCookie(options =>
            //{
            //    //options.LoginPath = "/login";
            //    //options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            //});

            services.AddControllersWithViews();
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.AllowTrailingCommas = true;

                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });
            services.AddSignalR();//.AddAzureSignalR("Endpoint=https://automatesignalr.service.signalr.net;AccessKey=gf+uRQ4y1wlpaGqFYOBb71BjiJYP+Hu/JHBL+JociAk=;Version=1.0;");
            //.AddMessagePackProtocol(options =>
            //{
            //    options.FormatterResolvers = new List<MessagePack.IFormatterResolver>()
            //    {
            //        MessagePack.Resolvers.StandardResolver.Instance
            //    };
            //});

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //policy 
            app.UseCors(x =>
                x.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
              
            app.UseAuthentication();

            //app.Use(async (context, next) =>
            //{
            //    await next();
            //    var bearerAuth = context.Request.Headers["Authorization"]
            //        .FirstOrDefault()?.StartsWith("Bearer ") ?? false;
            //    if (context.Response.StatusCode == 401
            //        && !context.User.Identity.IsAuthenticated
            //        && !bearerAuth)
            //    {
            //        await context.ChallengeAsync("oidc");
            //    }
            //});


            app.UseAuthorization();
            //app.UseAzureSignalR(buider =>
            //  {
            //      buider.MapHub<AutomateHub>("/AutomateHub");
            //  });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<AutomateHub>("/AutomateHub");
                //endpoints.MapAzureSignalR(this.GetType().FullName);
                //endpoints.MapHub<StreamHub>("/streamHub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
           
        }
    }
}
