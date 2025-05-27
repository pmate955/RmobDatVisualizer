using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace RmobDatVisualizer.GUI.Windows
{
    /// <summary>
    /// Interaction logic for BitmapViewerWindow.xaml
    /// </summary>
    public partial class BitmapViewerWindow : Window
    {
        public BitmapViewerWindow(Bitmap input, List<string> pathList)
        {
            InitializeComponent();
            this.Title = string.Join(", ", pathList.Select(x => Path.GetFileName(x)));
            this.DisplayBitmap(input);
        }
        // Convert System.Drawing.Bitmap to BitmapSource
        private BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // Important if used across threads

                return bitmapImage;
            }
        }

        // Display the Bitmap in the WPF window
        private void DisplayBitmap(Bitmap bitmap)
        {
            this.Width = (bitmap.Width / 2) + 100;  // Add some padding
            this.Height = bitmap.Height / 2; // Add some padding
            BitmapSource bitmapSource = ConvertBitmapToBitmapSource(bitmap);
            ImageControl.Source = bitmapSource;
        }

        // Save the image to a file when the button is clicked
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the SaveFileDialog to let the user choose where to save the image
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Images (*.png)|*.png|JPEG Images (*.jpg)|*.jpg|BMP Images (*.bmp)|*.bmp",
                DefaultExt = "png"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                BitmapSource bitmapSource = ImageControl.Source as BitmapSource;
                if (bitmapSource != null)
                {
                    SaveBitmapToFile(bitmapSource, saveFileDialog.FileName);
                }
            }
        }

        // Save the BitmapSource to a file
        private void SaveBitmapToFile(BitmapSource bitmapSource, string filePath)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }
    }
}
