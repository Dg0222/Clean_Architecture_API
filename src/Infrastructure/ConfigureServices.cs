using Amazon.S3;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure.Health;
using CleanArchitecture.Infrastructure.Identity;
using CleanArchitecture.Infrastructure.Jobs;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Persistence.Interceptors;
using CleanArchitecture.Infrastructure.Services;
using CleanArchitecture.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Quartz;

namespace CleanArchitecture.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<S3Settings>(configuration.GetSection("S3"));

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        #region MSSQL
        //services.AddDbContext<ApplicationDbContext>(options =>
        //    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
        //        builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        #endregion

        #region MYSQL
        services.AddDbContext<ApplicationDbContext>(options =>
             options.UseMySql(configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(
            int.Parse(configuration.GetSection("MySQLVersion").GetSection("Major").Value ?? string.Empty),
            int.Parse(configuration.GetSection("MySQLVersion").GetSection("Minor").Value ?? string.Empty),
            int.Parse(configuration.GetSection("MySQLVersion").GetSection("Build").Value ?? string.Empty)
        ))));
        #endregion

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services
            .AddDefaultIdentity<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<IS3Service, S3Service>();
        services.AddTransient<IHttpClientService, HttpClientService>();
        services.AddTransient<IEmailService, EmailService>();

        services.AddIdentityServer()
            .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddIdentityServerJwt()
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
            });

        services.AddDataProtection().SetApplicationName("CleanArchitecture");

        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonS3>();

        services.AddHttpClient("TestAPI", client =>
        {
            client.BaseAddress = new Uri(configuration["TestAPI:ApiUrl"] ?? string.Empty);

        }).AddTransientHttpErrorPolicy(policyBuilder =>
            policyBuilder.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5)));

        services.AddScoped<ApplicationDbContextInitializer>();

        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("Database");

        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            // Just use the name of your job that you created in the Jobs folder.
            var jobKey = new JobKey("BackupDbJob");
            q.AddJob<BackupDbJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("BackupDbJob-trigger")
                //This Cron interval can be described as "run every minute" (when second is zero)
                .WithCronSchedule("0 0 23 ? * *") //runs every day at 11pm
            );
        });
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        return services;
    }
}
