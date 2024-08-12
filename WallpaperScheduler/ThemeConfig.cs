using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace WallpaperScheduler.Theme;

public class ThemeConfig
{
    public string themePath { get; set; }
    public string imageFilename { get; set; }
    public int[] sunriseImageList { get; set; }
    public int[] dayImageList { get; set; }
    public int[] sunsetImageList { get; set; }
    public int[] nightImageList { get; set; }


    public static ThemeConfig Load(string themePath)
    {
        string jsonPath = Path.Combine(themePath, "theme.json");

        ThemeConfig theme;

        theme = JsonConvert.DeserializeObject<ThemeConfig>(File.ReadAllText(jsonPath));

        theme.themePath = themePath;
        return theme;
    }
}
