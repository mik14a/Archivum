﻿using Android.App;
using Android.Runtime;

namespace Archivum;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(nint handle, JniHandleOwnership ownership)
        : base(handle, ownership) {
    }

    protected override MauiApp CreateMauiApp() {
        return MauiProgram.CreateMauiApp();
    }
}
