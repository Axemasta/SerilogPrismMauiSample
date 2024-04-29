# Serilog Prism Maui Sample

This sample app demonstrates the best way to use Serilog with .NET MAUI, using prism to do some spicy DI magic to automatically set the logging context.

If you aren't interested why you should choose Serilog over Microsoft.Extensions.Logging, or how to setup our spicy registration look no further!

## Install Packages

The following packages should be installed, based on your requirements you can add the Serilog sinks you require:

```xml
<ItemGroup>
    <PackageReference Include="Serilog" Version="3.1.1" />
    <PackageReference Include="Serilog.Expressions" Version="4.0.0" />
    <PackageReference Include="Serilog.Extensions.Logging" Version="8.0.0" />
    <PackageReference Include="Serilog.Sinks.Console" Version="5.0.1" />
    <PackageReference Include="Serilog.Sinks.Debug" Version="2.0.0" />
</ItemGroup>
```

## Add Extension Methods

### Device Logging

This extension method will allow you to always log onto the device regardless of whether your app is debugging or in release mode.

```csharp
public static class SerilogExtensions
{
    public static LoggerConfiguration WriteToDevice(this LoggerConfiguration configuration, ExpressionTemplate outputTemplate)
    {
        return Debugger.IsAttached 
            ? configuration.WriteTo.Debug(outputTemplate) 
            : configuration.WriteTo.Console(outputTemplate);
    }
}
```

When debugging you will see the logs normally, when in release the logs will be written to the device console (via `Console.WriteLine` which prints to the native console) and can be read from the device, for example when connected via USB.

### Prism Registration

This extension method makes use of the DryIoc container so won't be available for other containers. This method uses context based resolution to set the context of the logger when it exists, see [Examples of context based resolution](https://github.com/dadhi/DryIoc/blob/master/docs/DryIoc.Docs/ExamplesContextBasedResolution.md#serilog-logger) for a more in depth explanation.

```csharp
public static class ContainerRegistryExtension
{
    public static void RegisterSerilog(this IContainerRegistry containerRegistry)
    {
        var container = containerRegistry.GetContainer();

        // default logger
        container.Register(Made.Of(() => Serilog.Log.Logger),
            setup: Setup.With(condition: r => r.Parent.ImplementationType == null));

        // type dependent logger
        container.Register(
            Made.Of(() => Serilog.Log.ForContext(Arg.Index<Type>(0)), r => r.Parent.ImplementationType),
            setup: Setup.With(condition: r => r.Parent.ImplementationType != null));
    }
}
```

When simply resolving the `ILogger` directly, you will get a blank logger.
When resolving the logger within a class, the context will be set to the class the logger has been injected into!

## Create Serilog Logger

In your `MauiProgram.cs` add the following method:

```csharp
private static void ConfigureSerilog()
{
    var outputTemplate =
        new ExpressionTemplate(
            "[{@t:yyyy-MM-dd HH:mm:ss.fff zzz}] [{@l:u3}] {#if SourceContext is not null}[{Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)}] {#end}{@m:lj}\n{@x}");

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        // Add whatever sinks you require here
        .WriteToDevice(outputTemplate)
        .CreateLogger();
}
```

Call the configuration method **BEFORE** creating the `MauiAppBuilder`:

```diff
public static MauiApp CreateMauiApp()
{
+   ConfigureSerilog();
    
    var builder = MauiApp.CreateBuilder();
    ...
}
```

## Add Serilog Logging Integration

Add the serilog provider for Microsoft.Extensions.Logging. This will mean your app collects any logs from the `Microsoft.Extensions.Logging.ILogger`

```diff
public static MauiApp CreateMauiApp()
{
    ConfigureSerilog();
    
    var builder = MauiApp.CreateBuilder();

+   builder.Logging.AddSerilog(dispose: true);

    return builder.Build();
}
```

## Use your logger

In your app whenever you inject an `ILogger`, it will be registered contexually. Given the following base viewmodel class:

```csharp
public abstract class BaseViewModel : ObservableObject
{
    protected ILogger logger;
    protected INavigationService navigationService;

    protected BaseViewModel(ILogger logger, INavigationService navigationService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(navigationService);
        
        this.logger = logger;
        this.navigationService = navigationService;
    }
}
```

Any viewmodel that inherits `BaseViewModel` will have its context set automatically!

```csharp
public partial class MainViewModel(ILogger logger, INavigationService navigationService)
    : BaseViewModel(logger, navigationService)
{
    [RelayCommand]
    private void Write()
    {
        logger.Information("WriteCommand executed");
    }
}
```

When `WriteCommand` executes, the following will be printed:

```bash
[2024-04-29 12:03:42.123 +01:00] [INF] [MainViewModel] WriteCommand executed
```