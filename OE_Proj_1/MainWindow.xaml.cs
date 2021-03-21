using OE_Proj_1.Model;
using System;
using System.Windows;

namespace OE_Proj_1
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
        private Individual[] population;
        private Individual[] populationToCross;
        private static Random _random = new Random();
        public MainWindow()
        {
            InitializeComponent();
        }
        
        public void calculate(object sender, RoutedEventArgs e)
        {
            initialize();
            for (int i = 0; i < epochs; ++i)
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

            for(int i = 0; i < populationAmount; ++i)
            {
                population[i] = new Individual(Convert.ToInt32(numberOfBits));
            }
        }

        public void doEvaluate()
        {
            Array.ForEach(population, individual => individual.result = f(individual.chromosomeX, individual.chromosomeY));
        }

        public void doSelection()
        {
            switch (selection)
            {
                case "BEST":
                    doBestSelection();
                    break;
                case "ROULETTE":
                    doRouletteSelection();
                    break;
                case "TOURNAMENT":
                    doTournamentSelection();
                    break;
            }
        }

        public void doCrossover()
        {
            switch (crossover)
            {
                case "ONE_POINT":
                    doOnePointCrossover();
                    break;
                case "TWO_POINT":
                    doOnePointCrossover();
                    break;
                case "THREE_POINT":
                    doThreePointCrossover();
                    break;
                case "HOMO":
                    doHomoCrossover();
                    break;
            }

        }

        public void doMutatuion()
        {

        }

        public void doBestSelection()
        {
            Array.Sort(population);
            populationToCross = new Individual[Convert.ToInt32(Math.Floor(population.Length * (crossPercentage * 1.0 / 100)))];
            for(int i = 0; i < (Math.Floor(population.Length * (crossPercentage * 1.0 / 100))); ++i){
                populationToCross[i] = population[i];
            }
        }

        public void doRouletteSelection()
        {
            shufflePopulation();
            populationToCross = new Individual[Convert.ToInt32(Math.Ceiling(population.Length/crossPercentage))];
            int populationToCrossIncrement = 0;
            Individual min;
            for (int i = 0; i < population.Length; i += Convert.ToInt32(crossPercentage))
            {
                min = population[i];

                for(int j = i+1; j < population.Length && j < i+ Convert.ToInt32(crossPercentage); ++j)
                {
                    if(min.result > population[j].result)
                    {
                        min = population[j];
                    }
                }

                populationToCross[populationToCrossIncrement] = min;
                ++populationToCrossIncrement;
            }

        }

        public void doTournamentSelection()
        {
            //TODO
            doRouletteSelection();
        }


        public void doOnePointCrossover()
        {
            shufflePopulationToCross();
            int divider;
            for(int i = 0; i < populationToCross.Length && i+1 <= populationToCross.Length; i += 2)
            {
                divider = _random.Next(0, Convert.ToInt32(numberOfBits) + 1);

                bool temp;
                for(int j = 0; j<divider; ++j)
                {
                    temp = populationToCross[i].chromosomeX[j];
                    populationToCross[i].chromosomeX[j] = populationToCross[i + 1].chromosomeX[j];
                    populationToCross[i + 1].chromosomeX[j] = temp;
                }
            }
        }

        public void doTwoPointCrossover()
        {
            //TODO Michal
        }

        public void doThreePointCrossover()
        {
            //TODO Michal
        }

        public void doHomoCrossover()
        {
            //TODO Michal
        }

        //MIN f(x,y)=-1,9133 (x, y)=(-0.54719, -0.54719)
        public double f(bool[] boolX, bool[] boolY)
        {
            int x = BoolArrayToInt(boolX);
            int y = BoolArrayToInt(boolY);
            return Math.Sin(x + y) + (x - y) * (x - y) - 1.5 * x + 2.5 * y + 1;
        }

        static int BoolArrayToInt(bool[] arr)
        {
            if (arr.Length > 31)
            {
                throw new ApplicationException("Too many elements to be converted to a single int");
            }

            int val = 0;
            for (int i = 0; i < arr.Length; ++i)
                if (arr[i]) val |= 1 << i;
            return val;
        }

        private void shufflePopulation()
        {
            for (int i = 0; i < population.Length; ++i)
            {
                int randomIndex = _random.Next(0, i + 1);

                Individual temp = population[i];
                population[i] = population[randomIndex];
                population[randomIndex] = temp;
            }
        }

        private void shufflePopulationToCross()
        {
            for (int i = 0; i < populationToCross.Length; ++i)
            {
                int randomIndex = _random.Next(0, i + 1);

                Individual temp = populationToCross[i];
                populationToCross[i] = populationToCross[randomIndex];
                populationToCross[randomIndex] = temp;
            }
        }

    }
}
