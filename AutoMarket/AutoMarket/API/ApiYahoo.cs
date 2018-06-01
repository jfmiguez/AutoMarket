using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Bson;


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

        private static string GetQuoteUrl(String Symbol)
        {
            String QuoteBaseUrl = "https://query1.finance.yahoo.com/v7/finance/quote?symbols={symbol}&view=detail";

            String Url = QuoteBaseUrl.Replace("{symbol}", Symbol);
            return Url;
        }

        public class quoteResponse
        {
            public string Query { get; set; }
            public List<Result> Result { get; set; }
        }

        public class RootObject
        {
            public quoteResponse quoteResponse { get; set; }
        }

        
        public class Result
        {
            public string language { get; set; }
            public string quoteType { get; set; }
            public string quoteSourceName { get; set; }
            public string currency { get; set; }
            public double epsTrailingTwelveMonths { get; set; }
            public double epsForward { get; set; }
            public bool esgPopulated { get; set; }
            public bool tradeable { get; set; }
            public string shortName { get; set; }
            public double regularMarketPrice { get; set; }
            public int regularMarketTime { get; set; }
            public double regularMarketChange { get; set; }
            public double regularMarketOpen { get; set; }
            public double regularMarketDayHigh { get; set; }
            public double regularMarketDayLow { get; set; }
            public int regularMarketVolume { get; set; }
            public string market { get; set; }
            public int priceHint { get; set; }
            public double preMarketChange { get; set; }
            public double preMarketChangePercent { get; set; }
            public int preMarketTime { get; set; }
            public double preMarketPrice { get; set; }
            public double regularMarketChangePercent { get; set; }
            public string regularMarketDayRange { get; set; }
            public double regularMarketPreviousClose { get; set; }
            public double bid { get; set; }
            public double ask { get; set; }
            public int bidSize { get; set; }
            public int askSize { get; set; }
            public string messageBoardId { get; set; }
            public string fullExchangeName { get; set; }
            public string longName { get; set; }
            public string financialCurrency { get; set; }
            public int averageDailyVolume3Month { get; set; }
            public int averageDailyVolume10Day { get; set; }
            public double fiftyTwoWeekLowChange { get; set; }
            public string fiftyTwoWeekRange { get; set; }
            public double fiftyTwoWeekHighChange { get; set; }
            public double fiftyTwoWeekHighChangePercent { get; set; }
            public double fiftyTwoWeekLow { get; set; }
            public double fiftyTwoWeekHigh { get; set; }
            public int dividendDate { get; set; }
            public int earningsTimestamp { get; set; }
            public int earningsTimestampStart { get; set; }
            public string exchange { get; set; }
            public double fiftyTwoWeekLowChangePercent { get; set; }
            public int earningsTimestampEnd { get; set; }
            public double trailingAnnualDividendRate { get; set; }
            public double trailingPE { get; set; }
            public double trailingAnnualDividendYield { get; set; }
            public long sharesOutstanding { get; set; }
            public double bookValue { get; set; }
            public double fiftyDayAverage { get; set; }
            public double fiftyDayAverageChange { get; set; }
            public double fiftyDayAverageChangePercent { get; set; }
            public double twoHundredDayAverage { get; set; }
            public double twoHundredDayAverageChange { get; set; }
            public double twoHundredDayAverageChangePercent { get; set; }
            public long marketCap { get; set; }
            public double forwardPE { get; set; }
            public double priceToBook { get; set; }
            public int sourceInterval { get; set; }
            public string exchangeTimezoneName { get; set; }
            public string exchangeTimezoneShortName { get; set; }
            public int gmtOffSetMilliseconds { get; set; }
            public int exchangeDataDelayedBy { get; set; }
            public string marketState { get; set; }
            public string symbol { get; set; }
        }
        
        public Candle GetQuote(String Symbol)
        {
            string Url = GetQuoteUrl(Symbol);
            String cvsData = "";
            Candle candle = new Candle();

            Http.RestCaller restCaller = new Http.RestCaller();
            cvsData = restCaller.Get(Url);
            var data = JsonConvert.DeserializeObject<RootObject>(cvsData);

            //OHCL
            candle.Date = (data.quoteResponse.Result[0].market == "REGULAR") ? candle.Date = DateTime.Now : candle.Date = DateTime.Today;
            candle.Open = System.Convert.ToDecimal(data.quoteResponse.Result[0].regularMarketOpen);
            candle.High = System.Convert.ToDecimal(data.quoteResponse.Result[0].regularMarketDayHigh);
            candle.Close = System.Convert.ToDecimal(data.quoteResponse.Result[0].regularMarketPrice);
            candle.Low = System.Convert.ToDecimal(data.quoteResponse.Result[0].regularMarketDayLow);
            candle.Volume = System.Convert.ToDecimal(data.quoteResponse.Result[0].regularMarketVolume);
            return candle;
        }

    }
}
