using RmobDatVisualizet;
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

        if (args.Length != 1)
        {
            Console.WriteLine("Usage: Program <path_to_csv>");
            return;
        }

        string csvPath = args[0];

        if (!File.Exists(csvPath))
        {
            Console.WriteLine("The specified file does not exist.");
            return;
        }

        var data = ReadCsv(csvPath);
        if (data == null || data.Count == 0)
        {
            Console.WriteLine("No valid data found in the CSV file.");
            return;
        }

        GenerateImage(data, csvPath);
    }

    static List<(DateTime dateTime, int hour, int count)> ReadCsv(string path)
    {
        var data = new List<(DateTime dateTime, int hour, int count)>();

        foreach (var line in File.ReadLines(path))
        {
            var parts = line.Split(',');
            if (parts.Length != 3) continue;

            if (DateTime.TryParseExact(parts[0].Trim(), "yyyyMMddHH", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime) &&
                int.TryParse(parts[1].Trim(), out int hour) &&
                int.TryParse(parts[2].Trim(), out int count))
            {
                data.Add((dateTime, hour, count));
            }
        }

        return data;
    }

    static void GenerateImage(List<(DateTime dateTime, int hour, int count)> data, string csvPath)
    {
        int maxCount = data.Max(d => d.count);
        string fileName = Path.GetFileNameWithoutExtension(csvPath);
        DateTime firstDate = data.First().dateTime;
        int daysInMonth = DateTime.DaysInMonth(firstDate.Year, firstDate.Month);
        int cellSize = 20;
        int cellPadding = 3;
        int totalCellSize = cellSize + cellPadding;

        int width = daysInMonth * totalCellSize;
        int height = 24 * totalCellSize;

        int marginLeft = 60;  // Left margin for labels
        int marginTop = 40;
        int marginRight = 80; // Right margin for scale

        Bitmap bitmap = new Bitmap(width + marginLeft + marginRight, height + marginTop);
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.White);

            // Draw the grid background
            g.FillRectangle(Brushes.Black, marginLeft, marginTop, width, height);

            // Draw the data
            foreach (var item in data)
            {
                int x = marginLeft + (item.dateTime.Day - 1) * totalCellSize;
                int y = marginTop + item.hour * totalCellSize;
                Color color = GetColorForValue(item.count, maxCount);
                g.FillRectangle(new SolidBrush(color), x, y, cellSize, cellSize);
            }

            using (Font font = new Font("Arial", 15, FontStyle.Bold))
            {
                using (Brush brush = new SolidBrush(Color.Black))
                {
                    // Add hour labels
                    g.DrawString("0h", font, brush, marginLeft - 40, marginTop);
                    g.DrawString("6h", font, brush, marginLeft - 40, marginTop + 6 * totalCellSize);
                    g.DrawString("12h", font, brush, marginLeft - 40, marginTop + 12 * totalCellSize);
                    g.DrawString("18h", font, brush, marginLeft - 40, marginTop + 18 * totalCellSize);
                    g.DrawString("23h", font, brush, marginLeft - 40, marginTop + 23 * totalCellSize);

                    // Add day labels
                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        if (day == 1 || day == 5 || day == 10 || day == 15 || day == 20 || day == 25 || day == daysInMonth)
                        {
                            if (day < 10)
                                g.DrawString(day.ToString(), font, brush, marginLeft + (day - 1) * totalCellSize + cellPadding, 5);
                            else
                                g.DrawString(day.ToString(), font, brush, marginLeft + (day - 1) * totalCellSize + cellPadding - 7, 5);

                        }
                    }

                    // Draw the scale
                    int scaleX = marginLeft + width + 10;
                    int scaleYStart = marginTop;
                    int scaleYEnd = marginTop + height;
                    int scaleStep = (height - totalCellSize) / 23; // 24 values, 23 gaps

                    g.FillRectangle(Brushes.Black, scaleX - cellPadding, marginTop - cellPadding, totalCellSize + cellPadding, 24 * totalCellSize + cellPadding);
                    int i2 = 23;
                    // Draw 24 values with scale
                    for (int i = 0; i < 24; i++)
                    {
                        int y = scaleYStart + i2-- * scaleStep;
                        g.FillRectangle(new SolidBrush(GetColorForValue(i, 24)), scaleX, y, cellSize, cellSize);
                    }

                    g.DrawString(maxCount.ToString(), font, brush, scaleX + cellSize, scaleYStart - 3);
                    g.DrawString((maxCount / 2).ToString(), font, brush, scaleX + cellSize + 5, scaleYStart + (height / 2 - totalCellSize / 2) - 15);
                    g.DrawString("0", font, brush, scaleX + cellSize + 5, scaleYEnd - totalCellSize);
                }
            }
        }


        string outputFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}.png";
        bitmap.Save(outputFileName);
        Console.WriteLine($"Image saved as {outputFileName}");
    }

    static Color GetColorForValue(int value, int max)
    {              
        decimal percent = value / (decimal)max;
        int position = (int)Math.Floor(percent * (Scales.RmobColors.Count() - 1));
        return Scales.RmobColors[position];
    }  

}