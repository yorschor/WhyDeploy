using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WhyDeployClient.Base;
using WhyDeployClient.ViewModels.Pages;

namespace WhyDeployClient.Views.Pages;

public partial class SettingsPage : BaseControl<SettingsPageViewModel>
{
    public SettingsPage()
    {
        InitializeComponent();
    }
}