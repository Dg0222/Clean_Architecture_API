using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities.Identity;
using CleanArchitecture.Infrastructure.Identity;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Persistence.Interceptors;
using CleanArchitecture.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        #region MSSQL
        //services.AddDbContext<ApplicationDbContext>(options =>
        //    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
        //        builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        #endregion

        #region MYSQL
        services.AddDbContext<ApplicationDbContext>(options =>
             options.UseMySql(configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(
            int.Parse(configuration.GetSection("MySQLVersion").GetSection("Major").Value),
            int.Parse(configuration.GetSection("MySQLVersion").GetSection("Minor").Value),
            int.Parse(configuration.GetSection("MySQLVersion").GetSection("Build").Value)
        ))));
        #endregion

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services
            .AddDefaultIdentity<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddTransient<IIdentityService, IdentityService>();

        services.AddIdentityServer()
            .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = configuration.GetSection("IdentityAuthority").GetSection("Authority").Value;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/hubs/test")))
                    {
                        context.Token = accessToken;
                        //context.Request.Headers.Add("", new[] { $"Bearer {accessToken}" });
                    }
                    return Task.CompletedTask;
                }
            };
        })
        .AddIdentityServerJwt();

        services.AddDataProtection().SetApplicationName("CleanArchitecture");

        return services;
    }
}
