using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Program;

namespace RmobDatVisualizer
{
    public class VisualizationHelper
    {
        #region Public methods

        public static Bitmap GenerateImage(List<AggregatedData> data, int maxCount, bool hasLegend = true, bool hasScale = true)
        {
            DateTime firstDate = data.First().EventDt;
            int daysInMonth = DateTime.DaysInMonth(firstDate.Year, firstDate.Month);
            int cellSize = 20;
            int cellPadding = 3;
            int totalCellSize = cellSize + cellPadding;

            int width = daysInMonth * totalCellSize;
            int height = 24 * totalCellSize;

            int marginLeft = hasLegend ? 50 : 5;  // Left margin for labels and bar chart
            int marginTop = 100;
            int marginRight = hasScale ? 100 : 5; // Right margin for scale

            Bitmap bitmap = new Bitmap(width + marginLeft + marginRight, height + marginTop + 400);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);

                // Draw the grid background
                g.FillRectangle(Brushes.Black, marginLeft - cellPadding, marginTop - cellPadding, width + cellPadding, height + cellPadding);

                // Draw the data
                foreach (var item in data)
                {
                    int x = marginLeft + (item.EventDt.Day - 1) * totalCellSize;
                    int y = marginTop + item.Hour * totalCellSize;
                    Color color = Scales.GetColorForValue(item.Count, maxCount);
                    g.FillRectangle(new SolidBrush(color), x, y, cellSize, cellSize);
                }

                using (Font font = new Font("Arial", 15, FontStyle.Bold))
                {
                    using (Brush brush = new SolidBrush(Color.Black))
                    {
                        string title = data[0].EventDt.ToString("yyyy MMMM", CultureInfo.InvariantCulture);

                        g.DrawString(title, font, brush, width / 2.0f, 20);

                        if (hasLegend)
                            DrawHourLabels(g, font, brush, marginLeft, marginTop, totalCellSize);

                        DrawDayLabels(g, font, brush, marginLeft, marginTop - 30, daysInMonth, totalCellSize);

                        if (hasScale)
                            DrawScale(g, font, brush, marginLeft, marginTop, width, height, cellSize, totalCellSize, maxCount, cellPadding);

                        DrawLineChart(g, data, brush, 50, cellSize, height + marginTop + 450);

                    }
                }
            }

            return bitmap;
        }

        public static Bitmap MergeImages(List<Bitmap> images)
        {
            if (images.Count == 0)
                throw new ArgumentException("Not enough input!");

            int fixedHeight = images[0].Height;
            int totalWidth = 0;
            int currentRowWidth = 0;
            int rows = (int)Math.Ceiling(images.Count / 6.0);

            List<int> rowWidths = new List<int>();

            for (int i = 0; i < images.Count; i++)
            {
                currentRowWidth += images[i].Width;
                if ((i + 1) % 6 == 0 || i == images.Count - 1)
                {
                    rowWidths.Add(currentRowWidth);
                    totalWidth = Math.Max(totalWidth, currentRowWidth);
                    currentRowWidth = 0;
                }
            }

            Bitmap mergedBitmap = new Bitmap(totalWidth, fixedHeight * rows);
            using (Graphics g = Graphics.FromImage(mergedBitmap))
            {
                g.Clear(Color.Black);
                int x = 0, y = 0, imgIndex = 0;

                foreach (int rowWidth in rowWidths)
                {
                    x = 0;
                    for (int i = 0; i < 6 && imgIndex < images.Count; i++, imgIndex++)
                    {
                        g.DrawImage(images[imgIndex], x, y);
                        x += images[imgIndex].Width;
                    }
                    y += fixedHeight;
                }
            }

            return mergedBitmap;
        }

        #endregion

        static void DrawHourLabels(Graphics g, Font font, Brush brush, int marginLeft, int marginTop, int totalCellSize)
        {
            // Add hour labels
            g.DrawString("0h", font, brush, marginLeft - 40, marginTop);
            g.DrawString("6h", font, brush, marginLeft - 40, marginTop + 6 * totalCellSize);
            g.DrawString("12h", font, brush, marginLeft - 40, marginTop + 12 * totalCellSize);
            g.DrawString("18h", font, brush, marginLeft - 40, marginTop + 18 * totalCellSize);
            g.DrawString("23h", font, brush, marginLeft - 40, marginTop + 23 * totalCellSize);
        }

        static void DrawDayLabels(Graphics g, Font font, Brush brush, int marginLeft, int marginTop, int daysInMonth, int totalCellSize)
        {
            // Add day labels
            for (int day = 1; day <= daysInMonth; day++)
            {
                if (day == 1 || day == 5 || day == 10 || day == 15 || day == 20 || day == 25 || day == daysInMonth)
                {
                    if (day < 10)
                        g.DrawString(day.ToString(), font, brush, marginLeft + (day - 1) * totalCellSize, marginTop);
                    else
                        g.DrawString(day.ToString(), font, brush, marginLeft + (day - 1) * totalCellSize - 7, marginTop);

                }
            }
        }

        static void DrawScale(Graphics g, Font font, Brush brush, int marginLeft, int marginTop, int width, int height, int cellSize, int totalCellSize, int maxCount, int cellPadding)
        {
            int scaleX = marginLeft + width + 20;
            int scaleYStart = marginTop;
            int scaleYEnd = marginTop + height;
            int scaleStep = (height - totalCellSize) / 23; // 24 values, 23 gaps

            g.FillRectangle(Brushes.Black, scaleX - cellPadding, marginTop - cellPadding, totalCellSize + cellPadding, 24 * totalCellSize + cellPadding);
            int i2 = 23;
            // Draw 24 values with scale
            for (int i = 0; i < 24; i++)
            {
                int y = scaleYStart + i2-- * scaleStep;
                g.FillRectangle(new SolidBrush(Scales.GetColorForValue(i, 24)), scaleX, y, cellSize, cellSize);
            }

            g.DrawString(maxCount.ToString(), font, brush, scaleX + cellSize, scaleYStart - 3);
            g.DrawString((maxCount / 2).ToString(), font, brush, scaleX + cellSize + 5, scaleYStart + (height / 2 - totalCellSize / 2) - 15);
            g.DrawString("0", font, brush, scaleX + cellSize + 5, scaleYEnd - totalCellSize);
        }

        static void DrawLineChart(Graphics g, List<AggregatedData> data, Brush brush, int marginLeft, int cellSize, int bitmapHeight)
        {
            Font font = new Font("Arial", 15, FontStyle.Bold);
            int daysInMonth = DateTime.DaysInMonth(data.First().EventDt.Year, data.First().EventDt.Month);
            int[] daySums = new int[daysInMonth];
            int allSum = 0;

            // Calculate sum for each day
            foreach (var item in data)
            {
                daySums[item.EventDt.Day - 1] += item.Count;
                allSum += item.Count;
            }

            int maxBarHeight = 200; // Max height for the bars
            int maxDaySum = daySums.Max();

            // Adjust height of the bitmap to accommodate the line chart
            int chartHeight = maxBarHeight + 60; // Extra space for labels

            // Draw the line chart on the main bitmap
            int chartMarginTop = bitmapHeight - chartHeight - 100; // Position the chart below the main grid
            int chartWidth = daysInMonth * (cellSize + 3); // Width of the chart area
            int spacing = 10;

            // Draw the chart background
            g.FillRectangle(Brushes.White, marginLeft, chartMarginTop, chartWidth, chartHeight);

            // Draw line chart
            Point[] points = new Point[daysInMonth];
            for (int i = 0; i < daysInMonth; i++)
            {
                int x = marginLeft + i * (cellSize + 3) + spacing;
                int y = chartMarginTop + chartHeight - (int)((double)daySums[i] / maxDaySum * maxBarHeight) - 30;
                points[i] = new Point(x, y);
            }

            // Draw the lines
            g.DrawLines(Pens.Blue, points);

            // Draw the points
            foreach (var point in points)
            {
                g.FillEllipse(Brushes.Blue, point.X - 3, point.Y - 3, 6, 6);
            }

            // Draw y-axis
            g.DrawLine(Pens.Black, marginLeft, chartMarginTop, marginLeft, chartMarginTop + chartHeight - 25); // Y-axis
            g.DrawLine(Pens.Black, marginLeft, chartMarginTop + chartHeight - 25, marginLeft + chartWidth, chartMarginTop + chartHeight - 25); // X-axis

            // Draw y-axis labels
            g.DrawString("0", font, brush, marginLeft - 30, chartMarginTop + chartHeight - 32);
            g.DrawString(maxDaySum.ToString(), font, brush, marginLeft - 50, chartMarginTop + 10);

            // Add title to the line chart
            g.DrawString($"Daily Totals - Sum: {allSum}", font, brush, chartWidth / 2, chartMarginTop - 20);
        }
    }
}
