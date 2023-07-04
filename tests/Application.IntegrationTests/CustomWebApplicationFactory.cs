using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;

namespace CleanArchitecture.Application.IntegrationTests;

using static Testing;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            var integrationConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            configurationBuilder.AddConfiguration(integrationConfig);
        });

        builder.ConfigureServices((builder, services) =>
        {
            services
                .Remove<ICurrentUserService>()
                .AddTransient(provider => Mock.Of<ICurrentUserService>(s =>
                    s.Id == GetUserId()));

            services
                .Remove<DbContextOptions<ApplicationDbContext>>()
                //.AddDbContext<ApplicationDbContext>((sp, options) =>
                //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                //        builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), serverVersion: new MySqlServerVersion(new Version(
                            int.Parse(builder.Configuration.GetSection("MySQLVersion").GetSection("Major").Value ?? string.Empty),
                            int.Parse(builder.Configuration.GetSection("MySQLVersion").GetSection("Minor").Value ?? string.Empty),
                            int.Parse(builder.Configuration.GetSection("MySQLVersion").GetSection("Build").Value ?? string.Empty))),
                             builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        });
    }
}
