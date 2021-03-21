using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE_Proj_1.Model
{
    class Individual : IComparable
    {
        public bool[] chromosomeX;
        public bool[] chromosomeY;
        public double result;

        private static Random _random = new Random();

        public Individual(int size)
        {
            chromosomeX = randBoolArray(size);
            chromosomeY = randBoolArray(size);
        }

        public int CompareTo(object obj)
        {
            if(obj is Individual)
            {
                return this.result.CompareTo((obj as Individual).result);
            }
            throw new ArgumentException("Object is not a Individual");
        }
        static bool[] randBoolArray(int size)
        {

            bool[] ret = new bool[size];
            for (int i = 0; i < size; ++i)
            {
                ret[i] = _random.Next() % 2 == 0 ? false : true;
            }
            return ret;
        }

        override public string ToString()
        {
            string ret = "";

            for(int i = 0; i < chromosomeX.Length; ++i)
            {
                ret += chromosomeX[i] ? "1" : "0";
            }
            ret += " ";
            for (int i = 0; i < chromosomeY.Length; ++i)
            {
                ret += chromosomeY[i] ? "1" : "0";
            }
            ret += " | ";

            return ret;
        }
    }
}
