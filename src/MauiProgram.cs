using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SerilogMaui.Extensions;
using SerilogMaui.Pages;
using SerilogMaui.ViewModels;

namespace SerilogMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
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

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}