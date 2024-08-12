# Wallpaper Scheduler

Port of macOS Mojave Dynamic Desktop feature to every desktop operating system, simplified [WinDynamicDesktop](https://github.com/t1m0thyj/WinDynamicDesktop).

# Usage

1. Create `config.json` and input:
  ```json
  {
    "ThemePath": "Path to parent folder of theme.json",
    "IpInfoToken": "ipinfo.io token",
    "SystemAppearanceHandler": "AppearanceHandler.dll"
  }
  ```

  Here, format of `theme.json` is same as [WinDynamicDesktop](https://github.com/t1m0thyj/WinDynamicDesktop). You can download some themes on their [website](https://windd.info/themes/).

  `IpInfoToken` is for accessing their service of getting current GEO position. You can acquire one by signing up on their website. It's free.

  `SystemAppearanceHandler` is the path to `AppearanceHandler.dll`. You can build yours for your desktop environment simply by editing `AppearanceHandler.cs`.

2. Edit `SystemAppearanceHandler.cs` to meet your desktop environment. The existing one is for `swww` wallpaper engine and `gnome-tweak` theme.
   
3. Build the project and run `WallpaperScheduler`.
   

# Credits

- [WinDynamicDesktop](https://github.com/t1m0thyj/WinDynamicDesktop/): SolarScheduler.cs, Sun.cs, EventScheduler.cs.

# LICENSE

Mozilla Public License 2.0