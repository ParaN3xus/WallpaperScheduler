using Newtonsoft.Json;

namespace WallpaperScheduler;

public class Config
{
    public string ThemePath;
    public string IpInfoToken;
    public string SystemAppearanceHandler;

    public static void Load(string configPath)
    {
        string jsonPath = configPath;

        Globals.config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(jsonPath));
    }
}