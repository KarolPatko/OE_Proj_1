using OE_Proj_1.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace OE_Proj_1
{

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
        private Individual[] bestIndividuals = new Individual[2];
        private static Random _random = new Random();


    public MainWindow()
        {
            InitializeComponent();
        }

        private double[] bb;
        private double[] bbx;
        private double[] bby;
        private static int iterator = 0;

        public void calculate(object sender, RoutedEventArgs e)
        {
            initialize();
            bb = new double[Convert.ToInt32(epochs)*2];
            bbx = new double[Convert.ToInt32(epochs) * 2];
            bby = new double[Convert.ToInt32(epochs) * 2];
            iterator = 0;
            for (int i = 0; i < epochs; ++i)
            {
                doEvaluate();
                getBest();
                doSelection();
                doCrossover();
                doMutatuion();
                doInversion();
                rewriteBest();
            }
            ExampleAsync();


        }

        public void getBest()
        {
            Array.Sort(population);
            bestIndividuals[0] = population[0].Clone();
            bestIndividuals[1] = population[1].Clone();

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

        public void ExampleAsync()
        {
            string abc = "";
            for(int i =0; i<epochs; ++i)
            {
                abc += bb[i] + " " + bbx[i] + " " + bby[i] + "\n";
            }
            string[] hg = new string[1];
            hg[0] = abc;

            File.WriteAllLines("WriteLines.txt", hg);
        }
        public void initialize()
        {
            population = new Individual[Convert.ToInt32(populationAmount)];

            int chromosomeLength = Convert.ToInt32(numberOfBits);
            for (int i = 0; i < populationAmount; ++i)
            {
                population[i] = new Individual(chromosomeLength);
            }
        }

        public void doEvaluate()
        {
            for(int i=0; i<population.Length; ++i)
            {
                population[i].result = f(population[i].chromosomeX, population[i].chromosomeY);
            }
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
            for(int i = 2; i<population.Length; ++i)
            {
                double random = _random.NextDouble();
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
            populationToCross = new Individual[Convert.ToInt32(Math.Floor(population.Length * (bestPercentageOrTournamentAmount * 1.0 / 100)))];
            for(int i = 0; i < (Math.Floor(population.Length * (bestPercentageOrTournamentAmount * 1.0 / 100))); ++i){
                populationToCross[i] = population[i].Clone();
            }
        }

        public void doRouletteSelection()
        {
            //TODO
            doTournamentSelection();
        }

        public void doTournamentSelection()
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


        public void doOnePointCrossover()
        {
            int divider;

            for (int i = 2; i< population.Length;)
            {
                Individual firstIndividualToCross = populationToCross[_random.Next(0, Convert.ToInt32(populationToCross.Length) -1)].Clone();
                Individual secondIndividualToCross = populationToCross[_random.Next(0, Convert.ToInt32(populationToCross.Length)-1)].Clone();

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

                double random = _random.NextDouble();
                if (random < crossPercentage && i>=2)
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
            for (int i = 2; i < population.Length; ++i)
            {
                Individual firstIndividualToCross = populationToCross[_random.Next(0, Convert.ToInt32(populationToCross.Length))];
                Individual secondIndividualToCross = populationToCross[_random.Next(0, Convert.ToInt32(populationToCross.Length))];

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

                population[i] = firstIndividualToCross;
                ++i;
                if (i < population.Length)
                {
                    population[i] = secondIndividualToCross;
                }
            }
        }

        public void doThreePointCrossover()
        {
            int divider;
            int divider2;
            int divider3;
            for (int i = 4; i < population.Length; ++i)
            {
                Individual firstIndividualToCross = populationToCross[_random.Next(0, Convert.ToInt32(populationToCross.Length))];
                Individual secondIndividualToCross = populationToCross[_random.Next(0, Convert.ToInt32(populationToCross.Length))];

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

                population[i] = firstIndividualToCross;
                ++i;
                if (i < population.Length)
                {
                    population[i] = secondIndividualToCross;
                }
            }
        }

        public void doHomoCrossover()
        {
            for (int i = 4; i < population.Length; ++i)
            {
                Individual firstIndividualToCross = populationToCross[_random.Next(0, Convert.ToInt32(populationToCross.Length))];
                Individual secondIndividualToCross = populationToCross[_random.Next(0, Convert.ToInt32(populationToCross.Length))];

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

                population[i] = firstIndividualToCross;
                ++i;
                if (i < population.Length)
                {
                    population[i] = secondIndividualToCross;
                }
            }
        }

        public void doEdgeMutation()
        {
            for(int i = 2; i<population.Length; ++i)
            {
                double random = _random.NextDouble();
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
            for (int i = 2; i < population.Length; ++i)
            {
                double random = _random.NextDouble();

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
            for (int i = 2; i < population.Length; ++i)
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

        public  void rewriteBest()
        {
            population[0] = bestIndividuals[0].Clone();
            population[1] = bestIndividuals[1].Clone();
        }
    }
}
