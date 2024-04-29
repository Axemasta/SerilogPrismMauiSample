using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace SerilogMaui.ViewModels;

public partial class MainViewModel(ILogger logger, INavigationService navigationService)
    : BaseViewModel(logger, navigationService)
{
    [RelayCommand]
    private void Write()
    {
        logger.Information("WriteCommand executed");
    }
}