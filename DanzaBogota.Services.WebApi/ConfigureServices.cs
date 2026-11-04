using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DanzaBogota.Services.WebApi
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtKey = configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
            {
                throw new InvalidOperationException(
                    "Falta 'Jwt:Key' (mínimo 32 caracteres). Configúrala con: " +
                    "dotnet user-secrets set \"Jwt:Key\" \"<clave-larga-aleatoria>\" " +
                    "o con la variable de entorno Jwt__Key.");
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DanzaBogota API v1");
            });

            return app;
        }
    }
}
