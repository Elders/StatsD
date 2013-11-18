using System;
using System.Threading;

namespace NMSD.StatsD_PerfMon
{
    public abstract class ScheduledServiceBase
    {
        private Timer timer;
        protected TimeSpan defaultTickTimeSpan;
        protected TimeSpan initialDelay;
        private TimeSpan nextTickTimeSpan;
        private readonly TimeSpan maxNextTickTimeSpan;

        protected ScheduledServiceBase()
        {
            defaultTickTimeSpan = TimeSpan.FromSeconds(5);
            initialDelay = TimeSpan.Zero;
            nextTickTimeSpan = defaultTickTimeSpan;
            maxNextTickTimeSpan = TimeSpan.FromSeconds(300);
        }

        public void Start()
        {
            try
            {
                timer = new Timer(state => Tick());
                ScheduleNextOccurrence(initialDelay);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Tick()
        {
            try
            {
                DoWork();
                nextTickTimeSpan = defaultTickTimeSpan;
            }
            catch (Exception)
            {
                nextTickTimeSpan += nextTickTimeSpan;
                if (nextTickTimeSpan > maxNextTickTimeSpan)
                    nextTickTimeSpan = maxNextTickTimeSpan;
            }
            finally
            {
                ScheduleNextOccurrence(nextTickTimeSpan);
            }
        }

        protected abstract void DoWork();

        public virtual void Stop()
        {
            timer.Dispose();
        }

        public void ScheduleNextOccurrence(TimeSpan next)
        {
            var disablePeriodicSignalling = TimeSpan.FromMilliseconds(-1);
            timer.Change(next, disablePeriodicSignalling);
        }
    }
}