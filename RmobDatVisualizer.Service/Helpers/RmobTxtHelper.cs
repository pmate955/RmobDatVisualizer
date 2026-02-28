using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RmobDatVisualizer.Service
{
    public class RmobTxtHelper
    {
        /// <summary>
        /// Reads radio meteor detection data from an RmobTxt format file.
        /// </summary>
        /// <param name="path">File path to the RmobTxt file in format *MMYYYYrmob.txt</param>
        /// <param name="max">Output parameter containing the maximum count value found in the data.</param>
        /// <returns>List of AggregatedData containing parsed meteor detection counts by day and hour.</returns>
        /// <exception cref="Exception">Thrown when data table cannot be found in the file.</exception>
        public static List<AggregatedData> ReadRmobTxt(string path, out int max)
        {
            var data = new List<AggregatedData>();
            var maxCount = 0;
            var lines = File.ReadAllLines(path);
            
            // Find the data table start (skip header metadata)
            var tableStartIndex = FindTableStart(lines);
            if (tableStartIndex < 0)
                throw new Exception("Could not find data table in file");

            // Parse header row to get hour information
            var headerLine = lines[tableStartIndex];
            var hourColumns = ParseHourHeader(headerLine);

            // Extract month and year from filename (format: MMYYYYrmob.TXT)
            var dateInfo = ExtractDateFromFilename(path);
            int month = dateInfo.Month;
            int year = dateInfo.Year;

            // Parse data rows (days)
            for (int i = tableStartIndex + 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("["))
                    break;

                var parts = line.Split(new[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                    continue;

                // First part is the day
                if (!int.TryParse(parts[0].Trim(), out int day) || day < 1 || day > 31)
                    continue;

                // Parse hour data
                for (int j = 1; j < parts.Length && j <= hourColumns.Count; j++)
                {
                    var cellValue = parts[j].Trim();
                    
                    // Skip missing data marked as "???"
                    if (cellValue == "???" || string.IsNullOrEmpty(cellValue))
                        continue;

                    if (int.TryParse(cellValue, out int count))
                    {
                        var dateTime = new DateTime(year, month, day, hourColumns[j - 1], 0, 0);
                        data.Add(new AggregatedData(dateTime, hourColumns[j - 1], count));

                        if (count > maxCount)
                            maxCount = count;
                    }
                }
            }

            max = maxCount;
            return data;
        }

        /// <summary>
        /// Loads and processes multiple RmobTxt format files for visualization.
        /// </summary>
        /// <param name="paths">List of file paths to RmobTxt files to load.</param>
        /// <param name="maxCount">Output parameter containing the maximum count value across all files.</param>
        /// <returns>List of aggregated data lists, one per input file.</returns>
        /// <exception cref="Exception">Thrown when a file is invalid, contains no data, or when data cannot be parsed.</exception>
        public static List<List<AggregatedData>> GetDataForImage(List<string> paths, out int maxCount)
        {
            var rdmData = new List<List<AggregatedData>>();
            maxCount = 0;

            for (int i = 0; i < paths.Count; i++)
            {
                var data = ReadRmobTxt(paths[i], out int localMax);

                if (data == null || data.Count == 0)
                {
                    throw new Exception("Invalid input or no data");
                }

                if (localMax > maxCount)
                    maxCount = localMax;

                rdmData.Add(data);
            }

            return rdmData;
        }

        private static int FindTableStart(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                // Look for the header row containing hour information (00h, 01h, etc.)
                if (lines[i].Contains("00h") && lines[i].Contains("01h"))
                    return i;
            }
            return -1;
        }

        private static List<int> ParseHourHeader(string headerLine)
        {
            var hours = new List<int>();
            var parts = headerLine.Split(new[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < parts.Length; i++)
            {
                var part = parts[i].Trim();
                if (part.EndsWith("h") && int.TryParse(part.Substring(0, part.Length - 1), out int hour))
                {
                    hours.Add(hour);
                }
            }

            return hours;
        }

        private static (int Month, int Year) ExtractDateFromFilename(string path)
        {
            // Extract filename without extension (format: *_MMYYYYrmob or MMYYYYrmob)
            var filename = Path.GetFileNameWithoutExtension(path);
            
            // Find the "rmob" suffix to locate the date portion
            var rmobIndex = filename.IndexOf("rmob", StringComparison.OrdinalIgnoreCase);
            if (rmobIndex < 6)
                rmobIndex = -1;
            
            string dateStr;
            if (rmobIndex > 0)
            {
                // Extract the 6 characters before "rmob" (MMYYYY)
                dateStr = filename.Substring(rmobIndex - 6, 6);
            }
            else
            {
                // Fallback: try to find 6 consecutive digits
                dateStr = filename.Length >= 6 ? filename.Substring(0, 6) : "";
            }
            
            // Expected format: MMYYYY (e.g., 022024 for February 2024 or 022026 for February 2026)
            if (dateStr.Length >= 6)
            {
                var monthStr = dateStr.Substring(0, 2);
                var yearStr = dateStr.Substring(2, 4);
                
                if (int.TryParse(monthStr, out int month) && int.TryParse(yearStr, out int year))
                {
                    if (month >= 1 && month <= 12)
                        return (month, year);
                }
            }
            
            // Default fallback
            return (1, DateTime.Now.Year);
        }
    }
}

