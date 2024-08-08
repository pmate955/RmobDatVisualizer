using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RmobDatVisualizer
{
    public struct AggregatedData
    {
        public AggregatedData(DateTime dateTime, int hour, int count)
        {
            this.dateTime = dateTime;
            this.hour = hour;
            this.count = count;
        }

        public DateTime dateTime;
        public int hour;
        public int count;

    }
}
