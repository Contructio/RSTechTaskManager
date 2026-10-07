using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using RSTechAppTest.Views;

namespace RSTechAppTest; // Убедитесь, что namespace совпадает с именем вашего проекта

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Извлекаем зарегистрированное окно из DI-контейнера.
            // Контейнер сам автоматически создаст MainViewModel и передаст её в конструктор окна!
            desktop.MainWindow = Program.ServiceProvider.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
