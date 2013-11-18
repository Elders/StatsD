using System;
using System.ServiceProcess;
using System.Threading;

namespace NMSD.StatsD_PerfMon
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            var service = new PerfMonService();
            service.Debug();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new PerfMonService() 
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}