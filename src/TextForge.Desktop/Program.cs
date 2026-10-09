using Avalonia;
using System;

namespace TextForge.Desktop;

sealed class Program
{

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[Startup Crash]: {ex}");
            LogFatalException("Startup Crash in BuildAvaloniaApp", ex);
            throw;
        }
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            LogFatalException("AppDomain.CurrentDomain.UnhandledException", e.ExceptionObject as Exception);
        };
        System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            LogFatalException("TaskScheduler.UnobservedTaskException", e.Exception);
            e.SetObserved();
        };
    }
    private static void LogFatalException(string source, Exception? ex)
    {
        string message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] FATAL ERROR in {source}:\n{ex}\n\n";
        Console.Error.WriteLine(message);
        try
        {
            System.IO.File.AppendAllText("crash.log", message);
        }
        catch
        {
            // Fail silently if disk write is prohibited
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
        #if DEBUG
            .WithDeveloperTools()
        #endif
            .WithInterFont()
            .LogToTrace();
}
