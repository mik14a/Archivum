using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui;

namespace Archivum.Controls;

public partial class MemoryImageSource : Microsoft.Maui.Controls.ImageSource, IStreamImageSource, IImageSource
{
    bool IImageSource.IsEmpty => _imageData == null;

    public MemoryImageSource(byte[] imageData) {
        _imageData = imageData;
    }

    Task<Stream> IStreamImageSource.GetStreamAsync(CancellationToken userToken) {
        return Task.FromResult<Stream>(new MemoryStream(_imageData));
    }

    readonly byte[] _imageData;
}
