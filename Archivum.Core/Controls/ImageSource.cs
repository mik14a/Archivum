namespace Archivum.Controls;

/// <summary>
/// Represents a source for image data stored in memory as a byte array.
/// </summary>
public class ImageSource
{
    /// <summary>
    /// Gets the raw image data as a byte array.
    /// </summary>
    public byte[] ImageData => _imageData;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageSource"/> class with the specified image data.
    /// </summary>
    /// <param name="imageData">The raw image data as a byte array.</param>
    public ImageSource(byte[] imageData) {
        _imageData = imageData;
    }

    readonly byte[] _imageData;
}
