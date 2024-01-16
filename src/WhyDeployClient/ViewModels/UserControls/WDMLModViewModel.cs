using System;
using ReactiveUI;
using WhyDeployClient.Base;

namespace WhyDeployClient.ViewModels.UserControls;

public class WDMLModViewModel(IScreen screen) : ViewModelBase, IRoutableViewModel
{
    public string? Name { get; set; }
    public IScreen HostScreen { get; } = screen;
    public string UrlPathSegment { get; } = Guid.NewGuid().ToString()[..5];
}