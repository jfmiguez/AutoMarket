using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AutoMarket.API
{
    public class ApiYahoo
    {
        private Http.RestCaller restCaller;
        private const string BaseUrl = "http://finance.google.com/finance/getprices?q={0}&x={1}&i={2}&p={3}&f=d,c,h,l,o,v";

        private const int FirstYear = 1990;

        public ApiYahoo()
        {
            restCaller = new Http.RestCaller();
        }

        public System.Collections.Generic.IEnumerable<Candle> GetHistoricalPrice(string Symbol)
        {
            String market = "NASDAQ";
            DateTime dateTimeFrom = DateTime.MinValue;
            DateTime dateTimeTo = DateTime.Now;

            int numberOfSeconds = 60;
            DateTime dateFrom = DateTime.MinValue;
            DateTime dateTo = DateTime.MaxValue;
            var url = ApiYahoo.GetUrl(market, Symbol, numberOfSeconds, dateFrom, dateTo);
            var response = this.restCaller.Get(url);

            if (response == null)
                throw new DataMisalignedException("Market: " + market + ", Symbol " + Symbol + ", Date From " + System.Convert.ToString(dateTimeFrom) + ", Date To " + System.Convert.ToString(dateTimeTo) + " generated exception misaligned data");
            if (response.Length == 0)
                return new Candle[0];

            if ((numberOfSeconds < (24 * 60 * 60)) &&
               ((dateTimeTo == DateTime.MinValue) && dateTimeTo.TimeOfDay.Equals(new TimeSpan(0, 0, 0))))
            {
                // If to value is set and the time has not been set (so it's 0:00) then is set to 23:59:59 
                // to include values for that day in the result.
                dateTimeTo = dateTimeTo.Add(new TimeSpan(23, 59, 59));
            }

            var reader = new LatestCandleReader(numberOfSeconds);
            return reader.Read(new StringReader(response))
                .Where(c => (!(dateFrom == DateTime.MinValue) || c.Date >= dateFrom) &&
                            (!(dateTo == DateTime.MinValue) || c.Date <= dateTo));

            //first get a valid token from Yahoo Finance
            //while (string.IsNullOrEmpty(Token.Cookie) || string.IsNullOrEmpty(Token.Crumb))
            //{
            //    await Token.RefreshAsync().ConfigureAwait(false);
            //}

            //var hps = await Historical.GetPriceAsync(symbol, DateTime.Now.AddMonths(-1), DateTime.Now).ConfigureAwait(false);

            //do something

        }

        
        public static string GetUrl(string market, string symbol, int numberOfSeconds,
                             DateTime? from = null, DateTime? to = null)
        {
            var pValue = GetPeriodValueFrom(market, symbol, from, to);
            return string.Format(BaseUrl, symbol, market, numberOfSeconds, pValue);
        }



        private static string GetPeriodValueFrom(string market, string symbol,
            DateTime? from = null, DateTime? to = null)
        {
            if ((to.HasValue && !from.HasValue) ||
                (!to.HasValue && from.HasValue) ||
                (to.HasValue && from.HasValue && to.Value.Date < from.Value.Date))
                throw new FormatException("Date Validation ocurr. From and To Dates should be together or none of  them. From date should be smaller than To Date");

            if (!to.HasValue && !from.HasValue)
                return (DateTime.Today.Year - FirstYear) + "Y";

            // We use DateTime.Today instead of To.Value because Google only returns data from today and not
            // between dates.
            var numberOfDays = (DateTime.Today - from.Value).Days;
            if (numberOfDays < 50)
                return numberOfDays + "d";
            if (numberOfDays < 1500)
                return ((numberOfDays / 25) + 1) + "M";

            return ((numberOfDays / 320) + 1) + "Y";
        }

    }
}
