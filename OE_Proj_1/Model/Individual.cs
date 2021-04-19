using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE_Proj_1.Model
{
    class Individual : IComparable
    {
        public int id;
        public static double a;
        public static double b;
        public double chromosomeX;
        public double chromosomeY;
        public double result;
        public double distributor;

        private static int idIterator = 1;

        private static Random _random = new Random();

        public Individual(double a, double b)
        {
            id = idIterator;
            ++idIterator;
            chromosomeX = _random.NextDouble() * (b-a) + a;
            chromosomeY = _random.NextDouble() * (b - a) + a;
            Individual.a = a;
            Individual.b = b;
            distributor = 0;
        }

        public int CompareTo(object obj)
        {
            if(obj is Individual)
            {
                return this.result.CompareTo((obj as Individual).result);
            }
            throw new ArgumentException("Object is not a Individual");
        }

        public Individual Clone()
        {
            Individual i = new Individual(a, b);
            i.chromosomeX = this.chromosomeX;
            i.chromosomeY = this.chromosomeY;
            i.result = this.result;

            return i;
        }
    }
}
