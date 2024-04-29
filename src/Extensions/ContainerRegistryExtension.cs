using DryIoc;

namespace SerilogMaui.Extensions;

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