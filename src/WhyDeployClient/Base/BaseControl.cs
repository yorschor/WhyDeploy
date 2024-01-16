using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace WhyDeployClient.Base;

public class BaseControl<TViewModel> : ReactiveUserControl<TViewModel>
    where TViewModel : class
{
    protected BaseControl(bool activate = true)
    {
        if (activate)
        {
            this.WhenActivated(disposables =>
            {
                Disposable.Create(() => { }).DisposeWith(disposables);
            });
        }
    }
}