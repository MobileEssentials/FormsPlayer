# Xamarin Forms Player

Provides real-time previewing of Xamarin.Forms XAML source in Visual Studio, by pushing the XAML to devices running a Xamarin.Forms.Player app.

The extension provides a new `View | Other Windows | Xamarin Forms Player` command, that opens a Tool Window. The session ID shown in this window should be entered in the running Xamarin Forms Player mobile app, and that will connect both. This means that you can preview the XAML in more than one device at the same time.

![Visual Studio ToolWindow](https://raw.githubusercontent.com/MobileEssentials/FormsPlayer/master/src/FormsPlayer.Vsix/Resources/screen-1.png)

To get the Xamarin.Forms.Player app on the device, either build it yourself [from source](https://github.com/MobileEssentials/FormsPlayer) or create a new Xamarin.Forms app with whatever references you need to custom controls, control libraries, etc., and install the Xamarin.Forms.Player nuget package in it. Then replace the `new App()` instantiation with `new Xamarin.Forms.Player.App()` instead. That will bring in the player UI and enable all the rendering functionality automatically.

![Player on iOS](https://raw.githubusercontent.com/MobileEssentials/FormsPlayer/master/src/FormsPlayer.Vsix/Resources/screen-2.png)

![Player on Android](https://raw.githubusercontent.com/MobileEssentials/FormsPlayer/master/src/FormsPlayer.Vsix/Resources/screen-3.png)

For Android, you can also grab a pre-built APK directly from the [Releases](https://github.com/MobileEssentials/FormsPlayer/releases) page. The Visual Studio extension should match the version of the application.