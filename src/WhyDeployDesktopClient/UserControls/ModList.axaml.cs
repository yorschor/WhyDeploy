using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using WDBase.Collections;
using WDCore;
using WDCore.StorageHelper;

namespace WhyDeployDesktopClient.UserControls;

public partial class ModList : ReactiveUserControl<ModList>
{
	public static readonly StyledProperty<WDJobCollection> JobCollectionProperty =
	AvaloniaProperty.Register<ModList, WDJobCollection>(nameof(WDJobCollection));
	
	public WDJobCollection WDJobCollection
	{
		get => GetValue(JobCollectionProperty);
		set => SetValue(JobCollectionProperty, value);
	}
	
	public ModList()
	{
		InitializeComponent();
	}
	
	public List<DeployableMod> Items { get; set; } = new();

	protected void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
	{
		base.OnPropertyChanged(change);

		if (change.Property == JobCollectionProperty)
		{
			// Clear the Items list and add the new jobs
			Items.Clear();
			foreach (var job in WDJobCollection)
			{
				Items.Add(new DeployableMod(job));
			}
		}
	}
}