using System.Configuration;

namespace NMSD.StatsDClient.Cfg
{
    public class StatsDConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("server")]
        public ServerElement Server
        {
            get { return (ServerElement)this["server"]; }
            set { this["server"] = value; }
        }
    }
}