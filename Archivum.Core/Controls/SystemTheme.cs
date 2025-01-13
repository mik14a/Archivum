namespace Archivum.Controls;

/// <summary>
/// Specifies the theme mode to be applied to the application's user interface.
/// </summary>
public enum SystemTheme
{
    /// <summary>
    /// Uses the operating system's theme settings, automatically adapting between light and dark modes.
    /// </summary>
    System,

    /// <summary>
    /// Forces the application to use light theme mode regardless of system settings.
    /// </summary>
    Light,

    /// <summary>
    /// Forces the application to use dark theme mode regardless of system settings.
    /// </summary>
    Dark
}
