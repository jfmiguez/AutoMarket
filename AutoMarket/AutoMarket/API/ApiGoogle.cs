using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMarket.API
{
    public class ApiGoogle
    {

        private ApiGoogleHelper.Http.RestCaller restCaller; 

        public ApiGoogle()
        {
            restCaller = new ApiGoogleHelper.Http.RestCaller();
            ApiGoogleHelper.Style.PopulateStyles();
        }

        public Candle[] GetHistoricalPrice(string Symbol)
        {
            Candle[] Data = null;

            String market = "NASDAQ";
            DateTime dateTimeFrom = DateTime.MinValue;
            DateTime dateTimeTo = DateTime.Now;

            //Nuba.Finance.Google.LatestQuotesService quoteService = new Nuba.Finance.Google.LatestQuotesService();
            //var candle = quoteService.GetValues(market, Symbol, 60, dateTimeFrom, dateTimeTo);

            int numberOfSeconds = 60;
            DateTime dateFrom = DateTime.MinValue;
            DateTime dateTo = DateTime.MaxValue;
            var url = ApiGoogleHelper.GetUrl(market, Symbol, numberOfSeconds, dateFrom, dateTo);
            var response = this.restCaller.Get(url);

            if (response == null)
                throw new DataMisalignedException("Market: " + market + ", Symbol " + Symbol + ", Date From " + System.Convert.ToString(dateTimeFrom) + ", Date To " + System.Convert.ToString(dateTimeTo) + " generated exception misaligned data");
            if (response.Length == 0)
                return new Candle[0];

            if ((numberOfSeconds < Frequency.EveryDay) &&
               (dateTimeTo.HasValue && dateTimeTo.Value.TimeOfDay.Equals(new TimeSpan(0, 0, 0))))
            {
                // If to value is set and the time has not been set (so it's 0:00) then is set to 23:59:59 
                // to include values for that day in the result.
                dateTimeTo = dateTimeTo.Value.Add(new TimeSpan(23, 59, 59));
            }

            var reader = new LatestCandleReader(numberOfSeconds);
            return reader.Read(new StringReader(response))
                .Where(c => (!from.HasValue || c.Date >= from.Value) &&
                            (!to.HasValue || c.Date <= to.Value));


            return Data;
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
            Candle ddd = new Candle();



            return ddd;
        }





    }
}
