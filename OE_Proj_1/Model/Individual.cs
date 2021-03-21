using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE_Proj_1.Model
{
    class Individual
    {
        private byte[] chromosomeX { get; set; }
        private byte[] chromosomeY { get; set; }
        private double result { get; set; }

        public Individual(int size)
        {
            chromosomeX = new byte[Convert.ToInt32(size)];
            chromosomeY = new byte[Convert.ToInt32(size)];
        }
    }
}
