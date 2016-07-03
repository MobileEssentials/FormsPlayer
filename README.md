# Xamarin Forms Player

Provides real-time previewing of Xamarin.Forms XAML source in Visual Studio, by pushing the XAML to devices running a Xamarin.Forms.Player app.

The extension provides a new `View | Other Windows | Xamarin Forms Player` command, that opens a Tool Window. The session ID shown in this window should be entered in the running Xamarin Forms Player mobile app, and that will connect both. This means that you can preview the XAML in more than one device at the same time.

![Visual Studio ToolWindow](https://raw.githubusercontent.com/MobileEssentials/FormsPlayer/master/src/FormsPlayer.Vsix/Resources/screen-1.png)

To get the Xamarin.Forms.Player app on the device, either build it yourself [from source](https://github.com/MobileEssentials/FormsPlayer) or grab a pre-built APK directly from the [Releases](https://github.com/MobileEssentials/FormsPlayer/releases) page (for Android). 

To enable support for custom controls, behaviors, converters, etc. you need to embed `FormsPlayer` in your application. The steps to do so are (the complexity caused by Microsoft.AspNet.SignalR.Client not installable into latest iOS/Android):

1. Update Profile in your PCL project (should be Profile78 to be able to install Microsoft.AspNet.SignalR.Client). This could be done by editing you .csproj in notepad and altering TargetFrameworkProfile value to `<TargetFrameworkProfile>Profile78</TargetFrameworkProfile>`
2. `Install-Package Microsoft.AspNet.SignalR.Client` into your __PCL project__
3. `Install-Package Xamarin.Forms.Player2 -pre` into you Android/iOS platform-specific project
4. Replace the `LoadApplication(new App())` with the following `LoadApplication(new Xamarin.Forms.Player.AppController(new App()).App);` in your AppDelegate/MainActivity.

![Player on iOS](https://raw.githubusercontent.com/MobileEssentials/FormsPlayer/master/src/FormsPlayer.Vsix/Resources/screen-2.png)

![Player on Android](https://raw.githubusercontent.com/MobileEssentials/FormsPlayer/master/src/FormsPlayer.Vsix/Resources/screen-3.png)

The Visual Studio extension should match the version of the application.
