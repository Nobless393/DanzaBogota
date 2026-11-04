using DanzaBogota.Domain.Entity.Identities;
using DanzaBogota.Infrastructure.Data;
using DanzaBogota.Infrastructure.Data.Seed;
using DanzaBogota.Services.WebApi;
using DanzaBogota.Services.WebApi.SwaggerExtensions;
using DanzaBogota.Transversal.Common;
using DanzaBogota.Application.Main;
using DanzaBogota.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureDataServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);            
builder.Services.AddTransversalCommonServices();
builder.Services.AddApplicationMainServices();
builder.Services.AddInfrastructureRepositoryServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

var app = builder.Build();

app.UseStaticFiles();

// Sembrar roles al iniciar
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Sembrar roles y usuario administrador
    await IdentitySeeder.SeedRolesAsync(roleManager);
    await UserSeeder.SeedAdminUserAsync(userManager, app.Configuration);
    await LocalitySeeder.SeedLocalityAsync(dbContext);
    await EventTypeSeeder.SeedAsync(dbContext);
    await CategorySeeder.SeedAsync(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerConfiguration();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();