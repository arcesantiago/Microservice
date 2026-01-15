using Microservice.Application.Contracts.Infrastructure;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.Contracts.Persistence.Read;
using Microservice.Application.Contracts.Persistence.Write;
using Microservice.Infrastructure.Cache;
using Microservice.Infrastructure.Percistence;
using Microservice.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ExampleDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                            .LogTo(Console.WriteLine, [DbLoggerCategory.Database.Command.Name], Microsoft.Extensions.Logging.LogLevel.Information)
                            .EnableSensitiveDataLogging());

            services.AddMemoryCache();
            services.AddScoped<ICacheService, MemoryCacheService>();

            services.AddScoped(typeof(IReadRepository<>), typeof(RepositoryBase<>));
            services.AddScoped(typeof(IWriteRepository<>), typeof(RepositoryBase<>));
            services.AddScoped(typeof(IQueryRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<IExampleRepository, ExampleRepository>();
            services.AddScoped<IExampleUnitOfWork, ExampleUnitOfWork>();

            return services;
        }
    }
}