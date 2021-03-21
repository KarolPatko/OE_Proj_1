using System.ComponentModel;

namespace OE_Proj_1.ViewModel
{

    using Model;
    public class MainWindowViewModel// : INotifyPropertyChanged
    {
        private AlgorithmConfig config = new AlgorithmConfig();

        public double a
        {
            get {
                return config.a;
            }
            set
            {
                config.a = value;
                //onPropertyChanged(nameof(a));
            }
        }
        public double b { get; set; }
        public double numberOfBits { get; set; }
        public double populationAmount { get; set; }
        public double epochs { get; set; }
        public string selection { get; set; }

        /*        public event PropertyChangedEventHandler PropertyChanged;

                private void onPropertyChanged(string propertyName)
                {
                    if(PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                    }
                }*/

        public double f()
        {
            return 0.0;
        }
    }
}
