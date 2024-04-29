using System.Diagnostics;
using Serilog;
using Serilog.Templates;

namespace SerilogMaui.Extensions;

public static class SerilogExtensions
{
    public static LoggerConfiguration WriteToDevice(this LoggerConfiguration configuration, ExpressionTemplate outputTemplate)
    {
        if (Debugger.IsAttached)
        {
            return configuration.WriteTo.Debug(outputTemplate);
        }

        return configuration.WriteTo.Console(outputTemplate);
    }
}