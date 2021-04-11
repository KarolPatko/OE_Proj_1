using System;

namespace OE_Proj_1.ViewModel
{
    using Microsoft.Win32;
    using Model;
    using Newtonsoft.Json;
    using Syncfusion.UI.Xaml.Charts;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;
    using static OE_Proj_1.Model.AlgorithmConfig;

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private AlgorithmConfig config = AlgorithmConfig.Instance;

        public double a
        {
            get
            {
                return config.a;
            }
            set
            {
                config.a = value;
            }
        }
        public double b
        {
            get
            {
                return config.b;
            }
            set
            {
                config.b = value;
            }
        }
        public double numberOfBits
        {
            get
            {
                return config.numberOfBits;
            }
            set
            {
                config.numberOfBits = value;
            }
        }
        public double populationAmount
        {
            get
            {
                return config.populationAmount;
            }
            set
            {
                config.populationAmount = value;
            }
        }
        public double bestPercentageOrTournamentAmount
        {
            get
            {
                return config.bestPercentageOrTournamentAmount;
            }
            set
            {
                config.bestPercentageOrTournamentAmount = value;
            }
        }
        public double inversionPercentage
        {
            get
            {
                return config.inversionPercentage;
            }
            set
            {
                config.inversionPercentage = value;
            }
        }
        public double eliteAmount
        {
            get
            {
                return config.eliteAmount;
            }
            set
            {
                config.eliteAmount = value;
            }
        }
        public double epochs
        {
            get
            {
                return config.epochs;
            }
            set
            {
                config.epochs = value;
            }
        }
        public string selection
        {
            get
            {
                return config.selection;
            }
            set
            {
                config.selection = value.Replace("System.Windows.Controls.ComboBoxItem: ", "");
            }
        }
        public string mutation
        {
            get
            {
                return config.mutation;
            }
            set
            {
                config.mutation = value.Replace("System.Windows.Controls.ComboBoxItem: ", "");
            }
        }
        public string crossover
        {
            get
            {
                return config.crossover;
            }
            set
            {
                config.crossover = value.Replace("System.Windows.Controls.ComboBoxItem: ", "");
            }
        }
        public string error
        {
            get
            {
                return config.error;
            }
            set
            {
                config.error = value;
            }
        }
        public double crossPercentage
        {
            get
            {
                return config.crossPercentage;
            }
            set
            {
                config.crossPercentage = value;
            }
        }
        public double mutationPercentage
        {
            get
            {
                return config.mutationPercentage;
            }
            set
            {
                config.mutationPercentage = value;
                onPropertyChanged(nameof(bestValueToEpoch));
                onPropertyChanged(nameof(avgValueToEpoch));
                onPropertyChanged(nameof(sValueToEpoch));
            }
        }
        public string time { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        public void onPropertyChanged(string propertyName) {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ObservableCollection<BestValueToEpoch> bestValueToEpoch
        {
            get {
                return config.bestValueToEpoch;
            }
            set {
                config.bestValueToEpoch = value;
                onPropertyChanged(nameof(bestValueToEpoch));
            }
        }
        public ObservableCollection<BestValueToEpoch> avgValueToEpoch
        {
            get
            {
                return config.avgValueToEpoch;
            }
            set
            {
                config.avgValueToEpoch = value;
                onPropertyChanged(nameof(avgValueToEpoch));
            }
        }
        public ObservableCollection<BestValueToEpoch> sValueToEpoch
        {
            get
            {
                return config.sValueToEpoch;
            }
            set
            {
                config.sValueToEpoch = value;
                onPropertyChanged(nameof(sValueToEpoch));
            }
        }


        public void refresh(string ts)
        {
            onPropertyChanged(nameof(sValueToEpoch));
            onPropertyChanged(nameof(bestValueToEpoch));
            onPropertyChanged(nameof(avgValueToEpoch));
            time = "Time: " + ts;
            onPropertyChanged(nameof(time));
        }

        public void generateChart()
        {
            SfChart schart = App.Current.Windows[0].FindName("sValueToEpochChart") as SfChart;
            schart.Visibility = Visibility.Visible;
            schart.Save("charts/sChart.png");
            schart.Visibility = Visibility.Hidden;

            schart = App.Current.Windows[0].FindName("bestValueToEpochChart") as SfChart;
            schart.Visibility = Visibility.Visible;
            schart.Save("charts/bestChart.png");
            schart.Visibility = Visibility.Hidden;

            schart = App.Current.Windows[0].FindName("avgValueToEpochChart") as SfChart;
            schart.Visibility = Visibility.Visible;
            schart.Save("charts/avgChart.png");
            schart.Visibility = Visibility.Hidden;
        }

        public void refreshError(string newError)
        {
            error = newError;
            onPropertyChanged(nameof(error));
        }
    }
}
