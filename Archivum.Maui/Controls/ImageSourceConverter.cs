using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Archivum.Controls;

/// <summary>
/// Converts between <see cref="ImageSource"/> and platform-specific image source types.
/// </summary>
public class ImageSourceConverter : IValueConverter
{
    /// <summary>
    /// Converts an <see cref="ImageSource"/> to a platform-specific memory-based image source.
    /// </summary>
    /// <param name="value">The source <see cref="ImageSource"/> to convert.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">Optional conversion parameter (not used).</param>
    /// <param name="culture">The culture to use for conversion.</param>
    /// <returns>A platform-specific <see cref="MemoryImageSource"/> if conversion succeeds; otherwise, null.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return value is ImageSource imageSource ? new MemoryImageSource(imageSource.ImageData) : null;
    }

    /// <summary>
    /// Converts a platform-specific image source back to an <see cref="ImageSource"/>.
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">Optional conversion parameter (not used).</param>
    /// <param name="culture">The culture to use for conversion.</param>
    /// <returns>This operation is not supported and will throw <see cref="NotImplementedException"/>.</returns>
    /// <exception cref="NotImplementedException">This conversion operation is not supported.</exception>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
