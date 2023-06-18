using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using WDBase.Models;

namespace WhyDeployDesktopClient.UserControls;

public partial class DeployableMod : UserControl
{
    public static readonly StyledProperty<ImageDrawing> ModImageProperty =
        AvaloniaProperty.Register<DeployableMod, ImageDrawing>(nameof(ModImage));

    public static readonly StyledProperty<string> ModNameProperty =
        AvaloniaProperty.Register<DeployableMod, string>(nameof(ModName));

    public static readonly StyledProperty<string> ModInstanceProperty =
        AvaloniaProperty.Register<DeployableMod, string>(nameof(ModInstance));

    public DeployableMod()
    {
        InitializeComponent();
    }

    public DeployableMod(DeployJob job)
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