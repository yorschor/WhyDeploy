using System;
using ReactiveUI;
using WhyDeployClient.ViewModels.Pages;
using WhyDeployClient.Views.Pages;

namespace WhyDeployClient.ViewLocators;

public class PageViewLocator : ReactiveUI.IViewLocator
{
    public IViewFor ResolveView<T>(T viewModel, string contract = null)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));
        
        var viewModelTypeName = viewModel.GetType().FullName;
        var viewTypeName = viewModelTypeName?.Replace("ViewModels", "Views").Replace("ViewModel", "");

        if (string.IsNullOrEmpty(viewTypeName))
            throw new InvalidOperationException("Unable to determine view type from view model type.");

        var viewType = Type.GetType(viewTypeName);

        if (viewType == null || !typeof(IViewFor).IsAssignableFrom(viewType))
            throw new InvalidOperationException($"No view found for view model type {viewModelTypeName}.");

        var view = (IViewFor)Activator.CreateInstance(viewType)!;
        view.ViewModel = viewModel;

        return view;
    }
}