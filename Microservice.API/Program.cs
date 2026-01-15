using Microservice.API;
using Microservice.API.Middleware;
using Microservice.Application;
using Microservice.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Registrar servicios de aplicaci�n e infraestructura
builder.Services.AddApiServices(builder.Configuration, builder.Environment);
builder.Services.AddAplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.RoutePrefix = string.Empty;
        c.DisplayRequestDuration();
    });
}

//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<DbContext>();

//    if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Testing")
//    {
//        await db.Database.EnsureDeletedAsync();
//        await db.Database.EnsureCreatedAsync();
//    }
//    else
//        await db.Database.MigrateAsync();

//    DbInitializer.Seed(db);
//}

app.UseCors("DefaultCors");
app.UseMiddleware<ExceptionMiddleware>();
app.MapHealthChecks("/health");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();