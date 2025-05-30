using RmobDatVisualizer.Service;
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
            string path = AppDomain.CurrentDomain.BaseDirectory;

            if (args.Length != 1)
            {
                Console.WriteLine("Usage: Program <path_to_csv>");
                Console.WriteLine($"I will use the program path! {path}");
            } 
            else
            {
                Console.WriteLine($"Path: {path}");
                path = args[0];
            }

            FileAttributes attr = File.GetAttributes(path);

            Bitmap res = new Bitmap(10, 10);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                res = ProcessDirectory(path);
            }
            else
            {
                res = ProcessSingleFile(path);
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

    static Bitmap ProcessSingleFile(string path)
    {
        Console.WriteLine("Read file");

        if (!File.Exists(path))
            throw new Exception("The specified file does not exist.");
            
        var data = CsvHelper.ReadCsv(path, out int max);
        
        if (data == null || data.Count == 0)
            throw new Exception("No valid data found in the CSV file.");

        return VisualizationHelper.GenerateRmobImage(data, max, Scales.RmobColors);
    }

    static Bitmap ProcessDirectory(string path)
    {
        int countByRow = 6;

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
            var data = CsvHelper.ReadCsv(paths[i], out int localMax);

            if (data == null || data.Count == 0)
            {
                throw new Exception("Invalid input");
            }

            if (localMax > maxCount)
                maxCount = localMax;

            csvData.Add(data);
        }

        for (int i = 0; i < paths.Count; i++)
        {
            var gen = VisualizationHelper.GenerateRmobImage(csvData[i], maxCount, Scales.RmobColors, i % countByRow == 0, (i % countByRow == countByRow - 1 && i > 0) || i == paths.Count - 1);
            graphList.Add(gen);
        }

        return VisualizationHelper.MergeImages(graphList, countByRow);
    }    
}