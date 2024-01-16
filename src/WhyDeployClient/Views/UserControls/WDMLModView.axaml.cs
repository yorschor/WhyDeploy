using Avalonia;
using Avalonia.Media;
using WDBase.Models;
using WhyDeployClient.Base;
using WhyDeployClient.ViewModels.UserControls;

namespace WhyDeployClient.Views.UserControls;

public partial class WDMLModView : BaseControl<WDMLModViewModel>
{
    public static readonly StyledProperty<ImageDrawing> ModImageProperty =
        AvaloniaProperty.Register<WDMLModView, ImageDrawing>(nameof(ModImage));

    public static readonly StyledProperty<string> ModNameProperty =
        AvaloniaProperty.Register<WDMLModView, string>(nameof(ModName));

    public static readonly StyledProperty<string> ModInstanceProperty =
        AvaloniaProperty.Register<WDMLModView, string>(nameof(ModInstance));

    public WDMLModView()
    {
        InitializeComponent();
    }

    public WDMLModView(DeployJob job)
    {
        InitializeComponent();
        ModName = job.Name;
    }

    public ImageDrawing ModImage
    {
        get => GetValue(ModImageProperty);
        set => SetValue(ModImageProperty, value);
    }

    public string ModName
    {
        get => GetValue(ModNameProperty);
        set => SetValue(ModNameProperty, value);
    }

    public string ModInstance
    {
        get => GetValue(ModInstanceProperty);
        set => SetValue(ModInstanceProperty, value);
    }
}