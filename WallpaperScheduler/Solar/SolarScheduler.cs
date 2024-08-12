using System;
using System.Collections.Generic;
using System.Linq;
using WallpaperScheduler.Theme;

namespace WallpaperScheduler.Solar;

public enum DaySegment
{
    Sunrise,
    Day,
    Sunset,
    Night,
    AlwaysDay,
    AlwaysNight
}

public class DaySegmentData
{
    public DaySegment segmentType;
    public int segment2;
    public int segment4;

    public DaySegmentData(DaySegment segmentType, int segment2, int segment4)
    {
        this.segmentType = segmentType;
        this.segment2 = segment2;
        this.segment4 = segment4;
    }
}

class SolarScheduler
{
    public static DaySegmentData GetDaySegmentData(SolarData data, DateTime time)
    {
        int daySegment2 = (data.sunriseTime <= DateTime.Now && DateTime.Now < data.sunsetTime) ? 0 : 1;

        if (data.polarPeriod == PolarPeriod.PolarDay)
        {
            return new DaySegmentData(DaySegment.AlwaysDay, 0, 1);
        }

        if (data.polarPeriod == PolarPeriod.PolarNight)
        {
            return new DaySegmentData(DaySegment.AlwaysNight, 1, 3);
        }

        if ((data.solarTimes[0] <= time && time < data.solarTimes[1]) ||
            (data.polarPeriod == PolarPeriod.CivilPolarDay && DateTime.Now > data.solarTimes[3]))
        {
            return new DaySegmentData(DaySegment.Sunrise, daySegment2, 0);
        }

        if (data.solarTimes[1] <= time && time < data.solarTimes[2])
        {
            return new DaySegmentData(DaySegment.Day, daySegment2, 1);
        }

        if ((data.solarTimes[2] <= time && time < data.solarTimes[3]) ||
            (data.polarPeriod == PolarPeriod.CivilPolarDay && DateTime.Now < data.solarTimes[0]))
        {
            return new DaySegmentData(DaySegment.Sunset, daySegment2, 2);
        }

        return new DaySegmentData(DaySegment.Night, daySegment2, 3);
    }

    public static DateTime CalcNextUpdateTime(SolarData data)
    {
        if (data.IsPolarPeriodTotal())
        {
            return DateTime.Today.AddDays(1);
        }

        if (data.sunriseTime <= DateTime.Now && DateTime.Now < data.sunsetTime)
        {
            return data.sunsetTime;
        }

        if (DateTime.Now < data.solarTimes[0])
        {
            return data.sunriseTime;
        }

        SolarData tomorrowsData = SunriseSunsetService.GetSolarData(DateTime.Today.AddDays(1));
        return tomorrowsData.sunriseTime;
    }

    public static void CalcNextUpdateTime(SolarData data, DisplayEvent e, ThemeConfig currentTheme)
    {
        AppearanceMode mode;
        int[] imageList;
        DateTime segmentStart;
        DateTime segmentEnd;
        DateTime dateNow = DateTime.Now;
        DaySegmentData segmentData = GetDaySegmentData(data, dateNow);
        e.DaySegment2 = segmentData.segment2;

        bool preferSegment2 = false;
        if (currentTheme != null)
        {
            if ((segmentData.segmentType == DaySegment.Sunrise || segmentData.segmentType == DaySegment.Night) &&
                Globals.IsNullOrEmpty(currentTheme.sunriseImageList))
            {
                preferSegment2 = true;
            }
            else if ((segmentData.segmentType == DaySegment.Sunset || segmentData.segmentType == DaySegment.Day) &&
                     Globals.IsNullOrEmpty(currentTheme.sunsetImageList))
            {
                preferSegment2 = true;
            }
        }
        
        if (dateNow > data.sunsetTime || dateNow < data.sunriseTime)
        {
            e.AppearanceMode = AppearanceMode.Dark;
        }
        else
        {
            e.AppearanceMode = AppearanceMode.Light;
        }

        if (data.IsPolarPeriodTotal())
        {
            imageList = currentTheme?.dayImageList;
            if (data.polarPeriod == PolarPeriod.PolarNight)
            {
                imageList = currentTheme?.nightImageList;
            }

            segmentStart = data.solarNoon.AddHours(-12);
            segmentEnd = data.solarNoon.AddHours(12).AddTicks(-1);
        }
        else if (!preferSegment2)
        {
            e.DaySegment4 = segmentData.segment4;

            switch (segmentData.segmentType)
            {
                case DaySegment.Sunrise:
                    if (dateNow > data.solarTimes[3])
                    {
                        data = SunriseSunsetService.GetSolarData(dateNow.Date.AddDays(1));
                    }

                    imageList = currentTheme?.sunriseImageList;
                    segmentStart = data.solarTimes[0];
                    segmentEnd = data.solarTimes[1];
                    break;
                case DaySegment.Day:
                    imageList = currentTheme?.dayImageList;
                    segmentStart = data.solarTimes[1];
                    segmentEnd = data.solarTimes[2];
                    break;
                case DaySegment.Sunset:
                    if (dateNow < data.solarTimes[0])
                    {
                        data = SunriseSunsetService.GetSolarData(dateNow.Date.AddDays(-1));
                    }

                    imageList = currentTheme?.sunsetImageList;
                    segmentStart = data.solarTimes[2];
                    segmentEnd = data.solarTimes[3];
                    break;
                default:
                    imageList = currentTheme?.nightImageList;

                    if (dateNow < data.solarTimes[0])
                    {
                        SolarData yesterdaysData = SunriseSunsetService.GetSolarData(dateNow.Date.AddDays(-1));
                        segmentStart = yesterdaysData.solarTimes[3];
                        segmentEnd = data.solarTimes[0];
                    }
                    else
                    {
                        segmentStart = data.solarTimes[3];
                        SolarData tomorrowsData = SunriseSunsetService.GetSolarData(dateNow.Date.AddDays(1));
                        segmentEnd = tomorrowsData.solarTimes[0];
                    }

                    break;
            }
        }
        else
        {
            imageList = currentTheme?.dayImageList;
            if (segmentData.segment2 == 1)
            {
                imageList = currentTheme?.nightImageList;
            }

            if (segmentData.segment2 == 0)
            {
                segmentStart = data.sunriseTime;
                segmentEnd = data.sunsetTime;
            }
            else if (dateNow < data.sunriseTime)
            {
                SolarData yesterdaysData = SunriseSunsetService.GetSolarData(dateNow.Date.AddDays(-1));
                segmentStart = yesterdaysData.sunsetTime;
                segmentEnd = data.sunriseTime;
            }
            else
            {
                segmentStart = data.sunsetTime;
                SolarData tomorrowsData = SunriseSunsetService.GetSolarData(dateNow.Date.AddDays(1));
                segmentEnd = tomorrowsData.sunriseTime;
            }
        }

        if (imageList != null)
        {
            TimeSpan imageDuration = new TimeSpan((segmentEnd - segmentStart).Ticks / imageList.Length);
            int imageNumber = (int)((dateNow.Ticks - segmentStart.Ticks) / imageDuration.Ticks);
            e.ImageId = imageList[imageNumber];
            e.NextUpdateTime = new DateTime(segmentStart.Ticks + imageDuration.Ticks * (imageNumber + 1));
        }
    }
}