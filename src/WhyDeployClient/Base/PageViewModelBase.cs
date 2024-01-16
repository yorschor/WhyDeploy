using System;
using ReactiveUI;
using WhyDeployClient.ViewModels;

namespace WhyDeployClient.Base;

public class PageViewModelBase(IScreen screen) : ViewModelBase, IRoutableViewModel
{
    public IScreen HostScreen { get; } = screen;
    public string UrlPathSegment { get; } = Guid.NewGuid().ToString()[..5];
    
    public string? Header { get; init; }

    public string? Description { get; init; }

    public string? IconResourceKey { get; init; }

    public string? PageKey { get; init; }

    public string[]? SearchKeywords { get; init; }
}