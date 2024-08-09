using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RmobDatVisualizer
{
    public class CsvHelper
    {
        public static List<AggregatedData> ReadCsv(string path, out int max)
        {
            var data = new List<AggregatedData>();
            var maxCount = 0;

            foreach (var line in File.ReadLines(path))
            {
                var parts = line.Split(',');
                if (parts.Length != 3) continue;

                if (DateTime.TryParseExact(parts[0].Trim(), "yyyyMMddHH", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime) &&
                    int.TryParse(parts[1].Trim(), out int hour) &&
                    int.TryParse(parts[2].Trim(), out int count))
                {
                    data.Add(new AggregatedData(dateTime, hour, count));

                    if (count > maxCount)
                        maxCount = count;
                }
            }

            max = maxCount;

            return data;
        }
    }
}
