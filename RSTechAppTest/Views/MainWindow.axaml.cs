using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RSTechAppTest.Services;
using RSTechAppTest.ViewModels;

namespace RSTechAppTest.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public MainWindow(MainViewModel viewModel) : this()
    {
        DataContext = viewModel;
        // Переносим применение миграций в асинхронный поток при открытии окна.
        // Это предотвращает deadlock графического интерфейса Avalonia при долгом ответе СУБД.
        Opened += async (s, e) =>
        {
            try
            {
                using (var scope = Program.ServiceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    await db.Database.MigrateAsync();
                }

                await viewModel.LoadTasksCommand.ExecuteAsync(null);
            }
            catch (System.Exception)
            {
                viewModel.ErrorMessage = "Не удалось подключиться к базе данных PostgreSQL.";
                viewModel.HasError = true;
            }
        };
    }
}
