using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using NLog;

namespace Zalirun.Extentions
{
    public static class TimerManager
    {
        public static Dictionary<Guid, Timer> Timers { get; set; } = new Dictionary<Guid, Timer>();
        private static readonly ILogger s_logger = NLog.LogManager.GetCurrentClassLogger();
        public static Timer SetTimer(double interval, bool autoReset, out Guid timerId)
        {
            timerId = Guid.NewGuid();
            if (interval < 0 || interval >= 2147483647)
            {
                var emptyTimer = new Timer();
                s_logger.Error(new ArgumentException(), $"Timer interval argument exception, timer won't start : {interval}");
                Timers.Add(timerId, emptyTimer);
                return emptyTimer;
            }
            var timer = new Timer(interval)
            {
                AutoReset = autoReset,
                Enabled = true
            };
            var removeTimerId = timerId;
            timer.Elapsed += (sender, e) =>
            {
                if (!autoReset)
                {
                    ClearTimers(removeTimerId);
                }
            };
            Timers.Add(timerId, timer);
            s_logger.Info($"Created Timer Id - {timerId}, interval - {(DateTime.Now.AddMilliseconds(interval) - DateTime.Now)}");
            return timer;
        }
        public static void ClearAllTimers()
        {
            foreach (var timer in Timers.ToList())
            {
                timer.Value.Stop();
                timer.Value.Dispose();
                Timers.Remove(timer.Key);

            }
        }
        public static void ClearTimers(params Guid[] ids)
        {
            s_logger.Info($"Begin >> Clearing Timers");
            foreach (var id in ids)
            {
                if (!Timers.ContainsKey(id))
                {
                    s_logger.Warn($"Timer not found with id {id}");
                    continue;
                }
                var timer = Timers[id];
                timer.Stop();
                timer.Dispose();
                Timers.Remove(id);
                s_logger.Info($"Timer removed Id - {id}");
            }
            s_logger.Info($"End >> Clearing timers");
        }
        public static void ClearTimers(List<Guid> ids)
        {
            s_logger.Info($"Begin >> Clearing Timers");
            foreach (var id in ids)
            {
                if (!Timers.ContainsKey(id))
                {
                    s_logger.Warn($"Timer not found with id {id}");
                    continue;
                }
                var timer = Timers[id];
                timer.Stop();
                timer.Dispose();
                Timers.Remove(id);
                s_logger.Info($"Timer removed Id - {id}");
            }
            s_logger.Info($"End >> Clearing timers");
        }
    }
}
