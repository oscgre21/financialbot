using aChatBotApi.Jwt;
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
            //services.AddControllers();
            services.AddDbContext<BaseDBContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("aChatBotApi")));
            services.AddIdentity<Users, IdentityRole>().AddEntityFrameworkStores<BaseDBContext>();
            services.AddSignalR(opt => { opt.ClientTimeoutInterval = TimeSpan.FromMinutes(60); opt.KeepAliveInterval = TimeSpan.FromMinutes(30); }).AddJsonProtocol();
   
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "Spa/dist";
            });

            #region IoC Registry
            services.AddCoreRegistry(Configuration);  
            #endregion

            addJWTConfig(services);
            addRabbitMqt(services);
            RegisterServices(services);  
            
        }

        private void addJWTConfig(IServiceCollection services)
        {
            var jwtSettings = Configuration.GetSection("JWT");
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true, 
                    ValidIssuer = jwtSettings.GetSection("vIssuer").Value,
                    ValidAudience = jwtSettings.GetSection("Audience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.GetSection("vIssuer").Value))
                };
            });
        }
        private void RegisterServices(IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            services.AddScoped<JwtClaim>();

            services.AddSingleton<IMqtMessageSender, MqtMessageSender>();
            services.AddHostedService<MqtMessageReceiverServices>();
        }

        private void addRabbitMqt(IServiceCollection services)
        {
            var serviceClientSettingsConfig = Configuration.GetSection("MQTRabbit");
            services.Configure<RMqtConfiguration>(serviceClientSettingsConfig);
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
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<HubChatServices>("/hub/chat");
            });
            /*
            app.UseSpa(spa =>
            { 
                spa.Options.SourcePath = "Spa";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });*/
        }
    }
}
