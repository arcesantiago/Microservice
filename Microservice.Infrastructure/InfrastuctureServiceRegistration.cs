using Microservice.Application.Contracts.Infrastructure;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Infrastructure.Cache;
using Microservice.Infrastructure.Percistence;
using Microservice.Infrastructure.Repositories.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Database
            services.AddDbContext<ExampleDbContext>(options => 
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                    .LogTo(Console.WriteLine, [DbLoggerCategory.Database.Command.Name], 
                        Microsoft.Extensions.Logging.LogLevel.Information)
                    .EnableSensitiveDataLogging());

            // Caching
            services.AddMemoryCache();
            services.AddScoped<ICacheService, MemoryCacheService>();

            // LINQ Repositories (EF)
            services.AddScoped(typeof(IReadRepository<>), typeof(LINQRepository<>));
            services.AddScoped(typeof(IWriteRepository<>), typeof(LINQRepository<>));
            services.AddScoped(typeof(IQueryRepository<>), typeof(LINQRepository<>));

            // SQL Raw Repositories (.NET 10 + C# 14)
            // Separamos la implementación SQL raw en SqlRepository<>
            services.AddScoped(typeof(ISqlQueryRepository<>), typeof(SqlRepository<>));
            services.AddScoped(typeof(ISqlCommandRepository<>), typeof(SqlRepository<>));
            services.AddScoped(typeof(ISqlRepository<>), typeof(SqlRepository<>));

            // Custom repositories / UnitOfWork
            services.AddScoped<IUnitOfWork>(sp =>
                sp.GetRequiredService<ExampleDbContext>());

            return services;
        }
    }
}