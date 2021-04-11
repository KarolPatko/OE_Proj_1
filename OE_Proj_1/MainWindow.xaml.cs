using OE_Proj_1.Model;
using OE_Proj_1.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static OE_Proj_1.Model.AlgorithmConfig;


namespace OE_Proj_1
{

    public partial class MainWindow : Window
    {

        private AlgorithmConfig config = AlgorithmConfig.Instance;

        private MainWindowViewModel ViewModel;
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel();
            this.DataContext = ViewModel;
        }
        
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
            }
        }
        private Individual[] population;
        private Individual[] populationToCross;
        private Individual[] bestIndividuals;
        private static Random _random = new Random();
        public ObservableCollection<BestValueToEpoch> bestValueToEpoch
        {
            get { return config.bestValueToEpoch; }
            set { config.bestValueToEpoch = value; }
        }

        private double[] bb;
        private double[] sr;
        private double[] s;
        private static int iterator = 0;

        public void calculate(object sender, RoutedEventArgs e)
        {

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            setError();
            ViewModel.refreshError(config.error);
            if (config.error != "") return;

            initialize();

            for (int i = 0; i < epochs; ++i)
            {
                doEvaluate();
                saveIteration(i);
                getBest();
                doSelection();
                doCrossover();
                doMutatuion();
                doInversion();
                rewriteBest();
            }
            fillCharts();

            stopwatch.Start();

            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            ViewModel.refresh(elapsedTime);
        }

        public void generateChart(object sender, RoutedEventArgs e)
        {
            ViewModel.generateChart();
        }

        public void setError()
        {
            config.error = "";
            if (a > b)
            {
                double temp = b;
                b = a;
                a = temp;
            }

            if(numberOfBits <= 0)
            {
                config.error += "Liczba bitów musi być większa od 0\n";
            }

            if (numberOfBits >= 31)
            {
                config.error += "Liczba bitów musi być mniejsza lub równa 31\n";
            }

            if (populationAmount < 2)
            {
                config.error += "Populacja musi być większa niż 2\n";
            }

            if (epochs <= 0)
            {
                config.error += "Liczba epok musi być większa od zera\n";
            }
        }

        public void initialize()
        {
            //initialize population
            population = new Individual[Convert.ToInt32(populationAmount)];

            int chromosomeLength = Convert.ToInt32(numberOfBits);
            for (int i = 0; i < populationAmount; ++i)
            {
                population[i] = new Individual(chromosomeLength);
            }

            //initialize file with iterations
            string[] empty = new string[1];
            empty[0] = "";
            File.WriteAllLines("iterations.txt", empty);

            //initialize arrays to save values
            bb = new double[Convert.ToInt32(epochs)];
            sr = new double[Convert.ToInt32(epochs)];
            s = new double[Convert.ToInt32(epochs)];

            iterator = 0;

            bestIndividuals = new Individual[Convert.ToInt32(eliteAmount)];
        }

        public void doEvaluate()
        {
            //calculate sum and evaluate idividual value
            double sum = 0;
            for (int i = 0; i < population.Length; ++i)
            {
                population[i].result = f(population[i].chromosomeX, population[i].chromosomeY);
                sum += population[i].result;
            }
            sr[iterator] = sum / population.Length;

            //calculate standard deviation
            sum = 0;
            for (int i = 0; i < population.Length; ++i)
            {
                sum += Math.Pow(population[i].result - sr[iterator], 2);
            }
            s[iterator] = Math.Sqrt(sum / population.Length);
        }

        public void saveIteration(int i)
        {
            //file format
            //epoch
            //chromosomeX chromosomeY value | for each individual in population
            string[] iterationInfo = {"" + i};
            File.AppendAllLines("iterations.txt", iterationInfo);

            string[] individualInfo = new string[3];
            for(int j = 0; j<population.Length; ++j)
            {
                individualInfo[1] = "";
                individualInfo[2] = "";
                for (int k = 0; k< population[j].chromosomeX.Length; ++k)
                {
                    individualInfo[1] += population[j].chromosomeX[k] ? "1" : "0";
                    individualInfo[2] += population[j].chromosomeY[k] ? "1" : "0";
                }
                individualInfo[0] = " " + population[j].result;
                string[] info = { individualInfo[1] + " " + individualInfo[2] + individualInfo[0] };
                File.AppendAllLines("iterations.txt", info);
            }
        }

        public void getBest()
        {
            Array.Sort(population);
            for(int i = 0; i<eliteAmount; ++i)
            {
                bestIndividuals[i] = population[i].Clone();
            }

            double min = population[0].result;

            for(int i = 0; i<population.Length; ++i)
            {
                if(population[i].result < min)
                {
                    min = population[i].result;
                }
            }
            bb[iterator] = min;
            ++iterator;
        }

        public void doSelection()
        {
            switch (selection)
            {
                case "BEST":
                    doBestSelection();
                    break;
                case "TOURNAMENT":
                    doTournamentSelection();
                    break;
                case "ROULETTE":
                    doRouletteSelection();
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
                    doTwoPointCrossover();
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
            switch (mutation)
            {
                case "ONE_POINT":
                    doOnePointMutation();
                    break;
                case "TWO_POINT":
                    doOnePointMutation();
                    break;
                case "EDGE":
                    doEdgeMutation();
                    break;
            }
        }

        public void doInversion()
        {
            int pivot1;
            int pivot2;
            for(int i = Convert.ToInt32(eliteAmount); i<population.Length; ++i)
            {
                double random = _random.NextDouble() * 100;
                if (random < inversionPercentage)
                {
                    pivot1 = _random.Next(0, Convert.ToInt32(numberOfBits) - 1);
                    pivot2 = _random.Next(pivot1, Convert.ToInt32(numberOfBits));

                    for (int j = pivot1; j < pivot2; ++j)
                    {
                        population[i].chromosomeX[j] = !population[i].chromosomeX[j];
                        population[i].chromosomeY[j] = !population[i].chromosomeY[j];
                    }
                }
            }
        }

        public void doBestSelection()
        {
            Array.Sort(population);
            int populationToCrossLength = Convert.ToInt32(Math.Floor(population.Length * (bestPercentageOrTournamentAmount * 1.0 / 100)));
            populationToCross = new Individual[populationToCrossLength];
            for(int i = 0; i < populationToCrossLength ; ++i){
                populationToCross[i] = population[i].Clone();
            }
        }

        public void doRouletteSelection()
        {



            double min = population[0].result;
            double max = population[0].result;
            for (int i = 0; i< population.Length; ++i)
            {
                if(population[i].result < min)
                {
                    min = population[i].result;
                }
                if (population[i].result > max)
                {
                    max = population[i].result;
                }
            }

            if(min == 0)
            {
                for (int i = 0; i < population.Length; ++i)
                {
                    population[i].result += 0.0001;
                }
            }
            if(min < 0)
            {
                for (int i = 0; i < population.Length; ++i)
                {
                    population[i].result += min + ((-1 * min)/(max - min)) + 0.0001;
                }
            }

            double sum = 0;
            for (int i = 0; i < population.Length; ++i)
            {
                population[i].result /= 1;
                sum += population[i].result;
            }

            population[0].distributor = sum/population[0].result;
            for(int i = 1; i< population.Length; ++i)
            {
                population[i].distributor = population[i-1].distributor + sum / population[i].result;
            }
            population[population.Length - 1].distributor = 1;

            int populationToCrossLength = Convert.ToInt32(Math.Floor(population.Length * (bestPercentageOrTournamentAmount * 1.0 / 100)));
            populationToCross = new Individual[populationToCrossLength];

            double random = _random.NextDouble();
            bool breaked;
            for (int i = 0; i < populationToCrossLength; ++i)
            {
                breaked = false;
                for (int j = population.Length - 1; j > 0; --j)
                {
                    if(random > population[j].distributor)
                    {
                        populationToCross[i] = population[j + 1].Clone();
                        breaked = true;
                        break;
                    }
                }
                if (!breaked)
                {
                    populationToCross[i] = population[0].Clone();
                }
            }

        }

        public void doTournamentSelection()
        {
            shufflePopulation();
            
            int populationToCrossLength = Convert.ToInt32(Math.Floor(population.Length / (bestPercentageOrTournamentAmount * 1.0)));
            populationToCross = new Individual[populationToCrossLength];

            int populationToCrossIncrement = 0;
            Individual min;
            for (int i = 0; i < population.Length && populationToCrossIncrement < populationToCrossLength; i += Convert.ToInt32(bestPercentageOrTournamentAmount))
            {
                min = population[i].Clone();

                for(int j = i+1; j < population.Length && j < i+ Convert.ToInt32(bestPercentageOrTournamentAmount) ; ++j)
                {
                    if(min.result > population[j].result)
                    {
                        min = population[j].Clone();
                    }
                }

                populationToCross[populationToCrossIncrement] = min;
                ++populationToCrossIncrement;
            }

        }

        public void doOnePointCrossover()
        {
            int divider;

            for (int i = Convert.ToInt32(eliteAmount); i< population.Length;)
            {
                Individual firstIndividualToCross = populationToCross[_random.Next(0, Convert.ToInt32(populationToCross.Length))].Clone();
                Individual secondIndividualToCross = populationToCross[_random.Next(0, Convert.ToInt32(populationToCross.Length))].Clone();

                divider = _random.Next(0, Convert.ToInt32(numberOfBits));

                for(int j = divider; j < firstIndividualToCross.chromosomeX.Length; ++j)
                {
                    bool temp = firstIndividualToCross.chromosomeX[j];
                    firstIndividualToCross.chromosomeX[j] = secondIndividualToCross.chromosomeX[j];
                    secondIndividualToCross.chromosomeX[j] = temp;
                }

                divider = _random.Next(0, Convert.ToInt32(numberOfBits));

                for (int j = divider; j < firstIndividualToCross.chromosomeY.Length; ++j)
                {
                    bool temp = firstIndividualToCross.chromosomeY[j];
                    firstIndividualToCross.chromosomeY[j] = secondIndividualToCross.chromosomeY[j];
                    secondIndividualToCross.chromosomeY[j] = temp;
                }

                double random = _random.NextDouble() * 100;
                if (random < crossPercentage && i>= Convert.ToInt32(eliteAmount))
                {
                    population[i] = firstIndividualToCross.Clone();
                    ++i;
                    if (i < population.Length)
                    {
                        population[i] = secondIndividualToCross.Clone();
                    }
                    ++i;
                }
            }
        }

        public void doTwoPointCrossover()
        {
            int divider;
            int divider2;
            for (int i = Convert.ToInt32(eliteAmount); i < population.Length;)
            {
                Individual firstIndividualToCross = populationToCross[_random.Next(0, Convert.ToInt32(populationToCross.Length))].Clone();
                Individual secondIndividualToCross = populationToCross[_random.Next(0, Convert.ToInt32(populationToCross.Length))].Clone();

                divider = _random.Next(0, Convert.ToInt32(numberOfBits)-1);
                divider2 = _random.Next(divider, Convert.ToInt32(numberOfBits));

                for (int j = divider; j < divider2; ++j)
                {
                    bool temp = firstIndividualToCross.chromosomeX[j];
                    firstIndividualToCross.chromosomeX[j] = secondIndividualToCross.chromosomeX[j];
                    secondIndividualToCross.chromosomeX[j] = temp;
                }

                divider = _random.Next(0, Convert.ToInt32(numberOfBits)-1);
                divider2 = _random.Next(divider, Convert.ToInt32(numberOfBits));

                for (int j = divider; j < divider2; ++j)
                {
                    bool temp = firstIndividualToCross.chromosomeY[j];
                    firstIndividualToCross.chromosomeY[j] = secondIndividualToCross.chromosomeY[j];
                    secondIndividualToCross.chromosomeY[j] = temp;
                }

                double random = _random.NextDouble() * 100;
                if (random < crossPercentage && i >= Convert.ToInt32(eliteAmount))
                {
                    population[i] = firstIndividualToCross.Clone();
                    ++i;
                    if (i < population.Length)
                    {
                        population[i] = secondIndividualToCross.Clone();
                    }
                    ++i;
                }
            }
        }

        public void doThreePointCrossover()
        {
            int divider;
            int divider2;
            int divider3;
            for (int i = Convert.ToInt32(eliteAmount); i < population.Length;)
            {
                Individual firstIndividualToCross = populationToCross[_random.Next(0, Convert.ToInt32(populationToCross.Length))].Clone();
                Individual secondIndividualToCross = populationToCross[_random.Next(0, Convert.ToInt32(populationToCross.Length))].Clone();

                divider = _random.Next(0, Convert.ToInt32(numberOfBits) - 2);
                divider2 = _random.Next(divider, Convert.ToInt32(numberOfBits)-1);
                divider3 = _random.Next(divider2, Convert.ToInt32(numberOfBits));

                for (int j = divider; j < divider2; ++j)
                {
                    bool temp = firstIndividualToCross.chromosomeX[j];
                    firstIndividualToCross.chromosomeX[j] = secondIndividualToCross.chromosomeX[j];
                    secondIndividualToCross.chromosomeX[j] = temp;
                }

                for (int j = divider3; j < firstIndividualToCross.chromosomeX.Length; ++j)
                {
                    bool temp = firstIndividualToCross.chromosomeX[j];
                    firstIndividualToCross.chromosomeX[j] = secondIndividualToCross.chromosomeX[j];
                    secondIndividualToCross.chromosomeX[j] = temp;
                }

                divider = _random.Next(0, Convert.ToInt32(numberOfBits) - 2);
                divider2 = _random.Next(divider, Convert.ToInt32(numberOfBits) - 1);
                divider3 = _random.Next(divider2, Convert.ToInt32(numberOfBits));

                for (int j = divider; j < divider2; ++j)
                {
                    bool temp = firstIndividualToCross.chromosomeY[j];
                    firstIndividualToCross.chromosomeY[j] = secondIndividualToCross.chromosomeY[j];
                    secondIndividualToCross.chromosomeY[j] = temp;
                }

                for (int j = divider3; j < firstIndividualToCross.chromosomeY.Length; ++j)
                {
                    bool temp = firstIndividualToCross.chromosomeY[j];
                    firstIndividualToCross.chromosomeY[j] = secondIndividualToCross.chromosomeY[j];
                    secondIndividualToCross.chromosomeY[j] = temp;
                }



                double random = _random.NextDouble() * 100;
                if (random < crossPercentage && i >= Convert.ToInt32(eliteAmount))
                {
                    population[i] = firstIndividualToCross.Clone();
                    ++i;
                    if (i < population.Length)
                    {
                        population[i] = secondIndividualToCross.Clone();
                    }
                    ++i;
                }
            }
        }

        public void doHomoCrossover()
        {
            for (int i = Convert.ToInt32(eliteAmount); i < population.Length;)
            {
                Individual firstIndividualToCross = populationToCross[_random.Next(0, Convert.ToInt32(populationToCross.Length))].Clone();
                Individual secondIndividualToCross = populationToCross[_random.Next(0, Convert.ToInt32(populationToCross.Length))].Clone();

                for (int j = 0; j < firstIndividualToCross.chromosomeX.Length; ++j)
                {
                    if (j % 2 == 0)
                    {
                        bool temp = firstIndividualToCross.chromosomeX[j];
                        firstIndividualToCross.chromosomeX[j] = secondIndividualToCross.chromosomeX[j];
                        secondIndividualToCross.chromosomeX[j] = temp;
                    }
                }

                for (int j = 0; j < firstIndividualToCross.chromosomeY.Length; ++j)
                {
                    if (j % 2 == 0)
                    {
                        bool temp = firstIndividualToCross.chromosomeY[j];
                        firstIndividualToCross.chromosomeY[j] = secondIndividualToCross.chromosomeY[j];
                        secondIndividualToCross.chromosomeY[j] = temp;
                    }
                }


                double random = _random.NextDouble() * 100;
                if (random < crossPercentage && i >= Convert.ToInt32(eliteAmount))
                {
                    population[i] = firstIndividualToCross.Clone();
                    ++i;
                    if (i < population.Length)
                    {
                        population[i] = secondIndividualToCross.Clone();
                    }
                    ++i;
                }
            }
        }

        public void doEdgeMutation()
        {
            for(int i = Convert.ToInt32(eliteAmount); i<population.Length; ++i)
            {
                double random = _random.NextDouble() * 100;
                if (random < mutationPercentage)
                {
                    population[i].chromosomeX[Convert.ToInt32(numberOfBits) - 1] = !population[i].chromosomeX[Convert.ToInt32(numberOfBits) - 1];
                    population[i].chromosomeY[Convert.ToInt32(numberOfBits) - 1] = !population[i].chromosomeY[Convert.ToInt32(numberOfBits) - 1];
                }
            }
        }

        public void doOnePointMutation()
        {
            int randomNumber;
            for (int i = Convert.ToInt32(eliteAmount); i < population.Length; ++i)
            {
                double random = _random.NextDouble() * 100;

                if (random < mutationPercentage && i >= 1)
                {
                    randomNumber = _random.Next(0, Convert.ToInt32(numberOfBits));
                    population[i].chromosomeX[randomNumber] = !population[i].chromosomeX[randomNumber];
                    randomNumber = _random.Next(0, Convert.ToInt32(numberOfBits));
                    population[i].chromosomeY[randomNumber] = !population[i].chromosomeY[randomNumber];
                }
            }
        }

        public void doTwoPointMutation()
        {
            int randomNumber;
            for (int i = Convert.ToInt32(eliteAmount); i < population.Length; ++i)
            {
                double random = _random.NextDouble() * 100;

                if (random < mutationPercentage && i >= 1)
                {
                    randomNumber = _random.Next(0, Convert.ToInt32(numberOfBits));
                    population[i].chromosomeX[randomNumber] = !population[i].chromosomeX[randomNumber];
                    randomNumber = _random.Next(0, Convert.ToInt32(numberOfBits));
                    population[i].chromosomeX[randomNumber] = !population[i].chromosomeX[randomNumber];

                    randomNumber = _random.Next(0, Convert.ToInt32(numberOfBits));
                    population[i].chromosomeY[randomNumber] = !population[i].chromosomeY[randomNumber];
                    randomNumber = _random.Next(0, Convert.ToInt32(numberOfBits));
                    population[i].chromosomeY[randomNumber] = !population[i].chromosomeY[randomNumber];
                }
            }
        }

        //MIN f(x,y)=-1,9133 (x, y)=(-0.54719, -0.54719)
        public double f(bool[] boolX, bool[] boolY)
        {
            int x = BoolArrayToInt(boolX);
            int y = BoolArrayToInt(boolY);

            double m = numberOfBits;

            double newX = a + x * (b - a) / (Math.Pow(2, m) - 1);
            double newY = a + y * (b - a) / (Math.Pow(2, m) - 1);

            return Math.Sin(newX + newY) + (newX - newY) * (newX - newY) - 1.5 * newX + 2.5 * newY + 1;
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

        public  void rewriteBest()
        {
            for (int i = 0; i < eliteAmount; ++i)
            {
                population[i] = bestIndividuals[i].Clone();
            }
        }

        public void fillCharts()
        {
            ObservableCollection<BestValueToEpoch> bestValueToEpochToSet = new ObservableCollection<BestValueToEpoch>();
            ObservableCollection<BestValueToEpoch> avgValueToEpochToSet = new ObservableCollection<BestValueToEpoch>();
            ObservableCollection<BestValueToEpoch> sValueToEpochToSet = new ObservableCollection<BestValueToEpoch>();

            for (int i = 0; i < bb.Length; ++i) {
                bestValueToEpochToSet.Add(new BestValueToEpoch(i, bb[i]));
                avgValueToEpochToSet.Add(new BestValueToEpoch(i, sr[i]));
                sValueToEpochToSet.Add(new BestValueToEpoch(i, s[i]));
            }
            config.bestValueToEpoch = bestValueToEpochToSet;
            config.avgValueToEpoch = avgValueToEpochToSet;
            config.sValueToEpoch = sValueToEpochToSet;
        }
    }
}
