using System;
using System.IO;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Archivum.Controls;

public partial class ImageSourceConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language) {
        if (value is not ImageSource source) return null;
        var bitmap = new BitmapImage();
        using var stream = new MemoryStream(source.ImageData);
        bitmap.SetSource(stream.AsRandomAccessStream());
        return bitmap;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
        throw new NotImplementedException();
    }
}
