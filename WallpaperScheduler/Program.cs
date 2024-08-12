using System.Reflection;
using WallpaperScheduler.Theme;

namespace WallpaperScheduler;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Invalid args!");
            return;
        }

        try
        {
            Config.Load(args[0]);

  
            Globals.LoadAppearanceHandler();
            
            var theme = ThemeConfig.Load(Globals.config.ThemePath);
            var scheduler = new EventScheduler(theme);
            scheduler.RunAndUpdateLocation(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        Console.WriteLine("done"); 
        Console.ReadKey();
    }
}