using FinancialBot.Core.Jwt;
using aChatBotApi.Models;
using FinancialBot.Core;
using FinancialBot.Core.IoC;
using FinancialBot.Core.Repository;
using FinancialBot.Domain.Contexts;
using FinancialBot.Domain.Entities;
using FinancialBot.Domain.Repositories;
using FinancialBot.Services.Hubs;
using FinancialBot.Services.Mqt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aChatBotApi
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

            services.AddOptions();
            services.AddControllersWithViews();

            // services.AddDbContext<BaseDBContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("aChatBotApi")));
            // services.AddDbContext<BaseDBContext>(opt => opt.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()), ServiceLifetime.Scoped, ServiceLifetime.Scoped);
            services.AddDbContext<BaseDBContext>(opt => opt.UseSqlite(Configuration.GetConnectionString("SQLite"), b => b.MigrationsAssembly("WebChatApi")));
            services.AddIdentity<Users, IdentityRole>().AddEntityFrameworkStores<BaseDBContext>();
            services.AddSignalR(opt => { opt.ClientTimeoutInterval = TimeSpan.FromMinutes(60); opt.KeepAliveInterval = TimeSpan.FromMinutes(30); }).AddJsonProtocol();
   
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "Spa/dist";
            });

            #region CORS
            var securitySettings = Configuration.GetSection("SecurityConfig").Get<SecurityConfig>();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder=> builder.WithOrigins(securitySettings.CORSClientUrls));
                options.AddDefaultPolicy(builder => builder.WithOrigins("http://localhost:4200"));
            
                options.AddPolicy("AllowAllPolicy",
                    builder =>
                    {
                        builder
                        .WithOrigins("http://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod().AllowCredentials();
                            
                    });
            });
            #endregion

            #region IoC Registry
            services.AddCoreRegistry(Configuration);  
            #endregion
             
            RegisterServices(services);  
            
        }
         
        private void RegisterServices(IServiceCollection services)
        { 
            services.AddSingleton<IMqtMessageSender, MqtMessageSender>();
            services.AddHostedService<MqtMessageReceiverServices>();
        }  
         
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
          //  app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseCors("AllowAllPolicy"); 

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<HubChatServices>("/hub/chat");
            });
          
            app.UseSpa(spa =>
            { 
                spa.Options.SourcePath = "Spa";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
