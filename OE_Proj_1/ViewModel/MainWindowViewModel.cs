using System;

namespace OE_Proj_1.ViewModel
{

    using Model;

    public class MainWindowViewModel// : INotifyPropertyChanged
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

        /*        public event PropertyChangedEventHandler PropertyChanged;

                private void onPropertyChanged(string propertyName)
                {
                    if(PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                    }
                }*/


    }
}
