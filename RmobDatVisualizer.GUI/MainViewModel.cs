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

        private RmobScale _selectedRmobScale = RmobScale.BlueToRed;
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

        public static Array AvailableScales => Enum.GetValues(typeof(RmobScale));

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

        private DateTime _meteorShowerStartDt = DateTime.Now.Date;
        public DateTime MeteorShowerStartDt
        {
            get => _meteorShowerStartDt;
            set
            {
                if (_meteorShowerStartDt != value)
                {
                    _meteorShowerStartDt = value;
                    this.MeteorShowerEndDt = _meteorShowerStartDt.AddDays(7);
                    OnPropertyChanged(nameof(MeteorShowerStartDt));
                }
            }
        }

        private DateTime _meteorShowerEndDt = DateTime.Now.Date;
        public DateTime MeteorShowerEndDt
        {
            get => _meteorShowerEndDt;
            set
            {
                if (_meteorShowerEndDt != value)
                {
                    _meteorShowerEndDt = value;
                    OnPropertyChanged(nameof(MeteorShowerEndDt));
                }
            }
        }

        private bool _meteorShowerShowGrid = true;
        public bool MeteorShowerShowGrid
        {
            get => _meteorShowerShowGrid;
            set
            {
                if (_meteorShowerShowGrid != value)
                {
                    _meteorShowerShowGrid = value;
                    OnPropertyChanged(nameof(MeteorShowerShowGrid));
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
                case RmobScale.Original:
                    return Scales.RmobColors;
                case RmobScale.GrayScale:
                    return Scales.GrayscaleColors;
                case RmobScale.Contrasty:
                    return Scales.ContrastyColors;
                case RmobScale.BlueToRed:
                    return Scales.BlueToRedScale;
                default:
                    return Scales.RmobColors;
            }
        }
    }

}
