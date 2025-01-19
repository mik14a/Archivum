using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;
using Microsoft.Maui;

namespace Archivum.Platforms.Android;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState) {
        base.OnCreate(savedInstanceState);

        // Android 11 or later
        if (Window != null && OperatingSystem.IsAndroidVersionAtLeast(30)) {
            Window.InsetsController?.Hide(WindowInsets.Type.NavigationBars());
            if (Window.InsetsController != null) {
                Window.InsetsController.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
            }
        }
    }
}
