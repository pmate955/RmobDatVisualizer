using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RmobDatVisualizer.Service
{
    public class Scales
    {
        public static Color[] RmobColors = new Color[]
        {
            Color.FromArgb(0, 67, 255),
            Color.FromArgb(0, 80, 255),
            Color.FromArgb(0, 95, 255),
            Color.FromArgb(0, 113, 255),
            Color.FromArgb(0, 135, 255),
            Color.FromArgb(0, 160, 255),
            Color.FromArgb(0, 191, 255),
            Color.FromArgb(0, 227, 255),
            Color.FromArgb(71, 255, 241),
            Color.FromArgb(85, 255, 202),
            Color.FromArgb(101, 255, 170),
            Color.FromArgb(120, 255, 143),
            Color.FromArgb(143, 255, 120),
            Color.FromArgb(170, 255, 101),
            Color.FromArgb(202, 255, 85),
            Color.FromArgb(241, 255, 71),
            Color.FromArgb(255, 227, 0),
            Color.FromArgb(255, 191, 0),
            Color.FromArgb(255, 160, 0),
            Color.FromArgb(255, 135, 0),
            Color.FromArgb(255, 113, 0),
            Color.FromArgb(255, 95, 0),
            Color.FromArgb(255, 80, 0),
            Color.FromArgb(255, 0, 0)
        };

        public static Color GetColorForValue(int value, int max)
        {
            decimal percent = value / (decimal)max;
            int position = (int)Math.Floor(percent * (RmobColors.Count() - 1));
            return RmobColors[position];
        }
    }
}
