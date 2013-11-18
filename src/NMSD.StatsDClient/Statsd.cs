using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using NMSD.StatsDClient.Cfg;

namespace NMSD.StatsDClient
{
    public class XmlConfiguration { }

    public class Statsd : IStatsd
    {
        private bool hasPrefix;

        private string prefix;

        private UdpChannel udpChannel;

        public Statsd(string host, int port, string prefix = null)
        {
            this.prefix = prefix;
            hasPrefix = !String.IsNullOrEmpty(prefix);
            udpChannel = new UdpChannel(host, port);
            Current = this;
        }

        public Statsd(XmlConfiguration xml)
        {
            StatsDConfigurationSection cfg = (StatsDConfigurationSection)ConfigurationManager.GetSection("statsD");
            UdpChannel channel = new UdpChannel(cfg.Server.Host, cfg.Server.Port);
            this.prefix = cfg.Server.Prefix;
            hasPrefix = !String.IsNullOrEmpty(prefix);
            Current = this;
        }

        public static Statsd Current { get; private set; }

        public void Dispose()
        {
            try
            {
                if (udpChannel != null)
                    udpChannel.Dispose();
            }
            catch { }
        }

        public void LogCount(string name, int count = 1)
        {
            SendMetric(MetricType.COUNT, name, prefix, count);
        }

        public void LogGauge(string name, int value)
        {
            SendMetric(MetricType.GAUGE, name, prefix, value);
        }

        public void LogRaw(string name, int value, long? epoch = null)
        {
            SendMetric(MetricType.RAW, name, String.Empty, value, epoch.HasValue ? epoch.ToString() : (string)null);
        }

        public void LogSet(string name, int value)
        {
            SendMetric(MetricType.SET, name, prefix, value);
        }

        public void LogTiming(string name, int milliseconds)
        {
            SendMetric(MetricType.TIMING, name, prefix, milliseconds);
        }

        public TimingToken LogTiming(string name)
        {
            return new TimingToken(this, name);
        }

        public void LogTiming(string name, long milliseconds)
        {
            LogTiming(name, (int)milliseconds);
        }

        string PrepareMetric(string metricType, string name, string prefix, int value, string postFix = null)
        {
            StringBuilder metricBuilder = new StringBuilder();
            if (hasPrefix)
            {
                metricBuilder.Append(prefix);
                metricBuilder.Append(".");
                metricBuilder.Append(name);
            }
            else
            {
                metricBuilder.Append(name);
            }
            metricBuilder.Append(":"); metricBuilder.Append(value);
            metricBuilder.Append("|"); metricBuilder.Append(metricType);
            if (postFix != null)
            {
                metricBuilder.Append("|"); metricBuilder.Append(postFix);
            }
            return metricBuilder.ToString();
        }

        void SendMetric(string metricType, string name, string prefix, int value, string postFix = null)
        {
            if (value < 0 || String.IsNullOrEmpty(name))
                return;

            udpChannel.Send(PrepareMetric(metricType, name, prefix, value, postFix));
        }

        class UdpChannel
        {
            private UdpClient udpClient;

            public UdpChannel(string host, int port)
            {
                var ipv4Address = System.Net.Dns.GetHostAddresses(host).First(p => p.AddressFamily == AddressFamily.InterNetwork);
                udpClient = new UdpClient();
                udpClient.Connect(ipv4Address, port);
            }

            public void Dispose()
            {
                if (udpClient != null)
                    udpClient.Close();
            }

            public void Send(string line)
            {
                byte[] payload = Encoding.UTF8.GetBytes(line);
                udpClient.BeginSend(payload, payload.Length, null, null);
            }

        }

        static class MetricType
        {
            public const string COUNT = "c";
            public const string GAUGE = "g";
            public const string RAW = "r";
            public const string SET = "s";
            public const string TIMING = "ms";
        }

    }

    public sealed class TimingToken : IDisposable
    {
        private IStatsd client;
        private string name;
        private Stopwatch stopwatch;

        internal TimingToken(IStatsd client, string name)
        {
            stopwatch = Stopwatch.StartNew();
            this.client = client;
            this.name = name;
        }

        public void Dispose()
        {
            stopwatch.Stop();
            client.LogTiming(name, (int)stopwatch.ElapsedMilliseconds);
        }
    }
}