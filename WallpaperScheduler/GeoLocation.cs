using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WallpaperScheduler;

class GeoLocation
{
    public class GeoPosition
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    private static async Task<GeoPosition> UnsafeUpdateGeoposition()
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync($"https://ipinfo.io/json?token={Globals.config.IpInfoToken}");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(json);

                string loc = jsonObject["loc"].ToString();
                string[] coords = loc.Split(',');

                double latitude = double.Parse(coords[0]);
                double longitude = double.Parse(coords[1]);

                return new GeoPosition
                {
                    Latitude = latitude,
                    Longitude = longitude
                };
            }

            throw new Exception($"Can't get GEO info: {response.StatusCode}");
        }
    }

    public static async Task<bool> UpdateGeoPosition()
    {
        try
        {
            var pos = await UnsafeUpdateGeoposition();
            Globals.latitude = pos.Latitude;
            Globals.longitude = pos.Longitude;

            return true;
        }
        catch(Exception e)
        {
            throw e;
        }
    }
}