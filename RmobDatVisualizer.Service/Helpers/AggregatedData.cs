using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RmobDatVisualizer.Service
{
    public struct AggregatedData
    {
        public AggregatedData(DateTime dateTime, int hour, int count)
        {
            this.EventDt = dateTime;
            this.Hour = hour;
            this.Count = count;
        }

        public DateTime EventDt;
        public int Hour;
        public int Count;

    }
}
