using RmobDatVisualizer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

internal class Program
{
    
    private static void Main(string[] args)
    {
        try
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: Program <path_to_csv>");
                return;
            }

            string path = args[0];

            FileAttributes attr = File.GetAttributes(path);

            Bitmap res = new Bitmap(10, 10);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                Console.WriteLine("Read directory");

                var paths = Directory.GetFiles(path, "RMOB-*.dat")
                        .Select(Path.GetFileName)
                        .OrderBy(fileName => fileName)
                        .ToList();

                List<Bitmap> graphList = new List<Bitmap>();
                List<List<AggregatedData>> csvData = new List<List<AggregatedData>>();
                int maxCount = 0;

                for (int i = 0; i < paths.Count; i++)
                {
                    var data = ReadCsv(paths[i], out int localMax);

                    if (data == null || data.Count == 0)
                    {
                        Console.WriteLine("No valid data found in the CSV file.");
                        return;
                    }
                    
                    if (localMax > maxCount)
                        maxCount = localMax;

                    csvData.Add(data);
                }

                for (int i = 0; i < paths.Count; i++)
                {
                    var gen = VisualizationHelper.GenerateImage(csvData[i], paths[i], maxCount, i % 6 == 0, (i % 6 == 0 && i > 0) || i == paths.Count - 1);
                    graphList.Add(gen);
                    res =  VisualizationHelper.MergeImages(graphList);
                }
            }
            else
            {
                Console.WriteLine("Read file");

                if (!File.Exists(path))
                {
                    Console.WriteLine("The specified file does not exist.");
                    return;
                }

                var data = ReadCsv(path, out int max);
                if (data == null || data.Count == 0)
                {
                    Console.WriteLine("No valid data found in the CSV file.");
                    return;
                }

                res = VisualizationHelper.GenerateImage(data, path, max);
            }

            string outputFileName = $"{Path.GetFileNameWithoutExtension(path)}_{DateTime.Now:yyyyMMddHHmmss}.png";
            res.Save(outputFileName);
            Console.WriteLine($"Image saved as {outputFileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Data);
            throw;
        }
    }

    static List<AggregatedData> ReadCsv(string path, out int max)
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