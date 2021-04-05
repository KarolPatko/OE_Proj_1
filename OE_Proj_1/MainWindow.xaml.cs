using OE_Proj_1.Model;
using System;
using System.Collections.Generic;
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

            int chromosomeLength = Convert.ToInt32(numberOfBits);
            for (int i = 0; i < populationAmount; ++i)
            {
                population[i] = new Individual(chromosomeLength);
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

        //Dodac prawdopodobienstwo, teraz mutuje zawsze
        public void doMutatuion()
        {
            switch (mutation)
            {
                case "EDGE":
                    doEdgeMutation();
                    break;
                case "ONE_POINT":
                    doOnePointMutation();
                    break;
                case "TWO_POINT":
                    doOnePointMutation();
                    break;
            }
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
            for (int i = 0; i< population.Length; ++i)
            {
                Individual firstIndividualToCross = populationToCross[_random.Next(0, Convert.ToInt32(populationToCross.Length))];
                Individual secondIndividualToCross = populationToCross[_random.Next(0, Convert.ToInt32(populationToCross.Length))];

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

                population[i] = firstIndividualToCross;
                ++i;
                if(i < population.Length)
                {
                    population[i] = secondIndividualToCross;
                }
            }
        }

        public void doTwoPointCrossover()
        {
            int divider;
            int divider2;
            for (int i = 0; i < population.Length; ++i)
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
            /*
            shufflePopulationToCross();
            int[] dividers = { 0, 0 };
            for (int i = 0; i < populationToCross.Length && i + 1 <= populationToCross.Length; i++)
            {
                dividers[0] = _random.Next(0, Convert.ToInt32(numberOfBits) + 1);
                dividers[1] = _random.Next(0, Convert.ToInt32(numberOfBits) + 1);
                Array.Sort(dividers);

                while(dividers[0] == dividers[1])
                {
                    dividers[1] = _random.Next(0, Convert.ToInt32(numberOfBits) + 1);
                }

                Array.Sort(dividers);

                bool temp;
                
                for (int j = dividers[0]; j < dividers[1]; ++j)
                {
                    temp = populationToCross[i].chromosomeX[j];
                    populationToCross[i].chromosomeX[j] = populationToCross[i].chromosomeY[j];
                    populationToCross[i].chromosomeY[j] = temp;
                }
            }
            */
        }

        public void doThreePointCrossover()
        {
            int divider;
            int divider2;
            int divider3;
            for (int i = 0; i < population.Length; ++i)
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
            /*
            shufflePopulationToCross();
            int[] dividers = { 0, 0, 0 };
            for (int i = 0; i < populationToCross.Length && i + 1 <= populationToCross.Length; i++)
            {
                dividers[0] = _random.Next(0, Convert.ToInt32(numberOfBits) + 1);
                dividers[1] = _random.Next(0, Convert.ToInt32(numberOfBits) + 1);
                dividers[2] = _random.Next(0, Convert.ToInt32(numberOfBits) + 1);

                while(new HashSet<int>(dividers).Count == dividers.Length)
                {
                    dividers[0] = _random.Next(0, Convert.ToInt32(numberOfBits) + 1);
                    dividers[1] = _random.Next(0, Convert.ToInt32(numberOfBits) + 1);
                    dividers[2] = _random.Next(0, Convert.ToInt32(numberOfBits) + 1);   
                }

                Array.Sort(dividers);

                bool temp;

                for (int j = 0; j < dividers[0]; ++j)
                {
                    temp = populationToCross[i].chromosomeX[j];
                    populationToCross[i].chromosomeX[j] = populationToCross[i].chromosomeY[j];
                    populationToCross[i].chromosomeY[j] = temp;
                }

                for (int j = dividers[1]; j < dividers[2]; ++j)
                {
                    temp = populationToCross[i].chromosomeX[j];
                    populationToCross[i].chromosomeX[j] = populationToCross[i].chromosomeY[j];
                    populationToCross[i].chromosomeY[j] = temp;
                }
            }
            */
        }

        public void doHomoCrossover()
        {
            for (int i = 0; i < population.Length; ++i)
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
            /*
            //pobrać s z formularza, jak na razie fixed s = 0,25
            double s = 0.25;
            bool temp;

            for (int i = 0; i < populationToCross.Length; i++)
            {
                for(int j = 0; j < populationToCross[i].chromosomeX.Length && j < populationToCross[i].chromosomeY.Length; ++j)
                {
                    double epsilon = _random.Next(0, 1);
                    if(epsilon <= s)
                    {
                        temp = populationToCross[i].chromosomeX[j];
                        populationToCross[i].chromosomeX[j] = populationToCross[i].chromosomeY[j];
                        populationToCross[i].chromosomeY[j] = temp;
                    }
                }    
            }
            */
        }

        public void doEdgeMutation()
        {
            for(int i = 0; i<population.Length; ++i)
            {
                population[i].chromosomeX[Convert.ToInt32(numberOfBits) - 1] = !population[i].chromosomeX[Convert.ToInt32(numberOfBits) - 1];
                population[i].chromosomeY[Convert.ToInt32(numberOfBits) - 1] = !population[i].chromosomeY[Convert.ToInt32(numberOfBits) - 1];
            }
        }

        public void doOnePointMutation()
        {
            int randomNumber;
            for (int i = 0; i < population.Length; ++i)
            {
                randomNumber = _random.Next(0, Convert.ToInt32(numberOfBits));
                population[i].chromosomeX[randomNumber] = !population[i].chromosomeX[randomNumber];
                randomNumber = _random.Next(0, Convert.ToInt32(numberOfBits));
                population[i].chromosomeY[randomNumber] = !population[i].chromosomeY[randomNumber];
            }
        }

        public void doTwoPointMutation()
        {
            int randomNumber;
            for (int i = 0; i < population.Length; ++i)
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

        /*
        private string ToBinaryString(double value)
        {
            const int bitCount = sizeof(double) * 8;
            int intValue = System.BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
            return Convert.ToString(intValue, 2).PadLeft(bitCount, '0');
        }

        private float FromBinaryString(string bstra)
        {
            int intValue = Convert.ToInt32(bstra, 2);
            return BitConverter.ToSingle(BitConverter.GetBytes(intValue), 0);
        }
        */
    }
}
