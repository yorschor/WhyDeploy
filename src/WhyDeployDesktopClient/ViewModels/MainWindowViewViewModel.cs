using System;
using System.Collections.Generic;
using System.Windows.Input;
using FluentAvalonia.UI.Controls;
using ReactiveUI;

namespace WhyDeployDesktopClient.ViewModels;

public class MainWindowViewViewModel : ViewModelBase
{
    private List<AppPageNavItem> _appPages;

    public PageViewModel _currentPage;


    public object _selectedAppPage;

    private readonly SettingsPageViewModel _settingsPage = new();

    public MainWindowViewViewModel()
    {
        #region Commands

        RefreshCommand = ReactiveCommand.Create(() =>
        {
            AppPages = new List<AppPageNavItem>();
            AppPages.Add(new AppPageNavItem
            {
                Name = "Home Page",
                Vm = new HomePageViewModel()
            });
            var deployAppPages = LoadApplicationPages();
            AppPages.AddRange(deployAppPages);

            SelectedAppPage = AppPages.Count > 0
                ? AppPages[0]
                : new NavigationViewItemBase();
        });

        #endregion

        RefreshCommand.Execute(null);
    }

    public List<AppPageNavItem> AppPages
    {
        get => _appPages;
        set => this.RaiseAndSetIfChanged(ref _appPages, value);
    }

    public object SelectedAppPage
    {
        get => _selectedAppPage;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedAppPage, value);
            SetCurrentPage();
        }
    }

    public PageViewModel CurrentPage
    {
        get => _currentPage;
        set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }

    public ICommand RefreshCommand { get; }

    private void SetCurrentPage()
    {
        CurrentPage = SelectedAppPage switch
        {
            AppPageNavItem cat => cat.Vm,
            NavigationViewItem => _settingsPage,
            _ => Activator.CreateInstance<HomePageViewModel>()
        };
    }

    private List<AppPageNavItem> LoadApplicationPages()
    {
        return new List<AppPageNavItem>();
        // var applicationPages = new List<AppPageNavItem>();
        // var jsonFilePath = Properties.Settings.Default.DeployApplicationConfigStorage;
        //
        // try
        // {
        // 	var deployAppStorageSettingsList = WDAppStorageHelper.GetAppsFromStorage(jsonFilePath);
        // 	foreach (var deployAppStorageSettings in deployAppStorageSettingsList!)
        // 	{
        // 		var applicationPageInstance = Activator.CreateInstance<ApplicationPage>();
        // 		applicationPageInstance.DataContext =
        // 			new ApplicationPageViewModel(deployAppStorageSettings.PathToAppConfig);
        //
        // 		var newApp = new AppPageNavItem
        // 		{
        // 			Name = deployAppStorageSettings.Name
        // 			// Icon = Symbol.Games,
        // 			// InfoBadge = deployAppStorageSettings.PathToAppConfig
        // 			// ApplicationPage = applicationPageInstance
        // 		};
        //
        // 		applicationPages.Add(newApp);
        // 	}
        // }
        // catch (Exception ex)
        // {
        // 	Console.WriteLine("Error loading AppPages: " + ex.Message);
        // }
        //
        // return applicationPages;
    }
}

public class AppPageNavItem : NavigationViewItem
{
    public string ToolTip { get; set; }
    public PageViewModel Vm { get; set; }
}