using System.ComponentModel;
using System;

namespace OE_Proj_1.ViewModel
{

    using Model;
    public class MainWindowViewModel// : INotifyPropertyChanged
    {
        private AlgorithmConfig config = new AlgorithmConfig();

        /*        public double a
                {
                    get {
                        return config.a;
                    }
                    set
                    {
                        config.a = value;
                        //komentarz
                        //onPropertyChanged(nameof(a));
                    }
                }*/
        public double a { get; set; }
        public double b { get; set; }
        public double numberOfBits { get; set; }
        public double populationAmount { get; set; }
        public double epochs { get; set; }
        public string selection { get; set; }

        private Individual[] population;

        /*        public event PropertyChangedEventHandler PropertyChanged;

                private void onPropertyChanged(string propertyName)
                {
                    if(PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                    }
                }*/

        
        public void calculate()
        {
            initialize();
            for(int i = 0; i < epochs; ++i)
            {
                doEvaluate();
                doSelection();
                doCrossover();
                doMutatuion();
            }
        }

        public void initialize()
        {
            population = new Individual[Convert.ToInt32(populationAmount)];
        }

        public void doEvaluate()
        {

        }

        public void doSelection()
        {
            switch (selection)
            {
                case "BEST":
                    return;
                case "ROULETTE":
                    return;
                case "TOURNAMENT":
                    return;
            }
        }

        public void doCrossover()
        {

        }

        public void doMutatuion()
        {

        }

        public double f(double x, double y)
        {
            //MIN f(x,y)=-1,9133 (x, y)=(-0.54719, -0.54719)
            return Math.Sin(x + y) + (x - y) * (x - y) - 1.5 * x + 2.5 * y + 1;
        }
    }
}
