using aChatBotApi;
using AutoMapper;
using FinancialBot.Core.IoC;
using FinancialBot.Core.Jwt;
using FinancialBot.Domain.Contexts;
using FinancialBot.Domain.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialBot.UnitTest
{
    
    internal static class SetupSetting
    {

		public static readonly UserManager<Users> userManager;
		public static readonly IMapper mapper;
		public static readonly JwtClaim jwt;
		static SetupSetting()
        {
            IConfiguration appSettings = null;
            Startup startup = null;
            IServiceCollection serviceCollection = null;

            var service = WebHost
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.Sources.Clear();
                    config.AddConfiguration(hostingContext.Configuration);
                    config.AddJsonFile("appsettings.json");
                    appSettings = config.Build();


                }).ConfigureServices(sc =>
                {
					sc.AddDbContext<BaseDBContext>(opt => opt.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()), ServiceLifetime.Scoped, ServiceLifetime.Scoped);
					sc.AddIdentity<Users, IdentityRole>().AddEntityFrameworkStores<BaseDBContext>();
					sc.AddCoreRegistry(appSettings);
					sc.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
                    serviceCollection = sc;
                })
            .UseStartup<EmptyStartup>()
            .Build();

			userManager = service.Services.GetService(typeof(UserManager<Users>)) as UserManager<Users>;
            mapper = service.Services.GetService(typeof(IMapper)) as IMapper;
			jwt = service.Services.GetService(typeof(JwtClaim)) as JwtClaim; 
		}
       
    }

    public class EmptyStartup
    {
        public EmptyStartup(IConfiguration _) { }

        public void ConfigureServices(IServiceCollection _) { }

        public void Configure(IApplicationBuilder _) { }
    }
}
