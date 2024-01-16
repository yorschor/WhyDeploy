using System.Collections.Generic;
using System.Linq;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using WhyDeployClient.Base;
using WhyDeployClient.ViewModels.Pages;

namespace WhyDeployClient.ViewModels;

public class MainViewModel : ViewModelBase, IScreen
{
    public MainViewModel()
    {
        SetupStaticPages();
        LoadAppPages();
        _selectedAppPage = PageViewModels.First();
        Router.Navigate.Execute(HomePage!);
    }

    private void LoadAppPages()
    {
        PageViewModels.Add(new WDMLApplicationPageViewModel(this)
        {
            Header = "TestApp1",
            Description = "HelloThere"
        });
        PageViewModels.Add(new WDMLApplicationPageViewModel(this)
        {
            Header = "TestApp2",
            Description = "HelloThere"
        });
        PageViewModels.Add(new WDMLApplicationPageViewModel(this)
        {
            Header = "TestApp3",
            Description = "HelloThere"
        });
    }

    #region Routing

    private List<PageViewModelBase> _pageViewModels = [];
    private object _selectedAppPage;

    public List<PageViewModelBase> PageViewModels
    {
        get => _pageViewModels;
        set => this.RaiseAndSetIfChanged(ref _pageViewModels, value);
    }

    public object SelectedAppPage
    {
        get => _selectedAppPage;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedAppPage, value);
            if (_selectedAppPage is NavigationViewItem) Router.Navigate.Execute(SettingsPage!);
            Router.Navigate.Execute((PageViewModelBase)value);
        }
    }

    public RoutingState Router { get; } = new();

    private HomePageViewModel? HomePage { get; set; }
    private SettingsPageViewModel? SettingsPage { get; set; }

    private void SetupStaticPages()
    {
        HomePage = new HomePageViewModel(this)
        {
            Header = "HomePage",
            Description = "HomePageToolTip",
            IconResourceKey = "HomeFilled"
        };
        PageViewModels.Add(HomePage);
        SettingsPage = new SettingsPageViewModel(this)
        {
            Header = "Settings",
            Description = "SettingsPage ToolTip",
            IconResourceKey = "SettingsFilled"
        };
    }

    #endregion
}