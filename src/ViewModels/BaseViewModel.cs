using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
// ReSharper disable NotAccessedField.Global
// ReSharper disable InconsistentNaming

namespace SerilogMaui.ViewModels;

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