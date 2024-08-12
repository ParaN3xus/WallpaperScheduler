// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WallpaperScheduler.Solar;
using WallpaperScheduler.Theme;

namespace WallpaperScheduler
{
    public enum AppearanceMode
    {
        Light,
        Dark,
        Default
    }

    public class DisplayEvent
    {
        public int DaySegment2;
        public int? DaySegment4;
        public int ImageId;
        public string LastImagePath;
        public DateTime NextUpdateTime;
        public AppearanceMode AppearanceMode;
    }

    class EventScheduler
    {
        private System.Timers.Timer backgroundTimer = new();
        private System.Timers.Timer schedulerTimer = new();
        private const long timerError = (long)(TimeSpan.TicksPerMillisecond * 15.6);

        private List<DisplayEvent> displayEvents;
        private DateTime? nextUpdateTime;

        private ThemeConfig currentTheme;

        public EventScheduler(ThemeConfig currentTheme)
        {
            this.currentTheme = currentTheme;

            backgroundTimer.AutoReset = true;
            backgroundTimer.Interval = 60e3;
            backgroundTimer.Elapsed += OnBackgroundTimerElapsed;
            backgroundTimer.Start();

            schedulerTimer.Elapsed += OnSchedulerTimerElapsed;
        }

        public bool Run(bool forceImageUpdate = false)
        {
            // ensure location and pos ready
            if (displayEvents == null || forceImageUpdate)
            {
                displayEvents = new List<DisplayEvent> { null, null };
            }

            schedulerTimer.Stop();
            SolarData data = SunriseSunsetService.GetSolarData(DateTime.Today);
            LoggingHandler.LogMessage("Calculated solar data: {0}", data);
            for (int i = 0; i < displayEvents.Count; i++)
            {
                if (displayEvents[i] == null)
                {
                    displayEvents[i] = new DisplayEvent();
                }
                else if (forceImageUpdate)
                {
                    displayEvents[i].LastImagePath = null;
                }

                SolarScheduler.CalcNextUpdateTime(data, displayEvents[i], currentTheme);
                LoggingHandler.LogMessage("Updated display event: {0}", displayEvents[i]);

                if (currentTheme != null)
                {
                    HandleDisplayEvent(displayEvents[i]);
                }
            }

            nextUpdateTime = SolarScheduler.CalcNextUpdateTime(data);
            StartTimer(nextUpdateTime.Value);
            return true;
        }

        public void RunAndUpdateLocation(bool forceImageUpdate = false)
        {
            // bool result = Run(forceImageUpdate);

            if (GeoLocation.UpdateGeoPosition().GetAwaiter().GetResult())
            {
                Run(forceImageUpdate); // Update wallpaper again if location has changed
            }
            else
            {
                throw new Exception("Update GEO position info field!");
            }
        }

        private void HandleDisplayEvent(DisplayEvent e)
        {
            string imagePath = e.LastImagePath;
            if (currentTheme != null)
            {
                string imageFilename = currentTheme.imageFilename.Replace("*", e.ImageId.ToString());
                imagePath = Path.Combine(currentTheme.themePath, imageFilename);
                if (imagePath == e.LastImagePath)
                {
                    imagePath = string.Empty;
                }
            }

            LoggingHandler.LogMessage("Setting wallpaper to {0}", imagePath);
            try
            {
                if (imagePath != String.Empty)
                {
                    Globals.SetWallpaper(imagePath);
                    e.LastImagePath = imagePath;
                }

                if (e.AppearanceMode != AppearanceMode.Default)
                {
                    Globals.SetAppearanceMode(e.AppearanceMode ==
                                              AppearanceMode.Dark);
                }
            }
            catch (Exception exc)
            {
                LoggingHandler.LogMessage("Error setting wallpaper: {0}", exc.ToString());
            }
        }

        private void StartTimer(DateTime futureTime)
        {
            long intervalTicks = futureTime.Ticks - DateTime.Now.Ticks;
            if (intervalTicks < timerError)
            {
                intervalTicks = 1;
            }

            TimeSpan interval = new TimeSpan(intervalTicks);
            schedulerTimer.Interval = interval.TotalMilliseconds;
            schedulerTimer.Start();
            LoggingHandler.LogMessage("Started timer for {0:0.000} sec", interval.TotalSeconds);
        }

        private void HandleTimerEvent(bool updateLocation)
        {
            if (updateLocation)
            {
                RunAndUpdateLocation();
            }
            else
            {
                Run();
            }
        }

        private void OnBackgroundTimerElapsed(object sender, EventArgs e)
        {
            if (nextUpdateTime.HasValue && DateTime.Now >= nextUpdateTime.Value)
            {
                LoggingHandler.LogMessage("Scheduler event triggered by timer 2");
                HandleTimerEvent(true);
            }
        }

        private void OnSchedulerTimerElapsed(object sender, EventArgs e)
        {
            LoggingHandler.LogMessage("Scheduler event triggered by timer 1");
            HandleTimerEvent(true);
        }
    }
}