using FinancialBot.Core.Repository;
using FinancialBot.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

 
using FinancialBot.Core;
using FinancialBot.Core.IoC;
using FinancialBot.Core.Repository;
using FinancialBot.Domain.Contexts;
using FinancialBot.Domain.Entities;
using FinancialBot.Domain.Repositories; 
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
using FinancialBot.Core.Jwt;

namespace FinancialBot.Core.IoC
{
    public static class CoreRegistrer
    {
        public static void AddCoreRegistry(this IServiceCollection services, IConfiguration Configuration)
        {

			addJWTConfig(services, Configuration);
			addRabbitMqt(services, Configuration);
			RegisterServices(services, Configuration);
		}
        private static void addJWTConfig(this IServiceCollection services, IConfiguration Configuration)
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
					ValidateAudience = false,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwtSettings.GetSection("vIssuer").Value,
					ValidAudience = jwtSettings.GetSection("Audience").Value,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.GetSection("sKey").Value))
				};
			});
		}
        private static void RegisterServices(IServiceCollection services, IConfiguration Configuration)
        {
			services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
			services.AddScoped<JwtClaim>();

		}

        private static void addRabbitMqt(IServiceCollection services, IConfiguration Configuration)
        {
			var serviceClientSettingsConfig = Configuration.GetSection("MQTRabbit");
			services.Configure<RMqtConfiguration>(serviceClientSettingsConfig);
		}
  
		 


	}
}
