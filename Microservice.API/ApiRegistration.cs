using Microsoft.OpenApi;

namespace Microservice.API
{
    public static class ApiRegistration
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfigurationBuilder configuration, IWebHostEnvironment hostEnvironment)
        {
            // Add services to the container.
            services.AddControllers();

            // Configurar Swagger/OpenAPI
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API",
                    Version = "v1",
                    Description = "",
                    Contact = new OpenApiContact
                    {
                        Name = "",
                        Email = ""
                    }
                });

                // Incluir comentarios XML si los hay
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            configuration
            .AddJsonFile($"appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", optional: true);

            configuration.AddEnvironmentVariables();

            //if (hostEnvironment.IsDevelopment() && File.Exists("../.env.local"))
            //    configuration.AddDotNetEnv("../.env.local");

            //services.AddHealthChecks()
            //    .AddCheck<PostgresHealthCheck>("postgres");

            //if(hostEnvironment.IsDevelopment())
            //    services.AddHealthChecks()
            //    .AddCheck<RedisHealthCheck>("redis");

            services.AddCors(options =>
            {
                options.AddPolicy("DefaultCors", policy =>
                {
                    if (hostEnvironment.IsDevelopment() || hostEnvironment.EnvironmentName == "Testing")
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    }
                    else
                    {
                        policy.WithOrigins("http://")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    }
                });
            });

            return services;
        }
    }
}