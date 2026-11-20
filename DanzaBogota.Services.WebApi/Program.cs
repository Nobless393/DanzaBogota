using DanzaBogota.Domain.Entity.Identities;
using DanzaBogota.Infrastructure.Data;
using DanzaBogota.Infrastructure.Data.Seed;
using DanzaBogota.Services.WebApi;
using DanzaBogota.Services.WebApi.Filters;
using DanzaBogota.Services.WebApi.Middleware;
using DanzaBogota.Services.WebApi.SwaggerExtensions;
using DanzaBogota.Transversal.Common;
using DanzaBogota.Application.Main;
using DanzaBogota.Infrastructure.Repository;
using DanzaBogota.Application.Validator;
using DanzaBogota.Transversal.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = DanzaBogota.Transversal.Logging.ConfigureServices.CreateSerilogLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddInfrastructureDataServices(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddCorsConfiguration(builder.Configuration);
    builder.Services.AddTransversalCommonServices();
    builder.Services.AddApplicationMainServices();
    builder.Services.AddInfrastructureRepositoryServices();
    builder.Services.AddValidationServices();

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidationFilter>();
    }).ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwagger();

    var app = builder.Build();

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseStaticFiles();

    app.UseCorsConfiguration();

    // Sembrar datos de referencia al iniciar
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync();

        await IdentitySeeder.SeedRolesAsync(roleManager);
        await UserSeeder.SeedAdminUserAsync(userManager);
        await LocalitySeeder.SeedLocalityAsync(dbContext);
        await EventTypeSeeder.SeedAsync(dbContext);
        await CategorySeeder.SeedAsync(dbContext);

        if (app.Environment.IsDevelopment())
        {
            await TestDataSeeder.SeedAsync(dbContext, userManager, roleManager);
        }
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerConfiguration();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}
