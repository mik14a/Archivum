namespace Archivum.Controls;

/// <summary>
/// Specifies the type of system backdrop effect to be applied to a window or surface.
/// </summary>
public enum SystemBackdrop
{
    /// <summary>
    /// The default Mica effect, which creates a translucent, layered appearance that incorporates desktop wallpaper colors.
    /// </summary>
    Mica,

    /// <summary>
    /// An alternative version of the Mica effect with different transparency and blur settings.
    /// </summary>
    MicaAlt,

    /// <summary>
    /// The Acrylic effect, which creates a translucent, blurred background that helps establish visual hierarchy.
    /// </summary>
    Acrylic
}
