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
        //private const string YahooBaseUrl = "http://finance.google.com/finance/getprices?q={0}&x={1}&i={2}&p={3}&f=d,c,h,l,o,v";
        private const string YahooBaseUrl = "https://query1.finance.yahoo.com/v7/finance/chart/{symbol}?range={daterange}&interval={interval}&indicators=quote&includeTimestamps=true&includePrePost=false&corsDomain=finance.yahoo.com";
        private const int FirstYear = 1990;

        public ApiYahoo()
        {
            restCaller = new Http.RestCaller();
        }



















        //-----------------------------------------------------
        // FOR HISTORICAL VALUES ONLY
        //
        //    (not working)
        //
        //-----------------------------------------------------




















        //Example of 25 years with a 1d resolution
        //https://query1.finance.yahoo.com/v7/finance/chart/MSFT?range=25y&interval=1d&indicators=quote&includeTimestamps=true&includePrePost=false&corsDomain=finance.yahoo.com

        

        public System.Collections.Generic.IEnumerable<Candle> GetHistoricalPrice(string Symbol, int interval_s,  DateTime? dateTimeFrom, DateTime? dateTimeTo)
        {
            String market = "NASDAQ";
            List<Candle> lCandle = new List<Candle>();
            int numberOfSeconds = 60;

            dateTimeTo = DateTime.Today;
            var url = ApiYahoo.GetUrl(market, Symbol, interval_s, dateTimeFrom, dateTimeTo);
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
            
            var data = JsonConvert.DeserializeObject<RootObject>(response);

            //OHCL
            for (int i = 0; i < data.chart.result[0].indicators.quote[0].close.Count; i++)
            {
                if (data.chart.result[0].indicators.quote[0].open[i].HasValue)
                {
                    Candle candle = new Candle();
                    DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    dtDateTime = dtDateTime.AddSeconds( data.chart.result[0].timestamp[i] ).ToLocalTime();
                    candle.Date = dtDateTime;
                    candle.Open = System.Convert.ToDecimal(data.chart.result[0].indicators.quote[0].open[i].Value.ToString("#.00"));
                    candle.High = System.Convert.ToDecimal(data.chart.result[0].indicators.quote[0].high[i].Value.ToString("#.00"));
                    candle.Low = System.Convert.ToDecimal(data.chart.result[0].indicators.quote[0].low[i].Value.ToString("#.00"));
                    candle.Close = System.Convert.ToDecimal(data.chart.result[0].indicators.quote[0].close[i].Value.ToString("#.00"));
                    candle.Volume = System.Convert.ToDecimal(data.chart.result[0].indicators.quote[0].volume[i].Value.ToString("#"));
                    lCandle.Add(candle);
                    candle = null;
                }
            }

            //Trick! This automatically converts it to the ienumerable type return by value function :-)
            return lCandle;
        }
         
        
        /// <summary>
        /// Queries the Url for the yahoo historical values. 
        /// It's kept the same as google for obvious reasons and simplicity.
        /// However, Yahoo limits the intervals and limits the range per interval
        /// and therefore it is needed to create a valid interval based on seconds provided.
        /// Also for the range if the interval is too small and a big range then yahoo won't
        /// provide data but an error therefore a "table" will be created.
        /// </summary>
        /// <param name="market">The market where the stock is being traded (for compatibility only)</param>
        /// <param name="symbol">Symbol for which the data is required (eg. msft)</param>
        /// <param name="numberOfSeconds">
        /// Total amount of seconds as per interval. 
        /// The granulity was set by google because they have a much finer interval than yahoo, 
        /// and this one has to be parsed.</param>
        /// <param name="from">Starting date for the required range.</param>
        /// <param name="to">Ending date for the required range.</param>
        /// <returns>The URL that allows the software to generate historical values</returns>
        /// <remarks>
        /// I didn't want to complicate this with a dictionary, therefore a few
        /// if-else statements were created (since this is going to be only for yahoo)
        /// </remarks>
        public static string GetUrl(string market, string symbol, int numberOfSeconds,
                             DateTime? from = null, DateTime? to = null)
        {
            //Valid intervals: [1m, 2m, 5m, 15m, 30m, 60m, 90m, 1h, 1d, 5d, 1wk, 1mo, 3mo]
            // intervals are chosen here based on the amount of seconds (see google api)
            string interval = "";
            if (numberOfSeconds >= 5184000)
                interval = "3mo";
            else if (numberOfSeconds >= 1728000)
                interval = "1mo";
            else if (numberOfSeconds >= 432000)
                interval = "1wk";
            else if (numberOfSeconds >= 432000)
                interval = "5d";
            else if (numberOfSeconds >= 86400)
                interval = "1d";
            else if (numberOfSeconds >= 3600)
                interval = "1h";
            else if (numberOfSeconds >= 5400)
                interval = "90m";
            else if (numberOfSeconds >= 3600)
                interval = "60m";
            else if (numberOfSeconds >= 1800)
                interval = "30m";
            else if (numberOfSeconds >= 900)
                interval = "15m";
            else if (numberOfSeconds >= 300)
                interval = "5m";
            else if (numberOfSeconds >= 120)
                interval = "2m";
            else
                interval = "1m";

            //prevent future dates
            if (to.HasValue && (to > DateTime.Today))
                to = DateTime.Today;

            //Table of valid intervals vs. range
            //
            //  1m  -   7 days
            //  2m  -  60 days
            //  5m  -  60 days
            // 15m  -  60 days
            // 30m  - 730 days
            // 60m  - 730 days
            // 90m  -  60 days
            //  1h  - 730 days
            //  1d  - unlimited days (over 200 years)
            //  5d  - unlimited days (over 200 years)
            //  1wk - unlimited days (over 200 years)
            //  1mo - unlimited days (over 200 years)
            //  3mo - unlimited days (over 200 years)
            // Not all ranges are compatible with the intervals due to granularity
            // and therefore need to parse a bit more than usual. For example, if the
            // interval is 1 minute, we can't use the range of 1 year since it would be
            // too much data for yahoo to handle but we are limited to 7 days which for
            // most people is enough.
            string dateRange = "";
            TimeSpan tsdateRange = (to.Value - from.Value);
            if ((interval == "1m") && (tsdateRange.TotalDays > 7))
                dateRange = "7d";
            else if (((interval == "2m") || (interval == "5m") || (interval == "15m") || (interval == "90m")) && (tsdateRange.TotalDays > 60))
                dateRange = "60d";
            else if (((interval == "30m") || (interval == "60m") || (interval == "1h")) && (tsdateRange.TotalDays > 730))
                dateRange = "730d";
            else
                dateRange = "";

            // ToDo: Find out which dates are closed.
            //There will be a difference of 9 days because we are not yet calculating the parameters that allows us to subtract 
            // the actual date that the market is closed. For now I can live without that, but it has to be noted that we are not able to do it yet
            // and for this we would need some sort of either dictionary or better yet a list of dates that list the holidays or the bank holidays.
            int totalBusinessDays = BusinessDays.BusinessDaysUntil(from.Value, to.Value);

            //If the dateRange was not previously established then we need to come up with a date range.
            if (dateRange == "")
            {
                if (tsdateRange.TotalDays < 1)
                    dateRange = "1d";                           //minimum to set is 1d (intraday)
                else if (tsdateRange.TotalDays >= (200 * 365))
                    dateRange = "200y";                         //maximum to set is 200y (stock market wasn't there in 200y)
                else if (totalBusinessDays >= 252)
                    dateRange = System.Convert.ToInt32(totalBusinessDays / 252).ToString() + "y"; //convert to years
                else if ((totalBusinessDays % 5) == 0)
                    dateRange = System.Convert.ToInt32(totalBusinessDays / 5).ToString() + "wk"; //convert to weeks
                else
                    dateRange = System.Convert.ToInt32(totalBusinessDays).ToString() + "d";       //convert to days
            }
            
            string returnedUrl = YahooBaseUrl.Replace("{symbol}", symbol).Replace("{daterange}", dateRange).Replace("{interval}", interval);
            //"https://query1.finance.yahoo.com/v7/finance/chart/{symbol}?range={daterange}&interval={interval}&indicators=quote&includeTimestamps=true&includePrePost=false&corsDomain=finance.yahoo.com";

            return returnedUrl;
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


        public class Pre
        {
            public string timezone { get; set; }
            public int start { get; set; }
            public int end { get; set; }
            public int gmtoffset { get; set; }
        }

        public class Regular
        {
            public string timezone { get; set; }
            public int start { get; set; }
            public int end { get; set; }
            public int gmtoffset { get; set; }
        }

        public class Post
        {
            public string timezone { get; set; }
            public int start { get; set; }
            public int end { get; set; }
            public int gmtoffset { get; set; }
        }

        public class CurrentTradingPeriod
        {
            public Pre pre { get; set; }
            public Regular regular { get; set; }
            public Post post { get; set; }
        }

        public class Meta
        {
            public string currency { get; set; }
            public string symbol { get; set; }
            public string exchangeName { get; set; }
            public string instrumentType { get; set; }
            public int firstTradeDate { get; set; }
            public int gmtoffset { get; set; }
            public string timezone { get; set; }
            public string exchangeTimezoneName { get; set; }
            public double chartPreviousClose { get; set; }
            public double previousClose { get; set; }
            public int scale { get; set; }
            public CurrentTradingPeriod currentTradingPeriod { get; set; }
            public List<List<CurrentTradingPeriod>> tradingPeriods { get; set; }
            public string dataGranularity { get; set; }
            public List<string> validRanges { get; set; }
        }

        public class Quote
        {
            public List<double?> high { get; set; }
            public List<double?> low { get; set; }
            public List<int?> volume { get; set; }
            public List<double?> close { get; set; }
            public List<double?> open { get; set; }
        }

        public class Indicators
        {
            public List<Quote> quote { get; set; }
        }



        public class Chart
        {
            public List<Result> result { get; set; }
            public object error { get; set; }
        }
























        //--------------------------------------------------------
        // FOR REAL TIME QUOTES ONLY
        //--------------------------------------------------------

















        /// <summary>
        /// Gets a URL that ultimately provides a detailed real-time quote 
        /// </summary>
        /// <param name="Symbol">The symbol to get the real-quote</param>
        /// <returns>A string-based quote for the acquired symbol</returns>
        /// <example>
        /// Symbol: MSFT
        /// Returns: https://query1.finance.yahoo.com/v7/finance/quote?symbols=MSFT&view=detail
        /// </example>
        private static string GetQuoteUrl(String Symbol)
        {
            String QuoteBaseUrl = "https://query1.finance.yahoo.com/v7/finance/quote?symbols={symbol}&view=detail";

            String Url = QuoteBaseUrl.Replace("{symbol}", Symbol);
            return Url;
        }

        /// <summary>
        /// Sets up the class used for storage purposes of a JSON string
        /// because yahoo likes to use the 'quoteresponse' as the root object of
        /// the JSON response.
        /// </summary>
        public class quoteResponse
        {
            public string Query { get; set; }
            public List<Result> Result { get; set; }
        }

        /// <summary>
        /// Same as above, but declaring the RootObject for decoding the JSON string.
        /// </summary>
        public class RootObject
        {
            public quoteResponse quoteResponse { get; set; }
            public Chart chart { get; set; }
        }

        
        /// <summary>
        /// Querying all parameters from the real-time quote system.
        /// This is necessary to obtain all the results from the JSON string
        /// but only a certain parameters are passed by the function by value.
        /// The rest we are going to leave them here so we can analyze later on
        /// for financial purposes.
        /// </summary>
        public class Result
        {
            public Meta meta { get; set; }
            public List<int> timestamp { get; set; }
            public Indicators indicators { get; set; }

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
        

        /// <summary>
        /// Queries a Real-Time quote from Yahoo! This is the main method that needs to pass for a simple OHLC call.
        /// </summary>
        /// <param name="Symbol">The symbol to acquire the real time quote</param>
        /// <returns>returns the date, volume, and candle for the day as a real time quote</returns>
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
