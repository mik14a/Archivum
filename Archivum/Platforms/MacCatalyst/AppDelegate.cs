﻿using Foundation;

namespace Archivum;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() {
        return MauiProgram.CreateMauiApp();
    }
}
