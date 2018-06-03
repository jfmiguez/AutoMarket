using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AutoMarket.API
{
    public class ApiGoogle
    {

        private Http.RestCaller restCaller;
        private String market = "NASDAQ";

        public ApiGoogle()
        {
            restCaller = new Http.RestCaller();
            ApiGoogleHelper.Style.PopulateStyles();
        }

        //public System.Collections.Generic.IEnumerable<Candle> GetHistoricalPrice(string Symbol)
        public System.Collections.Generic.IEnumerable<Candle> GetHistoricalPrice(string Symbol, int interval_s, DateTime? dateTimeFrom, DateTime? dateTimeTo)
        {
            int numberOfSeconds = interval_s;
            var url = ApiGoogleHelper.GetUrl(market, Symbol, numberOfSeconds, dateTimeFrom, dateTimeTo);
            var response = this.restCaller.Get(url);

            if (response == null)
                throw new DataMisalignedException("Market: " + market + ", Symbol " + Symbol + ", Date From " + System.Convert.ToString(dateTimeFrom) + ", Date To " + System.Convert.ToString(dateTimeTo) + " generated exception misaligned data");
            if (response.Length == 0)
                return new Candle[0];

            if ((numberOfSeconds < (24 * 60 * 60)) &&
               ((dateTimeTo == DateTime.MinValue) && dateTimeTo.Value.TimeOfDay.Equals(new TimeSpan(0, 0, 0))))
            {
                // If to value is set and the time has not been set (so it's 0:00) then is set to 23:59:59 
                // to include values for that day in the result.
                dateTimeTo = dateTimeTo.Value.Add(new TimeSpan(23, 59, 59));
            }

            var reader = new LatestCandleReader(numberOfSeconds);
            return reader.Read(new StringReader(response))
                .Where(c => (!(dateTimeFrom == DateTime.MinValue) || c.Date >= dateTimeFrom) &&
                            (!(dateTimeTo == DateTime.MinValue) || c.Date <= dateTimeTo));
        }



        public String[] GetRawHistoricalPrice(string Symbol)
        {
            String[] Data = null;

            return Data;
        }




        /// <summary>
        /// Queries the most recent quote from Google in Real-Time
        /// </summary>
        /// <returns>A candle stick of OHLC, date, and volume data</returns>
        public Candle GetQuote(String Symbol)
        {
            //http://finance.google.com/finance/getprices?q=MSFT&x=NASDAQ&i=86600&p=1d&f=d,o,h,l,c,v
            Candle candle = new Candle();

            int numberOfSeconds = 86600;
            var url = ApiGoogleHelper.GetUrl(market, Symbol, numberOfSeconds, DateTime.Now, DateTime.Now);
            var response = this.restCaller.Get(url);

            if (response == null)
                throw new DataMisalignedException("Market: " + market + ", Symbol " + Symbol + ", Date From " + System.Convert.ToString(DateTime.Now) + ", Date To " + System.Convert.ToString(DateTime.Now) + " generated exception misaligned data");
            if (response.Length == 0)
                return new Candle();

            var reader = new LatestCandleReader(numberOfSeconds);
            var read = reader.Read(new StringReader(response))
                .Where(c => (!(DateTime.Now == DateTime.MinValue)));

            foreach (var r in read)
            {
                candle.Close = r.Close;
                candle.Date = r.Date;
                candle.High = r.High;
                candle.Low = r.Low;
                candle.Open = r.Open;
                candle.Volume = r.Volume;
            }

            return candle;

        }





    }
}
