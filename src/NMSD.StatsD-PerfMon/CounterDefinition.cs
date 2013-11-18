namespace NMSD.StatsD_PerfMon
{
    public class CounterDefinition
    {
        public string StatName { get; set; }

        public string CategoryName { get; set; }
        public string CounterName { get; set; }
        public string InstanceName { get; set; }
    }
}