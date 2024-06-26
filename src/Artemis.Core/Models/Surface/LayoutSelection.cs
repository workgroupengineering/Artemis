namespace Artemis.Core;

/// <summary>
///     Represents a reference to a layout for a device.
/// </summary>
public class LayoutSelection : CorePropertyChanged
{
    private string? _type;
    private string? _parameter;
    private string? _errorState;

    /// <summary>
    ///     Gets or sets what kind of layout reference this is.
    /// </summary>
    public string? Type
    {
        get => _type;
        set => SetAndNotify(ref _type, value);
    }

    /// <summary>
    ///     Gets or sets the parameter of the layout reference, such as a file path of workshop entry ID.
    /// </summary>
    public string? Parameter
    {
        get => _parameter;
        set => SetAndNotify(ref _parameter, value);
    }

    /// <summary>
    ///     Gets or sets the error state of the layout reference.
    /// </summary>
    public string? ErrorState
    {
        get => _errorState;
        set => SetAndNotify(ref _errorState, value);
    }
}