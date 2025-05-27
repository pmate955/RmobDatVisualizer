using Microsoft.Win32;
using RmobDatVisualizer.GUI.Windows;
using RmobDatVisualizer.Service;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RmobDatVisualizer.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this._selectedPaths = new List<string>();
        }

        /// <summary>
        /// RMOB.dat path list
        /// </summary>
        private List<string> _selectedPaths;

        private void OnOpenBtnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "RMOB .dat files (RMOB-*.dat)|RMOB-*.dat",
                Multiselect = true
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                this._selectedPaths = openFileDialog.FileNames.ToList();
                this.StatusLbl.Content = "Files selected";
            }
            else
            {
                this.StatusLbl.Content = "No file selected";
            }
        }

        private void GeneraterBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int countByRow = 6;
                Console.WriteLine("Read directory");

                var paths = this._selectedPaths
                        .OrderBy(fileName => fileName)
                        .ToList();

                List<Bitmap> graphList = new List<Bitmap>();
                
                int maxCount;
                List<List<AggregatedData>> csvData = CsvHelper.GetDataForImage(paths, out maxCount);

                for (int i = 0; i < paths.Count; i++)
                {
                    var gen = VisualizationHelper.GenerateImage(csvData[i], maxCount, i % countByRow == 0, (i % countByRow == countByRow - 1 && i > 0) || i == paths.Count - 1);
                    graphList.Add(gen);
                }

                var img = VisualizationHelper.MergeImages(graphList, countByRow);

                BitmapViewerWindow w = new(img, paths);
                w.Show();
            }
            catch (Exception ex)
            {
                this.StatusLbl.Content = "Exception!";
                MessageBox.Show(ex.Message);
            }
        }

       
    }
}
