using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RmobDatVisualizer.Service
{
    public class VisualizationHelper
    {
        #region Public methods

        public static Bitmap GenerateRmobImage(List<AggregatedData> data, int maxCount, Color[] colors, bool hasLegend = true, bool hasScale = true, bool hasBarChart = true)
        {
            DateTime firstDate = data.First().EventDt;
            int daysInMonth = DateTime.DaysInMonth(firstDate.Year, firstDate.Month);
            int cellSize = 20;
            int cellPadding = 3;
            int totalCellSize = cellSize + cellPadding;

            int width = daysInMonth * totalCellSize;
            int height = 24 * totalCellSize;

            int marginLeft = hasLegend ? 60 : 5;  // Left margin for labels and bar chart
            int marginTop = 100;
            int marginRight = hasScale ? 100 : 5; // Right margin for scale

            Bitmap bitmap = new Bitmap(width + marginLeft + marginRight, height + marginTop + (hasBarChart ? 400 : 50));

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
                    Color color = Scales.GetColorForValue(colors, item.Count, maxCount);
                    g.FillRectangle(new SolidBrush(color), x, y, cellSize, cellSize);
                }

                using (Font font = new Font("Arial", 15, FontStyle.Bold))
                {
                    using (Brush brush = new SolidBrush(Color.Black))
                    {
                        string title = data[0].EventDt.ToString("yyyy MMMM", CultureInfo.InvariantCulture);

                        g.DrawString(title, font, brush, width / 2.0f, 20);

                        if (hasLegend)
                            DrawHourLabels(g, font, brush, marginLeft, marginTop - 4, totalCellSize);

                        DrawDayLabels(g, font, brush, marginLeft, marginTop - 32, daysInMonth, totalCellSize);

                        if (hasScale)
                            DrawScale(g, font, brush, marginLeft, marginTop, width, height, cellSize, totalCellSize, maxCount, cellPadding, colors);

                        if (hasBarChart)
                            DrawBarChart(g, data, brush, marginLeft, cellSize, height + marginTop + 450, maxCount, hasLegend);

                    }
                }
            }

            return bitmap;
        }

        public static Bitmap MergeImages(List<Bitmap> images, int countByRow)
        {
            if (images.Count == 0)
                throw new ArgumentException("Not enough input!");

            int fixedHeight = images[0].Height;
            int totalWidth = 0;
            int currentRowWidth = 0;
            int rows = (int)Math.Ceiling(images.Count / (decimal)countByRow);

            List<int> rowWidths = new List<int>();

            for (int i = 0; i < images.Count; i++)
            {
                currentRowWidth += images[i].Width;
                if ((i + 1) % countByRow == 0 || i == images.Count - 1)
                {
                    rowWidths.Add(currentRowWidth);
                    totalWidth = Math.Max(totalWidth, currentRowWidth);
                    currentRowWidth = 0;
                }
            }

            Bitmap mergedBitmap = new Bitmap(totalWidth, fixedHeight * rows);
            using (Graphics g = Graphics.FromImage(mergedBitmap))
            {
                g.Clear(Color.White);
                int x = 0, y = 0, imgIndex = 0;

                foreach (int rowWidth in rowWidths)
                {
                    x = 0;
                    for (int i = 0; i < countByRow && imgIndex < images.Count; i++, imgIndex++)
                    {
                        g.DrawImage(images[imgIndex], x, y);
                        x += images[imgIndex].Width;
                    }
                    y += fixedHeight;
                }
            }

            return mergedBitmap;
        }

        public static Bitmap GenerateHistogram(List<AggregatedData> data, DateTime start, DateTime end, int imageWidth = 1200, int imageHeight = 600, bool showGrid = true)
        {
            var hours = new List<DateTime>();
            DateTime current = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0);
            while (current <= end)
            {
                hours.Add(current);
                current = current.AddHours(1);
            }

            var countsPerHour = hours.Select(h =>
            {
                var item = data.FirstOrDefault(d => d.EventDt.Date == h.Date && d.Hour == h.Hour);
                return item.EventDt == DateTime.MinValue ? 0 : item.Count;
            }).ToList();

            int marginLeft = 70;
            int marginBottom = 130;
            int marginTop = 20;
            int marginRight = 70;

            int plotWidth = imageWidth - marginLeft - marginRight;
            int plotHeight = imageHeight - marginTop - marginBottom;

            Bitmap bmp = new Bitmap(imageWidth, imageHeight);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.White);

                int maxCount = countsPerHour.Max();
                if (maxCount == 0) maxCount = 1;

                float barSpacing = 1;
                float barWidth = (float)plotWidth / countsPerHour.Count - barSpacing;

                // Draw bars
                for (int i = 0; i < countsPerHour.Count; i++)
                {
                    float x = marginLeft + i * (barWidth + barSpacing);
                    float barHeight = (float)countsPerHour[i] / maxCount * plotHeight;
                    float y = marginTop + (plotHeight - barHeight);

                    RectangleF barRect = new RectangleF(x, y, barWidth, barHeight);
                    g.FillRectangle(Brushes.SteelBlue, barRect);
                }

                using (Pen axisPen = new Pen(Color.Black, 1))
                {
                    // X axis
                    g.DrawLine(axisPen, marginLeft, marginTop + plotHeight, imageWidth - marginRight, marginTop + plotHeight);
                    // Y axis
                    g.DrawLine(axisPen, marginLeft - 5, marginTop, marginLeft - 5, marginTop + plotHeight);
                }

                using (Font font = new Font("Arial", 16))
                using (Brush textBrush = new SolidBrush(Color.Black))
                {
                    // Y-axis labels
                    int ySteps = 5;
                    for (int i = 0; i <= ySteps; i++)
                    {
                        int val = maxCount * i / ySteps;
                        float y = marginTop + plotHeight - (float)val / maxCount * plotHeight;

                        if (showGrid)
                            g.DrawLine(Pens.Gray, marginLeft - 5, y, imageWidth - marginRight, y);
                        else
                            g.DrawLine(Pens.Gray, marginLeft - 5, y, marginLeft, y);

                        g.DrawString(val.ToString(), font, textBrush, 0, y - 12);
                    }

                    // X-axis labels (sparse for readability)
                    for (int i = 0; i < hours.Count; i++)
                    {
                        if (i % Math.Max(1, hours.Count / 20) == 0)
                        {
                            float x = marginLeft + i * (barWidth + barSpacing) + barWidth / 2;

                            // Draw vertical grid line
                            if (showGrid)
                                g.DrawLine(Pens.Gray, x, marginTop, x, marginTop + plotHeight + 6);

                            // Draw angled label
                            string label = hours[i].ToString("MM-dd HH");
                            g.TranslateTransform(x, marginTop + plotHeight + 5);
                            g.RotateTransform(45);
                            g.DrawString(label, font, textBrush, 0, 0);
                            g.ResetTransform();
                        }
                    }
                }
            }

            return bmp;
        }

        #endregion

        static void DrawHourLabels(Graphics g, Font font, Brush brush, int marginLeft, int marginTop, int totalCellSize)
        {
            // Add hour labels
            g.DrawString("0h", font, brush, marginLeft - 45, marginTop);
            g.DrawString("6h", font, brush, marginLeft - 45, marginTop + 6 * totalCellSize);
            g.DrawString("12h", font, brush, marginLeft - 55, marginTop + 12 * totalCellSize);
            g.DrawString("18h", font, brush, marginLeft - 55, marginTop + 18 * totalCellSize);
            g.DrawString("23h", font, brush, marginLeft - 55, marginTop + 23 * totalCellSize);
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

        static void DrawScale(Graphics g, Font font, Brush brush, int marginLeft, int marginTop, int width, int height, int cellSize, int totalCellSize, int maxCount, int cellPadding, Color[] colors)
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
                g.FillRectangle(new SolidBrush(Scales.GetColorForValue(colors, i, 24)), scaleX, y, cellSize, cellSize);
            }

            g.DrawString(maxCount.ToString(), font, brush, scaleX + cellSize + 5, scaleYStart - 3);
            g.DrawString((maxCount / 2).ToString(), font, brush, scaleX + cellSize + 5, scaleYStart + (height / 2 - totalCellSize / 2) - 15);
            g.DrawString("0", font, brush, scaleX + cellSize + 5, scaleYEnd - totalCellSize);
        }

        static void DrawBarChart(Graphics g, List<AggregatedData> data, Brush brush, int marginLeft, int cellSize, int bitmapHeight, int maxDaySum, bool hasLegend)
        {
            Font font = new Font("Arial", 15, FontStyle.Bold);
            int daysInMonth = DateTime.DaysInMonth(data.First().EventDt.Year, data.First().EventDt.Month);
            int[] dayMax = new int[daysInMonth];
            int allSum = 0;

            // Calculate sum for each day
            foreach (var item in data)
            {
                if (item.Count > dayMax[item.EventDt.Day - 1])
                    dayMax[item.EventDt.Day - 1] = item.Count;

                allSum += item.Count;
            }

            int maxBarHeight = 200; // Max height for the bars

            // Adjust height of the bitmap to accommodate the line chart
            int chartHeight = maxBarHeight + 60; // Extra space for labels

            // Draw the line chart on the main bitmap
            int spacing = 10;
            int chartMarginTop = bitmapHeight - chartHeight - 100; // Position the chart below the main grid
            int chartWidth = daysInMonth * (cellSize + 3) + spacing; // Width of the chart area

            // Draw the chart background
            g.FillRectangle(Brushes.White, marginLeft, chartMarginTop, chartWidth, chartHeight);

            // Draw line chart
            Point[] points = new Point[daysInMonth];
            for (int i = 0; i < daysInMonth; i++)
            {
                int x = marginLeft + i * (cellSize + 3) + spacing;
                int y = chartMarginTop + chartHeight - (int)((double)dayMax[i] / maxDaySum * maxBarHeight) - 30;
                points[i] = new Point(x, y);

                int barHeight = (int)((double)dayMax[i] / maxDaySum * maxBarHeight);
                y = chartMarginTop + chartHeight - barHeight - 30;
                g.FillRectangle(Brushes.Blue, x - (cellSize / 2), y, cellSize - 3, barHeight);
            }

            // Draw y-axis
            g.DrawLine(Pens.Black, marginLeft, chartMarginTop + chartHeight - 25, marginLeft + chartWidth, chartMarginTop + chartHeight - 25); // X-axis

            if (hasLegend)
            {
                g.DrawLine(Pens.Black, marginLeft, chartMarginTop, marginLeft, chartMarginTop + chartHeight - 25); // Y-axis
                g.DrawString("0", font, brush, marginLeft - 30, chartMarginTop + chartHeight - 32);
                g.DrawString(maxDaySum.ToString(), font, brush, marginLeft - 50, chartMarginTop + 10);
            }
            // Add title to the line chart
            g.DrawString($"Daily Maximums - Sum: {allSum}", font, brush, (chartWidth / 2) - 50, chartMarginTop - 20);
        }
    }
}
