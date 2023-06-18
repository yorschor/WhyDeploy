using Avalonia.ReactiveUI;

namespace WhyDeployDesktopClient.Views;

public partial class ApplicationPage : ReactiveUserControl<ApplicationPage>
{
	private string _applicationTooltip;
	private string _applicationName;
	private string _iconName;

	#region Properties

	public string ApplicationTooltip
	{
		get => _applicationTooltip;
		set
		{
			_applicationTooltip = value;
		}
	}

	public string ApplicationName
	{
		get => _applicationName;
		set
		{
			_applicationName = value;
		}
	}

	public string IconName
	{
		get => _iconName;
		set
		{
			_iconName = value;
		}
	}

	#endregion

	public ApplicationPage()
	{
		InitializeComponent();
		ApplicationTooltip = "Test";
		ApplicationName = "HelloName";
		IconName = "ArrowRight12Filled";
	}
	
}