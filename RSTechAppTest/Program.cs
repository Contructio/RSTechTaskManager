using Avalonia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RSTechAppTest.Services;
using RSTechAppTest.ViewModels;
using RSTechAppTest.Views;
using System;

namespace RSTechAppTest;

class Program
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    [STAThread]
    public static void Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // Считываем ini-файл
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddIniFile("config.ini", optional: false, reloadOnChange: true)
            .Build();

        var connectionStringBuilder = new Npgsql.NpgsqlConnectionStringBuilder
        {
            Host = configuration["Database:Host"] ?? "localhost",
            Port = int.TryParse(configuration["Database:Port"], out var port) ? port : 5432,
            Database = configuration["Database:Database"] ?? "rtech_tasks_db",
            Username = configuration["Database:Username"] ?? "postgres",
            Password = configuration["Database:Password"] ?? string.Empty
        };

        string connectionString = connectionStringBuilder.ConnectionString;

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure();
                npgsqlOptions.CommandTimeout(60);
            }),
            ServiceLifetime.Scoped);

        services.AddScoped<ITaskRepository, PostgresTaskRepository>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();
    }


}
