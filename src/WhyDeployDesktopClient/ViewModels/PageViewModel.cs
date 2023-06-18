namespace WhyDeployDesktopClient.ViewModels;

public class PageViewModel : ViewModelBase
{
	public static string CurrentDevPath
	{
		get => Properties.Settings.Default.CurrentDevPath;
		set
		{
			Properties.Settings.Default.CurrentDevPath = value;
			Properties.Settings.Default.Save();
		}
	}

	// public Deployer DeployerInstance { get; private set; }

	public string AppName { get; set; }
	// public JobCollection Jobs { get; set; }
}