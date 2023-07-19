using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Humanizer;

namespace Artemis.UI.Shared.Converters;

public class HumanizerConverter : IValueConverter
{
    #region Implementation of IValueConverter

    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            TimeSpan ts => ts.Humanize(),
            DateTime dt => dt.Humanize(),
            DateTimeOffset dto => dto.Humanize(),
            string str => str.Humanize(),
            Enum @enum => @enum.Humanize(),
            _ => value?.ToString().Humanize()
        };
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    #endregion
}