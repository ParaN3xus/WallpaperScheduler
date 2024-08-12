using System.Diagnostics;


namespace AppearanceHandler;

public static class Handler
{
    private static bool ExecuteCommand(string command, string arguments)
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
                return process.ExitCode == 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing command '{command} {arguments}': {ex.Message}");
            return false;
        }
    }

    public static bool SetWallpaper(string wallpaperFilename)
    {
        string command = "swww";
        string arguments = $"img {wallpaperFilename}";
        return ExecuteCommand(command, arguments);
    }

    public static bool SetAppearanceMode(bool isDarkMode)
    {
        string mode = isDarkMode ? "prefer-dark" : "prefer-light";
        string command = "gsettings";
        string arguments = $"set org.gnome.desktop.interface color-scheme '{mode}'";
        return ExecuteCommand(command, arguments);
    }
}