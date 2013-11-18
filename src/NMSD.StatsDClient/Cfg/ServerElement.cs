using System.Configuration;

namespace NMSD.StatsDClient.Cfg
{
    public class ServerElement : ConfigurationElement
    {

        [ConfigurationProperty("host", DefaultValue = "localhost", IsRequired = true)]
        public string Host
        {
            get { return (string)this["host"]; }
            set { this["host"] = value; }
        }

        [ConfigurationProperty("port", DefaultValue = "8125", IsRequired = false)]
        public int Port
        {
            get { return (int)this["port"]; }
            set { this["port"] = value; }
        }

        [ConfigurationProperty("prefix", DefaultValue = "", IsRequired = false)]
        public string Prefix
        {
            get { return (string)this["prefix"]; }
            set { this["prefix"] = value; }
        }
    }
}