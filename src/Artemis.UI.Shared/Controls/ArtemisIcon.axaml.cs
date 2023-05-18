using System;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Material.Icons;
using Material.Icons.Avalonia;

namespace Artemis.UI.Shared;

/// <summary>
///     Represents a control that can display an arbitrary kind of icon.
/// </summary>
public partial class ArtemisIcon : UserControl
{
    private static readonly Regex ImageRegex = new(@"[\/.](gif|jpg|jpeg|tiff|png)$", RegexOptions.Compiled);

    /// <summary>
    ///     Creates a new instance of the <see cref="ArtemisIcon" /> class.
    /// </summary>
    public ArtemisIcon()
    {
        InitializeComponent();
        DetachedFromLogicalTree += OnDetachedFromLogicalTree;
        LayoutUpdated += OnLayoutUpdated;
        PropertyChanged += OnPropertyChanged;
    }

    private void Update()
    {
        try
        {
            // First look for an enum value instead of a string
            if (Icon is MaterialIconKind materialIcon)
            {
                Content = new MaterialIcon {Kind = materialIcon, Width = Bounds.Width, Height = Bounds.Height};
            }
            // If it's a string there are several options
            else if (Icon is string iconString)
            {
                // An URI pointing to an image
                if (ImageRegex.IsMatch(iconString))
                {
                    Image image = new() {Source = new Bitmap(iconString), VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Stretch};
                    RenderOptions.SetBitmapInterpolationMode(image, BitmapInterpolationMode.HighQuality);
                    Content = image;
                }
                // An enum defined as a string
                else if (Enum.TryParse(iconString, true, out MaterialIconKind parsedIcon))
                {
                    Content = new MaterialIcon {Kind = parsedIcon, Width = Bounds.Width, Height = Bounds.Height};
                }
                else
                {
                    Content = new MaterialIcon {Kind = MaterialIconKind.QuestionMark, Width = Bounds.Width, Height = Bounds.Height};
                }
            }
        }
        catch
        {
            Content = new MaterialIcon {Kind = MaterialIconKind.QuestionMark, Width = Bounds.Width, Height = Bounds.Height};
        }
    }

    private void OnLayoutUpdated(object? sender, EventArgs e)
    {
        if (Content is Control contentControl)
        {
            contentControl.Width = Bounds.Width;
            contentControl.Height = Bounds.Height;
        }
    }

    private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == IconProperty)
            Update();
    }

    private void OnDetachedFromLogicalTree(object? sender, LogicalTreeAttachmentEventArgs e)
    {
        if (Content is Image image && image.Source is IDisposable disposable)
            disposable.Dispose();
    }

    #region Properties

    /// <summary>
    ///     Gets or sets the currently displayed icon as either a <see cref="MaterialIconKind" /> or an <see cref="Uri" />
    ///     pointing to an SVG
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty = AvaloniaProperty.Register<ArtemisIcon, object?>(nameof(Icon));


    /// <summary>
    ///     Gets or sets the currently displayed icon as either a <see cref="MaterialIconKind" /> or an <see cref="Uri" />
    ///     pointing to an SVG
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    #endregion
}