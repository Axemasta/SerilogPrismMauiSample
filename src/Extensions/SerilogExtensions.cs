using System.Diagnostics;
using Serilog;
using Serilog.Templates;

namespace SerilogMaui.Extensions;

public static class SerilogExtensions
{
    public static LoggerConfiguration WriteToDevice(this LoggerConfiguration configuration, ExpressionTemplate outputTemplate)
    {
        return Debugger.IsAttached 
            ? configuration.WriteTo.Debug(outputTemplate) 
            : configuration.WriteTo.Console(outputTemplate);
    }
}