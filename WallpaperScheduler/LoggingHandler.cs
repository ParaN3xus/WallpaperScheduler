using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace WallpaperScheduler;

class LoggingHandler
{
    private static readonly object debugLogLock = new object();

    public static void LogMessage(string message, params object[] values)
    {
        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
        if (values.Length > 0)
        {
            for (int i = 0; i < values.Length; i++)
            {
#pragma warning disable SYSLIB0050
                if (!values[i].GetType().IsSerializable)
#pragma warning restore SYSLIB0050
                {
                    values[i] = JsonConvert.SerializeObject(values[i]);
                }
            }

            message = string.Format(message, values);
        }

        lock (debugLogLock)
        {
            Console.WriteLine(message);
            //File.AppendAllText("debug.log", string.Format("[{0}] {1}\n", timestamp, message));
        }
    }

}