using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Globalization;
using System.IO;
using System.Linq;

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
