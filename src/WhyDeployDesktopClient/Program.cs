using Avalonia;
using Avalonia.ReactiveUI;
using System;
using ReactiveUI;
using Splat;
using WhyDeployDesktopClient.ViewModels;
using WhyDeployDesktopClient.Views;

namespace WhyDeployDesktopClient;

class Program
{
	// Initialization code. Don't use any Avalonia, third-party APIs or any
	// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
	// yet and stuff might break.
	[STAThread]
	public static void Main(string[] args)
	{
		BuildAvaloniaApp()
			.StartWithClassicDesktopLifetime(args);
	}


	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<App>()
			.UsePlatformDetect()
			.With(new Win32PlatformOptions { OverlayPopups = true, })
			.With(new X11PlatformOptions { OverlayPopups = true })
			.With(new MacOSPlatformOptions { ShowInDock = true })
			.UseReactiveUI()
			.LogToTrace();
	// Avalonia configuration, don't remove; also used by visual designer.
}