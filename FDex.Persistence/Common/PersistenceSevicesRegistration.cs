using System;
using FDex.Application.Contracts.Persistence;
using FDex.Persistence.DbContexts;
using FDex.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Persistence.Common
{
    public static class PersistenceSevicesRegistration
    {
        public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FDexDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("FDexDB"), sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null
            );
                });
            },
            ServiceLifetime.Singleton);
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddSingleton<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ISwapRepository, SwapRepository>();
            return services;
        }
    }
}