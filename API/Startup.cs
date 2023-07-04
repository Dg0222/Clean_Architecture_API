﻿using Microsoft.AspNetCore.HttpOverrides;
using System.Text;
using API.Services;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.Hubs;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace API;

public class Startup
{
    readonly string _cors = string.Empty;
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplication();
        services.AddInfrastructure(Configuration);

        services.AddTransient<ICurrentUserService, CurrentUserService>();

        services.AddControllers();

        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
        });

        services.AddSignalR();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        services.AddCors(options =>
        {
            options.AddPolicy(name: _cors, builder =>
            {
                builder.WithOrigins(Configuration.GetSection("Cors").GetSection("URLs").GetChildren().Select(x => x.Value).ToArray())
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyHeader();

            });
        });

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clean Architecture API");
                c.RoutePrefix = string.Empty;
            });
        }

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseCors(_cors);

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseAuthentication();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<TestHub>("/hubs/test");
            endpoints.MapHealthChecks("/_health", new HealthCheckOptions()
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        });
    }
}
