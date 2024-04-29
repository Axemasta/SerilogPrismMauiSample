using Serilog;

namespace SerilogMaui.ViewModels;

public class MainViewModel(ILogger logger, INavigationService navigationService)
    : BaseViewModel(logger, navigationService);