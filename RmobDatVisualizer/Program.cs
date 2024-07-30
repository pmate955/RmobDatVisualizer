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

    static void DrawHourLabels(Graphics g, Font font, Brush brush, int marginLeft, int marginTop, int cellSize, int totalCellSize)
    {
        // Add hour labels
        g.DrawString("0h", font, brush, marginLeft - 40, marginTop);
        g.DrawString("6h", font, brush, marginLeft - 40, marginTop + 6 * totalCellSize);
        g.DrawString("12h", font, brush, marginLeft - 40, marginTop + 12 * totalCellSize);
        g.DrawString("18h", font, brush, marginLeft - 40, marginTop + 18 * totalCellSize);
        g.DrawString("23h", font, brush, marginLeft - 40, marginTop + 23 * totalCellSize);
    }

    static void DrawDayLabels(Graphics g, Font font, Brush brush, int marginLeft, int daysInMonth, int totalCellSize)
    {
        // Add day labels
        for (int day = 1; day <= daysInMonth; day++)
        {
            if (day == 1 || day == 5 || day == 10 || day == 15 || day == 20 || day == 25 || day == daysInMonth)
            {
                if (day < 10)
                    g.DrawString(day.ToString(), font, brush, marginLeft + (day - 1) * totalCellSize, 5);
                else
                    g.DrawString(day.ToString(), font, brush, marginLeft + (day - 1) * totalCellSize - 7, 5);

            }
        }
    }

    static void DrawScale(Graphics g, Font font, Brush brush, int marginLeft, int marginTop, int width, int height, int cellSize, int totalCellSize, int minCount, int maxCount, int cellPadding)
    {
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

    static void DrawBarChart(Graphics g, List<(DateTime dateTime, int count)> data, Brush brush, int marginLeft, int marginTop, int cellSize, int totalCellSize, int bitmapHeight)
    {
        Font font = new Font("Arial", 10);
        int daysInMonth = DateTime.DaysInMonth(data.First().dateTime.Year, data.First().dateTime.Month);
        int[] daySums = new int[daysInMonth];

        // Calculate sum for each day
        foreach (var item in data)
        {
            daySums[item.dateTime.Day - 1] += item.count;
        }

        int barWidth = 30; // Width of each bar
        int barMargin = 5; // Margin between bars
        int maxBarHeight = 300; // Max height for the bars
        int maxDaySum = daySums.Max();

        // Adjust height of the bitmap to accommodate the bar chart
        int chartHeight = maxBarHeight + 60; // Extra space for labels

        // Draw the bar chart on the main bitmap
        int chartMarginTop = bitmapHeight - chartHeight - 50; // Position the chart below the main grid
        int chartWidth = daysInMonth * (barWidth + barMargin) + 60;

        // Draw the bar chart background
        g.FillRectangle(Brushes.White, marginLeft, chartMarginTop, chartWidth, chartHeight);

        // Draw bars
        for (int i = 0; i < daysInMonth; i++)
        {
            int barHeight = (int)((double)daySums[i] / maxDaySum * maxBarHeight);
            int x = marginLeft + i * (barWidth + barMargin) + 20;
            int y = chartMarginTop + chartHeight - barHeight - 30;
            g.FillRectangle(Brushes.Blue, x, y, barWidth, barHeight);

            // Draw labels
            g.DrawString(daySums[i].ToString(), font, brush, x + barWidth / 2 - 10, y - 20);
        }

        // Draw x-axis labels
        for (int i = 1; i <= daysInMonth; i++)
        {
            if (i % 5 == 0 || i == 1)
            {
                g.DrawString(i.ToString(), font, brush, marginLeft + i * (barWidth + barMargin) - 10, chartMarginTop + chartHeight - 20);
            }
        }

        // Draw y-axis
        g.DrawLine(Pens.Black, marginLeft, chartMarginTop, marginLeft, chartMarginTop + chartHeight); // Y-axis
        g.DrawLine(Pens.Black, marginLeft, chartMarginTop + chartHeight, marginLeft + chartWidth, chartMarginTop + chartHeight); // X-axis

        // Add title to the bar chart
        g.DrawString("Daily Totals", font, brush, marginLeft + 10, chartMarginTop - 20);
    }


    static void GenerateImage(List<(DateTime dateTime, int hour, int count)> data, string csvPath)
    {
        int maxCount = data.Max(d => d.count);
        DateTime firstDate = data.First().dateTime;
        int daysInMonth = DateTime.DaysInMonth(firstDate.Year, firstDate.Month);
        int cellSize = 20;
        int cellPadding = 3;
        int totalCellSize = cellSize + cellPadding;

        int width = daysInMonth * totalCellSize;
        int height = 24 * totalCellSize;

        int marginLeft = 250;  // Left margin for labels and bar chart
        int marginTop = 40;
        int marginRight = 100; // Right margin for scale

        Bitmap bitmap = new Bitmap(width + marginLeft + marginRight + 200, height + marginTop + 400);
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
                    DrawHourLabels(g, font, brush, marginLeft, marginTop, cellSize, totalCellSize);
                    DrawDayLabels(g, font, brush, marginLeft, daysInMonth, totalCellSize);
                    DrawScale(g, font, brush, marginLeft, marginTop, width, height, cellSize, totalCellSize, 0, maxCount, cellPadding);
                    DrawBarChart(g, data.Select(d => (d.dateTime, d.count)).ToList(), brush, 50, marginTop + height + 300, cellSize, totalCellSize, height + marginTop + 450);

                }
            }
        }

        string outputFileName = $"{Path.GetFileNameWithoutExtension(csvPath)}_{DateTime.Now:yyyyMMddHHmmss}.png";
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