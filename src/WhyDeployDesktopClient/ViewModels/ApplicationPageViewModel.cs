namespace WhyDeployDesktopClient.ViewModels;

public class ApplicationPageViewModel : PageViewModel
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
	
	public ApplicationPageViewModel(string appConfigPath)
	{
		// DeployerInstance = new Deployer(appConfigPath);
		// AppName = DeployerInstance.DeployAppConfig.Name;
		// Jobs = DeployerInstance.Jobs;
	}

	public string AppName { get; set; }
	// public JobCollection Jobs { get; set; }
}