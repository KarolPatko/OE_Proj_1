using OE_Proj_1.Model;
using OE_Proj_1.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
                rewriteBest();

                ++iterator;
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

            if (a == b) config.error += "Przedział nie może być pusty\n";
            if (populationAmount < 2) config.error += "Populacja musi być większa niż 2\n";
            if (epochs <= 0) config.error += "Liczba epok musi być większa od zera\n";
            populationAmount = Convert.ToInt32(Math.Ceiling(populationAmount));
            if (crossPercentage == 0) crossPercentage = 10;
            if (selection == "BEST" && bestPercentageOrTournamentAmount > 100) bestPercentageOrTournamentAmount = 100;
            if (selection == "TOURNAMENT" && bestPercentageOrTournamentAmount > 100) bestPercentageOrTournamentAmount = 4;
        }

        public void initialize()
        {
            //initialize population
            population = new Individual[Convert.ToInt32(populationAmount)];

            int chromosomeLength = Convert.ToInt32(numberOfBits);
            for (int i = 0; i < populationAmount; ++i)
            {
                population[i] = new Individual(a, b);
            }

            //initialize file with iterations
            string[] empty = new string[1];
            empty[0] = "";
            File.WriteAllLines("charts/iterations.txt", empty);

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
            File.AppendAllLines("charts/iterations.txt", iterationInfo);

            string[] individualInfo = new string[3];
            for(int j = 0; j<population.Length; ++j)
            {
                individualInfo[1] = population[j].chromosomeX.ToString();
                individualInfo[2] = population[j].chromosomeY.ToString();

                individualInfo[0] = " " + population[j].result;
                string[] info = { individualInfo[1] + " " + individualInfo[2] + individualInfo[0] };
                File.AppendAllLines("charts/iterations.txt", info);
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
                default:
                    doBestSelection();
                    break;
            }
        }

        public void doBestSelection()
        {
            Array.Sort(population);

            int populationToCrossLength = population.Length;
            if (crossover == "HEURISTIC") {
                populationToCrossLength = populationToCrossLength * 2;
            }
            populationToCross = new Individual[populationToCrossLength];

            int cycle = 0;
            for(int i = 0; i < populationToCrossLength;) {
                int rand1 = Convert.ToInt32(_random.Next(0, Convert.ToInt32(bestPercentageOrTournamentAmount / 100 * population.Length)));
                int rand2 = Convert.ToInt32(_random.Next(0, Convert.ToInt32(bestPercentageOrTournamentAmount / 100 * population.Length)));

                if(crossover == "HEURISTIC")
                {
                    if((population[rand1].chromosomeX > population[rand2].chromosomeX && population[rand1].chromosomeY > population[rand2].chromosomeY))
                    {
                        double temp = population[rand2].chromosomeX;
                        population[rand2].chromosomeX = population[rand1].chromosomeX;
                        population[rand1].chromosomeX = temp;
                        
                        temp = population[rand2].chromosomeY;
                        population[rand2].chromosomeY = population[rand1].chromosomeY;
                        population[rand1].chromosomeY = temp;
                    }
                    else if((population[rand1].chromosomeX >= population[rand2].chromosomeX || population[rand1].chromosomeY >= population[rand2].chromosomeY) && cycle < 10)
                    {
                        ++cycle;
                        continue;
                    }
                    else
                    {
                        cycle = 0;
                        double maxX = Math.Max(population[rand1].chromosomeX, population[rand2].chromosomeX);
                        double minX = Math.Min(population[rand1].chromosomeX, population[rand2].chromosomeX);
                        double maxY = Math.Max(population[rand1].chromosomeY, population[rand2].chromosomeY);
                        double minY = Math.Min(population[rand1].chromosomeY, population[rand2].chromosomeY);

                        if(maxX != minX)
                        {
                            population[rand2].chromosomeX = maxX;
                            population[rand1].chromosomeX = minX;
                        }
                        else
                        {
                            population[rand1].chromosomeX = a;
                            population[rand2].chromosomeX = b;
                        }
                        if (maxY != minY)
                        {
                            population[rand2].chromosomeY = maxY;
                            population[rand1].chromosomeY = minY;
                        }
                        else
                        {
                            population[rand1].chromosomeY = a;
                            population[rand2].chromosomeY = b;
                        }
                    }
                }

                int rand = _random.Next(0, 100);
                if(rand > crossPercentage)
                {
                    continue;
                }

                populationToCross[i] = population[rand1].Clone();
                i += 1;

                if (i < populationToCrossLength)
                {
                    populationToCross[i] = population[rand2].Clone();
                    i += 1;
                }
            }
        }

        public void doRouletteSelection()
        {
            double min = population[0].result;
            double max = population[0].result;

            for (int i = 1; i<population.Length; ++i)
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

            if(min <= 0)
            {
                for (int i = 0; i < population.Length; ++i)
                {
                    population[i].result += -min + 0.1;
                }
            }

            double sum = 0;
            for (int i = 0; i < population.Length; ++i)
            {
                population[i].result = 1 / population[i].result;
                sum += population[i].result;
            }

            for (int i = 0; i < population.Length; ++i)
            {
                population[i].distributor = population[i].result / sum;

                if(i > 0)
                {
                    population[i].distributor += population[i].distributor;
                }
            }
            population[population.Length - 1].distributor = 1;

            int populationToCrossLength = population.Length;
            if (crossover == "HEURISTIC")
            {
                populationToCrossLength = populationToCrossLength * 2;
            }
            populationToCross = new Individual[populationToCrossLength];
            double rand;
            int cycle = 0;
            for (int i = 0; i < populationToCrossLength;)
            {
                Individual i1 = new Individual(a, b);
                Individual i2 = new Individual(a, b);
                rand = _random.NextDouble();
                for (int j = 0; j < population.Length; ++j)
                {
                    if (rand <= population[j].distributor)
                    {
                        i1 = population[j].Clone();
                        break;
                    }
                }
                rand = _random.NextDouble();
                for (int j = 0; j < population.Length; ++j)
                {
                    if (rand <= population[j].distributor)
                    {
                        i2 = population[j].Clone();
                        break;
                    }
                }



                if (crossover == "HEURISTIC")
                {
                    if ((i1.chromosomeX > i2.chromosomeX && i1.chromosomeY > i2.chromosomeY))
                    {
                        double temp = i2.chromosomeX;
                        i2.chromosomeX = i1.chromosomeX;
                        i1.chromosomeX = temp;

                        temp = i2.chromosomeY;
                        i2.chromosomeY = i1.chromosomeY;
                        i1.chromosomeY = temp;
                    }
                    else if ((i1.chromosomeX >= i2.chromosomeX || i1.chromosomeY >= i2.chromosomeY) && cycle < 10)
                    {
                        ++cycle;
                        continue;
                    }
                    else
                    {
                        cycle = 0;
                        double maxX = Math.Max(i1.chromosomeX, i2.chromosomeX);
                        double minX = Math.Min(i1.chromosomeX, i2.chromosomeX);
                        double maxY = Math.Max(i1.chromosomeY, i2.chromosomeY);
                        double minY = Math.Min(i1.chromosomeY, i2.chromosomeY);

                        if (maxX != minX)
                        {
                            i2.chromosomeX = maxX;
                            i1.chromosomeX = minX;
                        }
                        else
                        {
                            i1.chromosomeX = a;
                            i2.chromosomeX = b;
                        }
                        if (maxY != minY)
                        {
                            i2.chromosomeY = maxY;
                            i1.chromosomeY = minY;
                        }
                        else
                        {
                            i1.chromosomeY = a;
                            i2.chromosomeY = b;
                        }
                    }
                }

                rand = _random.Next(0, 100);
                if (rand > crossPercentage)
                {
                    continue;
                }

                populationToCross[i] = i1.Clone();
                i += 1;

                if(i < populationToCrossLength)
                {
                    populationToCross[i] = i2.Clone();
                    i += 1;
                }
            }
        }

        public void doTournamentSelection()
        {

            int populationToCrossLength = population.Length;
            if (crossover == "HEURISTIC")
            {
                populationToCrossLength = populationToCrossLength * 2;
            }
            populationToCross = new Individual[populationToCrossLength];

            int cycle = 0;
            for(int i = 0 ; i < populationToCrossLength;)
            {
                shufflePopulation();
                Individual min1 = population[0].Clone();
                for (int j = 1; j < Convert.ToInt32(bestPercentageOrTournamentAmount); j += Convert.ToInt32(bestPercentageOrTournamentAmount))
                {
                    for(int k = j; k < j + bestPercentageOrTournamentAmount && k < population.Length; ++k)
                    {
                        if(population[k].result < min1.result)
                        {
                            min1 = population[k].Clone();
                        }
                    }
                }

                if(crossover == "ARITHMETIC")
                {
                    populationToCross[i] = min1.Clone();
                    i += 1;
                    continue;
                }

                shufflePopulation();
                Individual min2 = population[0].Clone();
                for (int j = 1; j < Convert.ToInt32(bestPercentageOrTournamentAmount); j += Convert.ToInt32(bestPercentageOrTournamentAmount))
                {
                    for (int k = j; k < j + bestPercentageOrTournamentAmount && k < population.Length; ++k)
                    {
                        if (population[k].result < min2.result)
                        {
                            min2 = population[k].Clone();
                        }
                    }
                }

                if (crossover == "HEURISTIC")
                {
                    if ((min1.chromosomeX > min2.chromosomeX && min1.chromosomeY > min2.chromosomeY))
                    {
                        double temp = min2.chromosomeX;
                        min2.chromosomeX = min1.chromosomeX;
                        min1.chromosomeX = temp;

                        temp = min2.chromosomeY;
                        min2.chromosomeY = min1.chromosomeY;
                        min1.chromosomeY = temp;
                    }
                    else if ((min1.chromosomeX >= min2.chromosomeX || min1.chromosomeY >= min2.chromosomeY) && cycle < 10)
                    {
                        ++cycle;
                        continue;
                    }
                    else
                    {
                        cycle = 0;
                        double maxX = Math.Max(min1.chromosomeX, min2.chromosomeX);
                        double minX = Math.Min(min1.chromosomeX, min2.chromosomeX);
                        double maxY = Math.Max(min1.chromosomeY, min2.chromosomeY);
                        double minY = Math.Min(min1.chromosomeY, min2.chromosomeY);

                        if (maxX != minX)
                        {
                            min2.chromosomeX = maxX;
                            min1.chromosomeX = minX;
                        }
                        else
                        {
                            min1.chromosomeX = a;
                            min2.chromosomeX = b;
                        }
                        if (maxY != minY)
                        {
                            min2.chromosomeY = maxY;
                            min1.chromosomeY = minY;
                        }
                        else
                        {
                            min1.chromosomeY = a;
                            min2.chromosomeY = b;
                        }
                    }
                }

                populationToCross[i] = min1.Clone();
                i += 1;

                if (i < populationToCrossLength)
                {
                    populationToCross[i] = min2.Clone();
                    i += 1;
                }
            }

        }

        public void doCrossover()
        {
            switch (crossover)
            {
                case "ARITHMETIC":
                    doArithmeticCrossover();
                    break;
                case "HEURISTIC":
                    doHeuristicCrossover();
                    break;
                default:
                    doArithmeticCrossover();
                    break;
            }
        }

        public void doArithmeticCrossover()
        {
            double k;
            do
            {
                k = _random.NextDouble();
            } while (k == 0);

            for(int i = 0; i < population.Length; i += 2)
            {
                if( i == population.Length - 1)
                {
                    population[i] = populationToCross[i].Clone();
                    continue;
                }

                Individual new1 = new Individual(a, b);
                Individual new2 = new Individual(a, b);

                double x1New = k * populationToCross[i].chromosomeX + (1 - k) * populationToCross[i+1].chromosomeX;
                double y1New = k * populationToCross[i].chromosomeY + (1 - k) * populationToCross[i + 1].chromosomeY;

                double x2New = (1 - k) * populationToCross[i].chromosomeX + k * populationToCross[i + 1].chromosomeX;
                double y2New = (1 - k) * populationToCross[i].chromosomeY + k * populationToCross[i + 1].chromosomeY;

                new1.chromosomeX = x1New;
                new1.chromosomeY = y1New;
                new2.chromosomeX = x2New;
                new2.chromosomeY = y2New;

                population[i] = new1.Clone();
                population[i + 1] = new2.Clone();
            }
        }

        public void doHeuristicCrossover()
        {
            double k;
            do
            {
                k = _random.NextDouble();
            } while (k == 0);

            for (int i = 0; i< population.Length; ++i)
            {
                double x1New = k * (populationToCross[i*2 + 1].chromosomeX - populationToCross[i*2].chromosomeX) + populationToCross[i * 2].chromosomeX;
                double y1New = k * (populationToCross[i * 2 + 1].chromosomeY - populationToCross[i * 2].chromosomeY) + populationToCross[i * 2].chromosomeY;

                population[i].chromosomeX = x1New;
                population[i].chromosomeY = y1New;
            }
        }

        public void doMutatuion()
        {
            switch (mutation)
            {
                case "EVEN":
                    doEvenCrossover();
                    break;
                case "INDEX_SWAP":
                    doIndexSwapCrossover();
                    break;
                case "GAUSS":
                    doGaussCrossover();
                    break;
                default:
                    doEvenCrossover();
                    break;
            }
        }

        public void doEvenCrossover() {
            for(int i = 0;i < population.Length; ++i)
            {
                double rand = _random.Next(0, 100);

                if(rand < mutationPercentage)
                {
                    rand = _random.Next(0, 2);

                    if(rand == 0){
                        rand = _random.NextDouble() * (b - a) + a;
                        population[i].chromosomeX = rand;
                    }
                    else{
                        rand = _random.NextDouble() * (b - a) + a;
                        population[i].chromosomeY = rand;
                    }
                }
            }
        }
        public void doIndexSwapCrossover()
        {
            for (int i = 0; i < population.Length; ++i)
            {
                double rand = _random.Next(0, 100);

                if (rand < mutationPercentage)
                {
                    double temp = population[i].chromosomeX;
                    population[i].chromosomeX = population[i].chromosomeY;
                    population[i].chromosomeY = temp;
                }
            }
        }
        public void doGaussCrossover()
        {
            for (int i = 0; i < population.Length; ++i)
            {
                double rand = _random.Next(0, 100);

                if (rand < mutationPercentage)
                {
                    double u1= 1.0 - _random.NextDouble();
                    double u2 = 1.0 - _random.NextDouble();
                    double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
                    double randNormal = sr[iterator] + s[iterator] - randStdNormal;
                    population[i].chromosomeX += randNormal;

                    if(population[i].chromosomeX < a || population[i].chromosomeX > b)
                    {
                        population[i].chromosomeX = _random.NextDouble() * (b - a) + a;
                    }

                    u1 = 1.0 - _random.NextDouble();
                    u2 = 1.0 - _random.NextDouble();
                    randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
                    randNormal = sr[iterator] + s[iterator] - randStdNormal;
                    population[i].chromosomeY += randNormal;

                    if (population[i].chromosomeY < a || population[i].chromosomeY > b)
                    {
                        population[i].chromosomeY = _random.NextDouble() * (b - a) + a;
                    }
                }
            }
        }

        //MIN f(x,y)=-1,9133 (x, y)=(-0.54719, -0.54719)
        public double f(double x, double y)
        {
            return Math.Sin(x + y) + (x - y) * (x - y) - 1.5 * x + 2.5 * y + 1;
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
