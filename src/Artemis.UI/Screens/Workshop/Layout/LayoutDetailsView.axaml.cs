using System;
using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Artemis.UI.Screens.Workshop.Layout;

public partial class LayoutDetailsView : ReactiveUserControl<LayoutDetailsViewModel>
{
    public LayoutDetailsView()
    {
        InitializeComponent();
        this.WhenActivated(d => ViewModel.WhenAnyValue(vm => vm.Screen)
            .Subscribe(screen => RouterFrame.NavigateFromObject(screen ?? ViewModel?.LayoutDescriptionViewModel))
            .DisposeWith(d));
    }
}