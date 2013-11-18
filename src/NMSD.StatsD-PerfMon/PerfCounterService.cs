using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NMSD.StatsDClient;

namespace NMSD.StatsD_PerfMon
{
    public class PerfCounterService : ScheduledServiceBase
    {
        private readonly Lazy<Dictionary<string, PerformanceCounter>> counters;

        private readonly IStatsd statsD;

        public PerfCounterService(Func<IEnumerable<CounterDefinition>> getDefinitions, string statsDHost, int statsDPort = 8125)
        {
            defaultTickTimeSpan = TimeSpan.FromSeconds(10);
            initialDelay = TimeSpan.FromSeconds(10);

            var statsPrefix = string.Format("monitor.{0}", Environment.MachineName);

            statsD = new Statsd(statsDHost, statsDPort, statsPrefix);

            counters = new Lazy<Dictionary<string, PerformanceCounter>>(() => CreateCounters(getDefinitions()));
        }

        public override void Stop()
        {
            base.Stop();
            ZeroAllStats(); // Guages in statsD retain their last value so set everything to 0 when stopping so we can see there is no data
        }

        protected override void DoWork()
        {
            foreach (var keyValuePair in counters.Value)
            {
                var counter = keyValuePair.Value;
                var statsName = keyValuePair.Key;
                statsD.LogGauge(statsName, (int)counter.NextValue());
            }
        }

        private Dictionary<string, PerformanceCounter> CreateCounters(IEnumerable<CounterDefinition> definitions)
        {
            var counters = new Dictionary<string, PerformanceCounter>();
            foreach (var definition in definitions)
            {
                try
                {
                    if (System.Diagnostics.PerformanceCounterCategory.Exists(definition.CategoryName))
                    {
                        var counter = new PerformanceCounter(definition.CategoryName, definition.CounterName, definition.InstanceName);
                        counters.Add(definition.StatName, counter);
                    }
                }
                catch (Exception) { }
            }
            return counters;
        }

        private void ZeroAllStats()
        {
            foreach (var keyValuePair in counters.Value)
            {
                var statsName = keyValuePair.Key;
                statsD.LogGauge(statsName, 0);
            }
        }

    }
}