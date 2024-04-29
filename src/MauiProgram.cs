using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Templates;
using SerilogMaui.Extensions;
using SerilogMaui.Pages;
using SerilogMaui.ViewModels;

namespace SerilogMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        ConfigureSerilog();
        
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UsePrism(prism =>
            {
                prism.RegisterTypes(containerRegistry =>
                {
                    containerRegistry.RegisterSerilog();
                    
                    containerRegistry.RegisterForNavigation<MainPage, MainViewModel>();
                });
                
                prism.CreateWindow(async navigationService =>
                {
                    var result = await navigationService.CreateBuilder()
                        .AddNavigationPage()
                        .AddSegment<MainViewModel>()
                        .NavigateAsync();

                    if (!result.Success)
                    {
                        Debugger.Break();
                        Console.WriteLine(result.Exception);
                    }
                });
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Logging.AddSerilog(dispose: true);

        return builder.Build();
    }

    private static void ConfigureSerilog()
    {
        var outputTemplate =
            new ExpressionTemplate(
                "[{@t:yyyy-MM-dd HH:mm:ss.fff zzz}] [{@l:u3}] {#if SourceContext is not null}[{Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)}] {#end}{@m:lj}\n{@x}");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteToDevice(outputTemplate)
            .CreateLogger();
    }
}