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
using Color = System.Drawing.Color;

namespace RmobDatVisualizer.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            this._selectedPaths = new List<string>();
            DataContext = ViewModel;
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
                this.ViewModel.StatusText = "Files selected";
            }
            else
            {
                this.ViewModel.StatusText = "No file selected";
            }
        }

        private void GeneraterBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int countByRow = this.ViewModel.RmobMonthsPerRow;
                Color[] colors = this.ViewModel.GetRmobSelectedColors();

                var paths = this._selectedPaths
                        .OrderBy(fileName => fileName)
                        .ToList();

                if (paths.Count == 0)
                    throw new Exception("No input file selected. Please use the \"Open\" button!");

                List<Bitmap> graphList = new List<Bitmap>();

                int maxCount;
                List<List<AggregatedData>> csvData = CsvHelper.GetDataForImage(paths, out maxCount);

                if (ViewModel.SelectedType == MainViewModel.VisualizationType.Rmob)
                {

                    for (int i = 0; i < paths.Count; i++)
                    {
                        var gen = VisualizationHelper.GenerateImage(csvData[i], maxCount, colors, i % countByRow == 0, (i % countByRow == countByRow - 1 && i > 0) || i == paths.Count - 1);
                        graphList.Add(gen);
                    }

                    var img = VisualizationHelper.MergeImages(graphList, countByRow);
                    BitmapViewerWindow w = new(img, paths);
                    w.Show();
                }
                else
                {
                    MessageBox.Show(this, "Not supported yet!");
                }
            }
            catch (Exception ex)
            {
                this.ViewModel.StatusText = "Exception!";
                MessageBox.Show(this, ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }       
    }
}
