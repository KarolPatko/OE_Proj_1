using System;

namespace OE_Proj_1.ViewModel
{

    using Model;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Windows;
    using static OE_Proj_1.Model.AlgorithmConfig;

    public class Modell
    {
        public string Month { get; set; }

        public double Target { get; set; }

        public Modell(string xValue, double yValue)
        {
            Month = xValue;
            Target = yValue;
        }
    }
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

        public ObservableCollection<Modell> Dataa
        {
            get
            {
                return new ObservableCollection<Modell>()
        {
            new Modell("Jan", 50),
            new Modell("Feb", 70),
            new Modell("Mar", 65),
            new Modell("Apr", 57),
            new Modell("May", 48),
        }; 
            }
        }


       public event PropertyChangedEventHandler PropertyChanged;

        public void onPropertyChanged(string propertyName){
            if(PropertyChanged != null)
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
    }
}
