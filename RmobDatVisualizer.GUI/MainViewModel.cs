using RmobDatVisualizer.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RmobDatVisualizer.GUI
{

    public class MainViewModel : INotifyPropertyChanged
    {
        public enum VisualizationType
        {
            Rmob,
            MeteorShower
        }

        public enum RmobScale
        {
            Original,
            GrayScale,
            Contrasty,
            BlueToRed
        }

        private VisualizationType _selectedType = VisualizationType.Rmob;
        public VisualizationType SelectedType
        {
            get => _selectedType;
            set
            {
                if (_selectedType != value)
                {
                    _selectedType = value;
                    OnPropertyChanged(nameof(SelectedType));
                }
            }
        }

        private RmobScale _selectedRmobScale = RmobScale.Original;
        public RmobScale SelectedRmobScale
        {
            get => _selectedRmobScale;
            set
            {
                if (_selectedRmobScale != value)
                {
                    _selectedRmobScale = value;
                    OnPropertyChanged(nameof(SelectedRmobScale));
                }
            }
        }

        public Array AvailableScales => Enum.GetValues(typeof(RmobScale));

        private string _statusText = "";
        public string StatusText
        {
            get => _statusText;
            set
            {
                if (_statusText != value)
                {
                    _statusText = value;
                    OnPropertyChanged(nameof(StatusText));
                }
            }
        }

        private int _rmobMonthsPerRow = 6;
        public int RmobMonthsPerRow
        {
            get => _rmobMonthsPerRow; 
            set
            {
                if (_rmobMonthsPerRow != value)
                {
                    _rmobMonthsPerRow = value;
                    OnPropertyChanged(nameof(RmobMonthsPerRow));
                }
            }

        }

        private bool _rmobShowBarChart = true;
        public bool RmobShowBarChart
        {
            get => _rmobShowBarChart;
            set
            {
                if (_rmobShowBarChart != value)
                {
                    _rmobShowBarChart = value;
                    OnPropertyChanged(nameof(RmobShowBarChart));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public Color[] GetRmobSelectedColors()
        {
            switch (this.SelectedRmobScale)
            {
                case MainViewModel.RmobScale.Original:
                    return Scales.RmobColors;
                case MainViewModel.RmobScale.GrayScale:
                    return Scales.GrayscaleColors;
                case MainViewModel.RmobScale.Contrasty:
                    return Scales.ContrastyColors;
                case MainViewModel.RmobScale.BlueToRed:
                    return Scales.BlueToRedScale;
                default:
                    return Scales.RmobColors;
            }
        }
    }

}
