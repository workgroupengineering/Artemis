using System;
using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Artemis.UI.Screens.Workshop.Plugins;

public partial class PluginDetailsView : ReactiveUserControl<PluginDetailsViewModel>
{
    public PluginDetailsView()
    {
        InitializeComponent();
        this.WhenActivated(d => ViewModel.WhenAnyValue(vm => vm.Screen)
            .WhereNotNull()
            .Subscribe(screen => RouterFrame.NavigateFromObject(screen))
            .DisposeWith(d));
    }
}