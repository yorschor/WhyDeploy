using System;
using ReactiveUI;
using WhyDeployClient.Views.UserControls;

namespace WhyDeployClient.ViewLocators;

public class WDMLModViewLocator : IViewLocator
{
    public IViewFor ResolveView<T>(T viewModel, string contract = null)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        var viewTypeName = typeof(WDMLModView).FullName;

        var viewType = Type.GetType(viewTypeName!);

        var view = (IViewFor)Activator.CreateInstance(viewType!)!;
        view.ViewModel = viewModel;

        return view;
    }
}