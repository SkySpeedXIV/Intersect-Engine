using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intersect.Server.Core;
using Intersect.Server.General;
using Intersect.Server.Maps;

namespace Intersect.Server
{
    public static class ServerLoop
    {
        public static void RunServerLoop()
        {
            long cpsTimer = Globals.Timing.TimeMs + 1000;
            long cps = 0;
            long minuteTimer = 0;
            long lastGameSave = Globals.Timing.TimeMs + 60000;
            DateTime lastDbUpdate = DateTime.Now;
            long dbBackupMinutes = 120;
            while (ServerContext.Instance.IsRunning)
            {
                //TODO: If there are no players online then loop slower and save the poor cpu
                var timeMs = Globals.Timing.TimeMs;
                var maps = MapInstance.Lookup.Values.ToArray();
                //TODO: Could be optimized by keeping a list of active maps or something
                foreach (MapInstance map in maps)
                {
                    map.Update(timeMs);
                }
                if (minuteTimer < timeMs)
                {
                    if (lastDbUpdate.AddMinutes(dbBackupMinutes) < DateTime.Now)
                    {
                        Task.Run(() => DbInterface.BackupDatabase());
                        lastDbUpdate = DateTime.Now;
                    }
                    DbInterface.SavePlayerDatabaseAsync();
                    minuteTimer = timeMs + 60000;
                }
                cps++;
                if (timeMs >= cpsTimer)
                {
                    Globals.Cps = cps;
                    cps = 0;
                    cpsTimer = timeMs + 1000;
                }
                ServerTime.Update();
                var currentTime = Globals.Timing.TimeMs;
                if (Globals.CpsLock && currentTime < timeMs + 10)
                {
                    int waitTime = (int) ((timeMs + 10) - currentTime);
                    Thread.Sleep(waitTime);
                }
                if (timeMs > lastGameSave)
                {
                    Task.Run(() => DbInterface.SaveGameDatabase());
                    lastGameSave = timeMs + 60000;
                }
            }

            ServerContext.Instance.RequestShutdown();
            //Server is shutting down!!
            //ServerStatic.Shutdown(false);
            //TODO gracefully disconnect all clients
        }
    }
}
