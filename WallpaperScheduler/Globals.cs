using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace WallpaperScheduler;

static class Globals
{
    private static MethodInfo setWallpaperMethod;
    private static MethodInfo setAppearanceModeMethod;

    public static double latitude;
    public static double longitude;

    public static Config config;

    public static bool IsNullOrEmpty(Array array)
    {
        return (array == null || array.Length == 0);
    }

    public static void LoadAppearanceHandler()
    {
        var asm = Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), config.SystemAppearanceHandler));
        var type = asm.GetType("AppearanceHandler.Handler");

        setWallpaperMethod = type.GetMethod("SetWallpaper", BindingFlags.Static | BindingFlags.Public);
        setAppearanceModeMethod = type.GetMethod("SetAppearanceMode", BindingFlags.Static | BindingFlags.Public);
    }

    public static bool SetWallpaper(string wallpaperFilename)
    {
        try
        {
            var res = setWallpaperMethod.Invoke(null, [wallpaperFilename]);
            return (bool)res;
        }
        catch
        {
            return false;
        }
    }

    public static bool SetAppearanceMode(bool isDarkMode)
    {
        try
        {
            var res = setAppearanceModeMethod.Invoke(null, [isDarkMode]);
            return (bool)res;
        }
        catch
        {
            return false;
        }
    }
}