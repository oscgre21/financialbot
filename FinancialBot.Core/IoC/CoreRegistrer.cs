using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialBot.Core.IoC
{
    public static class CoreRegistrer
    {
        public static void AddCoreRegistry(this IServiceCollection services, IConfiguration Configuration)
        {
            

        }
        private static void addJWTConfig(this IServiceCollection services, IConfiguration Configuration)
        {
            
        }
        private static void RegisterServices(IServiceCollection services, IConfiguration Configuration)
        {
   
        }

        private static void addRabbitMqt(IServiceCollection services, IConfiguration Configuration)
        {
         
        }

    }
}
