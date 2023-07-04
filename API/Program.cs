using Serilog;

namespace API;

public static class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Async(wt => wt.Console())
            .CreateLogger();

        try
        {
            Log.Information("Starting web host");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host Terminated Unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }



    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services))
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls("https://localhost:5201");
                webBuilder.UseStartup<Startup>();
            });
}
