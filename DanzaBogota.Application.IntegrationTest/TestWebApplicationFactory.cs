using DanzaBogota.Infrastructure.Data;
using DanzaBogota.Infrastructure.Data.Seed;
using DanzaBogota.Domain.Entity.Identities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

namespace DanzaBogota.Application.IntegrationTest
{
    /// <summary>
    /// Fábrica de aplicación de prueba que reemplaza SQL Server por SQLite en memoria.
    /// Cada instancia tiene su propia base de datos aislada.
    /// </summary>
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            builder.ConfigureServices(services =>
            {
                // Reemplazar el DbContext de SQL Server por SQLite en memoria
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite($"DataSource={_dbName};Mode=Memory;Cache=Shared"));

                // Configurar JWT con clave de prueba
                services.Configure<Microsoft.IdentityModel.Tokens.TokenValidationParameters>(_ => { });
            });

            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = $"DataSource={_dbName};Mode=Memory;Cache=Shared",
                    ["Jwt:Key"]       = "ClaveSecretaDePruebaMuyLargaParaTest123!",
                    ["Jwt:Issuer"]    = "DanzaBogotaApi",
                    ["Jwt:Audience"]  = "DanzaBogotaClient",
                    ["Jwt:ExpirationMinutes"] = "60"
                });
            });
        }

        /// <summary>
        /// Inicializa la BD y siembra roles + usuario admin de prueba.
        /// Debe llamarse una sola vez antes de los tests.
        /// </summary>
        public async Task InitializeDatabaseAsync()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.EnsureCreatedAsync();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            await IdentitySeeder.SeedRolesAsync(roleManager);
            await UserSeeder.SeedAdminUserAsync(userManager);
            await LocalitySeeder.SeedLocalityAsync(db);
            await EventTypeSeeder.SeedAsync(db);
            await CategorySeeder.SeedAsync(db);
            await TestDataSeeder.SeedAsync(db, userManager, roleManager);
        }
    }
}
