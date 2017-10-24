using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFrobotWindowIoTTempelate
{
    internal class DataContainer
    {

        internal double temperature;
        internal int moisture;

        public DataContainer() { }

        public DataContainer(double temperature, int moisture) {
            Temperature = temperature;
            Moisture = moisture;
        }
        public double Temperature
        {
            get;
            set;
        }

        public int Moisture
        {
            get;
            set;
        }

    }
}
