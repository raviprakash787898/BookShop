using BookShop.ApiGateway.Contracts;
using BookShop.ApiGateway.Models;
using BookShop.ApiGateway.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BookShop.ApiGateway.Providers
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Registering the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration config)
        {
            string ValidIssuerAndAudience = config["AppSettings:JwtToken:Issuer"];
            string IssuerSigningKey = config["AppSettings:JwtToken:Key"].ToString();

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = ValidIssuerAndAudience,
                    ValidAudience = ValidIssuerAndAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(IssuerSigningKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });


            // Registering contracts
            services.AddScoped<IContractManager, ContractManager>();

            return services;
        }
    }
}
