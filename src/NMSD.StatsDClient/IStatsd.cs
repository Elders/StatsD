using System;

namespace NMSD.StatsDClient
{
    /// <summary>
    /// Interface for the statsd.net client
    /// </summary>
    public interface IStatsd : IDisposable
    {
        /// <summary>
        /// Logs a count for a metric
        /// </summary>
        void LogCount(string name, int count = 1);
        /// <summary>
        /// Logs a gauge value
        /// </summary>
        void LogGauge(string name, int value);
        /// <summary>
        /// Logs a latency / Timing
        /// </summary>
        void LogTiming(string name, int milliseconds);
        /// <summary>
        /// Logs a latency / Timing
        /// </summary>
        void LogTiming(string name, long milliseconds);
        /// <summary>
        /// Logs the number of unique occurrances of something
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void LogSet(string name, int value);
        /// <summary>
        /// Logs a raw metric that will not get aggregated on the server.
        /// </summary>
        /// <param name="name">The metric name.</param>
        /// <param name="value">The metric value.</param>
        /// <param name="epoch">(optional) The epoch timestamp. Leave this blank to have the server assign an epoch for you.</param>
        void LogRaw(string name, int value, long? epoch = null);
    }
}