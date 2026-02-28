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
        public static Color[] RmobColors = new Color[24]
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

        public static Color[] GrayscaleColors = new Color[24]
        {
            Color.FromArgb(0, 0, 0),      
            Color.FromArgb(11, 11, 11),
            Color.FromArgb(22, 22, 22),
            Color.FromArgb(34, 34, 34),
            Color.FromArgb(45, 45, 45),
            Color.FromArgb(56, 56, 56),
            Color.FromArgb(67, 67, 67),
            Color.FromArgb(79, 79, 79),
            Color.FromArgb(90, 90, 90),
            Color.FromArgb(101, 101, 101),
            Color.FromArgb(112, 112, 112),
            Color.FromArgb(124, 124, 124),
            Color.FromArgb(135, 135, 135),
            Color.FromArgb(146, 146, 146),
            Color.FromArgb(157, 157, 157),
            Color.FromArgb(169, 169, 169),
            Color.FromArgb(180, 180, 180),
            Color.FromArgb(191, 191, 191),
            Color.FromArgb(202, 202, 202),
            Color.FromArgb(214, 214, 214),
            Color.FromArgb(225, 225, 225),
            Color.FromArgb(236, 236, 236),
            Color.FromArgb(247, 247, 247),
            Color.FromArgb(255, 255, 255)  
        };

        public static Color[] ContrastyColors = new Color[]
        {
            Color.FromArgb(0, 0, 255),     
            Color.FromArgb(0, 128, 255),   
            Color.FromArgb(0, 255, 255),   
            Color.FromArgb(0, 255, 128),   
            Color.FromArgb(0, 255, 0),     
            Color.FromArgb(128, 255, 0),   
            Color.FromArgb(192, 255, 0),   
            Color.FromArgb(255, 255, 0),   
            Color.FromArgb(255, 192, 0),   
            Color.FromArgb(255, 128, 0),   
            Color.FromArgb(255, 64, 0),
            Color.FromArgb(255, 0, 0),     
            Color.FromArgb(255, 0, 128),   
            Color.FromArgb(255, 0, 192),   
            Color.FromArgb(255, 0, 255),   
            Color.FromArgb(192, 0, 255),   
            Color.FromArgb(128, 0, 255),   
            Color.FromArgb(64, 0, 255),    
            Color.FromArgb(0, 0, 160),     
            Color.FromArgb(0, 64, 128),    
            Color.FromArgb(0, 128, 64),    
            Color.FromArgb(128, 128, 0),   
            Color.FromArgb(128, 0, 0),     
            Color.FromArgb(64, 0, 0)
        };

        public static Color[] BlueToRedScale = new Color[]
        {
            Color.FromArgb(0, 0, 64),
            Color.FromArgb(0, 0, 96),
            Color.FromArgb(0, 0, 128),
            Color.FromArgb(0, 32, 160),
            Color.FromArgb(0, 64, 192),
            Color.FromArgb(0, 96, 224),
            Color.FromArgb(0, 128, 255),
            Color.FromArgb(0, 160, 255),
            Color.FromArgb(0, 192, 255),
            Color.FromArgb(0, 224, 255),
            Color.FromArgb(64, 255, 255),
            Color.FromArgb(128, 255, 224),
            Color.FromArgb(192, 255, 160),
            Color.FromArgb(255, 255, 96),
            Color.FromArgb(255, 224, 64),
            Color.FromArgb(255, 192, 32),
            Color.FromArgb(255, 160, 0),
            Color.FromArgb(255, 128, 0),
            Color.FromArgb(255, 96, 0),
            Color.FromArgb(255, 64, 0),
            Color.FromArgb(224, 32, 0),
            Color.FromArgb(192, 0, 0),
            Color.FromArgb(160, 0, 0),
            Color.FromArgb(128, 0, 0)
        };

        public static Color GetColorForValue(int value, int max)
        {
            decimal percent = value / (decimal)max;
            int position = (int)Math.Floor(percent * (RmobColors.Count() - 1));
            return RmobColors[position];
        }

        public static Color GetColorForValue(Color[] colors, int value, int max)
        {
            decimal percent = value / (decimal)(max - 1);
            int position = (int)Math.Floor(percent * (colors.Count() - 1));
            return colors[position];
        }
    }
}
