using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using TextForge.Core.Presentation;
using Microsoft.Extensions.DependencyInjection;
using TextForge.Desktop.Services;
using TextForge.Desktop.ViewModels;
using TextForge.Desktop.Views;

namespace TextForge.Desktop;

public partial class App : Application
{
    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddSingleton<IModulePaletteProvider, ModulePaletteService>();
        collection.AddTransient<MainWindowViewModel>();
        collection.AddTransient<MainWindow>();
        
        Services = collection.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.DataContext = Services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
