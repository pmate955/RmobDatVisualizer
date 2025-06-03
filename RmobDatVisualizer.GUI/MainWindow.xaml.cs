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
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel.StatusText = "Please open RMOB files!";
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
                this.ViewModel.StatusText = "Files selected.";
            }
            
            if (this._selectedPaths.Count() == 0)
            {
                this.ViewModel.StatusText = "No file selected!";
            }
        }

        private void GeneraterBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var paths = this._selectedPaths
                        .OrderBy(fileName => fileName)
                        .ToList();

                if (paths.Count == 0)
                    throw new Exception("No input file selected. Please use the \"Open\" button!");

                List<Bitmap> graphList = new();

                int maxCount;
                List<List<AggregatedData>> csvData = CsvHelper.GetDataForImage(paths, out maxCount);

                if (ViewModel.SelectedType == MainViewModel.VisualizationType.Rmob)
                {
                    int countByRow = this.ViewModel.RmobMonthsPerRow;
                    Color[] colors = this.ViewModel.GetRmobSelectedColors();
                    bool hasBarChart = this.ViewModel.RmobShowBarChart;

                    for (int i = 0; i < paths.Count; i++)
                    {
                        var gen = VisualizationHelper.GenerateRmobImage(csvData[i], maxCount, colors, i % countByRow == 0, (i % countByRow == countByRow - 1 && i > 0) || i == paths.Count - 1, hasBarChart);
                        graphList.Add(gen);
                    }

                    var img = VisualizationHelper.MergeImages(graphList, countByRow);
                    BitmapViewerWindow w = new(img, paths)
                    {
                        Owner = this
                    };
                    w.Show();
                }
                else
                {
                    var startDt = this.ViewModel.MeteorShowerStartDt;
                    var endDt = this.ViewModel.MeteorShowerEndDt.AddDays(1);
                    var showGrid = this.ViewModel.MeteorShowerShowGrid;

                    if (startDt >= endDt)
                        throw new Exception("StartDt has to be earlier than endDt!");

                    var filtered = csvData
                        .SelectMany(list => list) 
                        .Where(item => item.EventDt >= startDt && item.EventDt <= endDt)
                        .OrderBy(item => item.EventDt)
                        .ToList();

                    if (filtered.Count == 0)
                        throw new Exception("No data in the given period. Please select an another RMOB.dat file, or modify the period!");

                    var img = VisualizationHelper.GenerateHistogram(filtered, startDt, endDt, 1300, 900, showGrid);

                    BitmapViewerWindow w = new(img, paths)
                    {
                        Owner = this
                    };
                    w.Show();
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
