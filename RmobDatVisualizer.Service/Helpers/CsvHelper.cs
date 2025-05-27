using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RmobDatVisualizer.Service
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

        public static List<List<AggregatedData>> GetDataForImage(List<string> paths, out int maxCount)
        {
            var csvData = new List<List<AggregatedData>>();
            maxCount = 0;

            for (int i = 0; i < paths.Count; i++)
            {
                var data = ReadCsv(paths[i], out int localMax);

                if (data == null || data.Count == 0)
                {
                    throw new Exception("Invalid input or no data");
                }

                if (localMax > maxCount)
                    maxCount = localMax;

                csvData.Add(data);
            }

            return csvData;
        }
    }
}
