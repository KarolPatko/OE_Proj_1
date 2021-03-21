using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE_Proj_1.Model
{
    public class AlgorithmConfig
    {
        private static AlgorithmConfig instance = null;
        public double a { get; set; }
        public double b { get; set; }
        public double numberOfBits { get; set; }
        public double populationAmount { get; set; }
        public double epochs { get; set; }
        public string selection { get; set; }
        public double crossPercentage { get; set; }

        public static AlgorithmConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AlgorithmConfig();
                }
                return instance;
            }
        }
    }
}
