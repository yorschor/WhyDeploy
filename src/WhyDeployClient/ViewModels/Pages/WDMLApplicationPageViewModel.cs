using System.Collections.Generic;
using ReactiveUI;
using WhyDeployClient.Base;
using WhyDeployClient.ViewModels.UserControls;

namespace WhyDeployClient.ViewModels.Pages;

public class WDMLApplicationPageViewModel : PageViewModelBase, IScreen
{
    private object _selectedMod;

    public WDMLApplicationPageViewModel(IScreen screen) : base(screen)
    {
        ModSource.Add(new WDMLModViewModel(this)
        {
            Name = "Test1"
        });
        ModSource.Add(new WDMLModViewModel(this)
        {
            Name = "Test2"
        });
    }

    public List<WDMLModViewModel> ModSource { get; } = [];

    public object SelectedMod
    {
        get => _selectedMod;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedMod, value);
            if (value is WDMLModViewModel model) Router.Navigate.Execute(model);
        }
    }

    public RoutingState Router { get; } = new();
}