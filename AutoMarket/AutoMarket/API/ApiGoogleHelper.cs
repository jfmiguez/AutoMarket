using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace AutoMarket.API
{
    public abstract class ApiGoogleHelper
    {
        private const string BaseUrl = "http://finance.google.com/finance/getprices?q={0}&x={1}&i={2}&p={3}&f=d,c,h,l,o,v";

        private const int FirstYear = 1990;

        /// <summary>
        /// Gets the URL associated with the symbol for historical data.
        /// </summary>
        /// <param name="market"></param>
        /// <param name="symbol"></param>
        /// <param name="numberOfSeconds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
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


        public abstract class Conversions
        {

            /// <summary>
            /// Converts a Google Timestamp made in POSIX UNIX to a conventional date
            /// </summary>
            /// <param name="TimeStamp">This is google's Timestamp. See Notes.</param>
            /// <returns>Returns a date from the conversion</returns>
            /// <example>
            /// TimeStamp = 1527278340
            /// days = 43246.06181
            /// dateTime = 5/27/2018
            /// </example>
            public DateTime ConvertGoogleTimeStampToDate(long TimeStamp)
            {
                DateTime dateTime = new DateTime(1899, 12, 30);

                Double days = TimeStamp / 86400d + 25569d + (5.5d / 24d);

                dateTime.AddDays(days);

                return dateTime;
            }

            /// <summary>
            /// Converts a Google Timestamp made in POSIX UNIX to a conventional date
            /// </summary>
            /// <param name="TimeStamp">This is google's Timestamp. See Notes.</param>
            /// <returns>Returns a date from the conversion</returns>
            /// <example>
            /// TimeStamp = a1527278340
            /// days = 43246.06181
            /// dateTime = 5/27/2018
            /// </example>
            public DateTime ConvertGoogleTimeStampToDate(string timeStamp)
            {
                return ConvertGoogleTimeStampToDate(Convert.ToInt64(timeStamp.Replace("a", "").Replace("A", "")));
            }


            /// <summary>
            /// Converts each individual interval to a date and time value.
            /// </summary>
            /// <param name="initialDate"></param>
            /// <param name="interval"></param>
            /// <param name="period"></param>
            /// <returns></returns>
            public DateTime ConvertGoogleOffsetToDate(DateTime initialDate, int interval, String period)
            {
                return DateTime.Now;
            }

        }


        public class Http
        {


            internal class RestCaller
            {
                public string Get(string url)
                {
                    HttpClient client = new HttpClient();
                    var getTask = client.GetAsync(url);
                    getTask.Wait();
                    var response = getTask.Result;

                    if (!response.IsSuccessStatusCode)
                        return null;

                    if (response.StatusCode == HttpStatusCode.NotFound)
                        return string.Empty;

                    var responseContentTask = response.Content.ReadAsStringAsync();
                    responseContentTask.Wait();
                    return responseContentTask.Result.Replace("\n", Environment.NewLine);
                }
            }


            



        }


        public abstract class Style
        {

            

            //public String[] AggregationPeriod(int a)
            //{
                    

            //}

            enum TimeInterval { oneDay, twoDays, threeDays,  };
            enum AggregatinPeriod { oneMin, twoMin, fiveMin, tenMin, fifteenMin, thirtyMin, oneHour,  }

            // string period, string interval
            private static Dictionary<String, List<String>> dictStyles = new Dictionary<string, List<string>>();

            /// <summary>
            /// Determine whether or not styles were already populated.
            /// </summary>
            /// <returns>True if styles were populated, false otherwise</returns>
            public static Boolean areStylesPopulated()
            {
                return (dictStyles.Count == 0) ? false : true;
            }

            /// <summary>
            /// Populate the styles for this kind of 
            /// </summary>
            public static void PopulateStyles()
            {
                List<String> aggregationPeriod = new List<string>();
                aggregationPeriod.Add("1 min");
                aggregationPeriod.Add("2 min");
                aggregationPeriod.Add("3 min");
                aggregationPeriod.Add("5 min");
                aggregationPeriod.Add("10 min");
                aggregationPeriod.Add("15 min");
                //intraday = 1 day = p=1d
                // 1 min = i=60
                dictStyles["intraday"] = aggregationPeriod;
                dictStyles["1 day"]    = aggregationPeriod;
                dictStyles["2 day"]    = aggregationPeriod;
                dictStyles["3 day"]    = aggregationPeriod;
                dictStyles["5 day"]    = aggregationPeriod;
                dictStyles["10 day"]   = aggregationPeriod;
                aggregationPeriod.Remove("1 min");
                aggregationPeriod.Remove("2 min");
                aggregationPeriod.Remove("3 min");
                aggregationPeriod.Remove("5 min");
                aggregationPeriod.Remove("10 min");
                aggregationPeriod.Remove("15 min");
                dictStyles["1 year"]   = aggregationPeriod;
                dictStyles["2 year"]   = aggregationPeriod;
                dictStyles["5 year"]   = aggregationPeriod;
                dictStyles["10 year"]  = aggregationPeriod;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="TimeInterval">The time range to view a stock</param>
            /// <param name="AggregationPeriod">Each bar represents a certain period of time</param>
            /// <returns>Returns two values within a tuple:
            /// Value 1: Time interval used by google.
            /// Value 2: aggregation period used by google.
            /// </returns>
            public static Tuple<String, String> getStyle(String TimeInterval, String AggregationPeriod)
            {
                //if (dictStyles.ContainsKey(TimeInterval))
                return new Tuple<string, string>("", "");
            }





        }



    }


}
