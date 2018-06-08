using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMarket.Indicators
{
    public class IndicatorData
    {
        /// <summary>
        /// Current indicator name
        /// </summary>
        public String IndicatorName { set; get; }

        /// <summary>
        /// Current symbol
        /// </summary>
        public String Symbol { set; get; }

        /// <summary>
        /// Sample date and time
        /// </summary>
        public DateTime CurrentDate { set; get; }

        /// <summary>
        /// Volume Run Rate at the End of the Day
        /// </summary>
        public Decimal VRREOD { set; get; }

        /// <summary>
        /// Current Volume for the desired stock
        /// </summary>
        public Decimal Volume { set; get; }

        /// <summary>
        /// Volume average during the last 50 days
        /// </summary>
        public Decimal VolumeAverage { set; get; }

    }
}
