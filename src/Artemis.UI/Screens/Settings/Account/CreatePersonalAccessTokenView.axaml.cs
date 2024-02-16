using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Artemis.UI.Screens.Settings.Account;

public partial class CreatePersonalAccessTokenView : ReactiveUserControl<CreatePersonalAccessTokenViewModel>
{
    public CreatePersonalAccessTokenView()
    {
        InitializeComponent();
    }
}