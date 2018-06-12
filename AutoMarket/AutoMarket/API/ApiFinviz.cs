using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Text;

namespace AutoMarket.API
{
    //Queries from www.finviz.com
    class ApiFinviz
    {
        private Http.RestCaller restCaller;
        /// <summary>
        /// Stores the performance map.
        /// </summary>
        Dictionary<String, Double> dicStockPerformance = null;
        List<String> lstEarningsRelease = new List<String>();


        public ApiFinviz()
        {
            restCaller = new Http.RestCaller();
        }

        public bool CaptureMainScreenData()
        {
            //test financial visualization  http://www.finviz.com

            API.Http.RestCaller http = new API.Http.RestCaller();

            String finvizdata = http.Get("http://www.finviz.com");

            #region Sector Map Performance (the cool red-green map by FinViz)

            //Strip easily the Sector Map Performance
            int iStart = finvizdata.LastIndexOf("var FinvizSecMapPerf") + 24;
            int iEnd = finvizdata.IndexOf("}", iStart);
            String SectorMapPerformance = finvizdata.Substring(iStart, iEnd - iStart).Replace("\"", "");
            String[] arrIndividualStockPerformance = SectorMapPerformance.Split(',');
            dicStockPerformance = new Dictionary<String, Double>();
            for (int i = 0; i < arrIndividualStockPerformance.GetLength(0); i++)
            {
                String[] KeyValuePair = arrIndividualStockPerformance[i].Split(':');
                dicStockPerformance.Add(KeyValuePair[0], System.Convert.ToDouble(KeyValuePair[1]));
            }

            #endregion

            #region Get the earnings release

            iStart = finvizdata.IndexOf("Earnings release");
            iEnd = finvizdata.IndexOf("</table>", iStart) - iStart;
            //iEnd = finvizdata.IndexOf("Top Insider Trading") - iStart;
            String earningsRelease = finvizdata.Substring(iStart, iEnd);

            bool isread = true;
            int iStartRead = 0;
            int iEndRead = 0;
            int iLastRead = 0;
            while (isread)
            {
                iStartRead = earningsRelease.IndexOf("quote.ashx?t=", iStartRead) + 13;

                if (iStartRead > iLastRead)
                    iLastRead = iStartRead;
                else
                    isread = false;


                if ((iStartRead < 0) || (isread == false))
                    break;
                else
                {
                    iEndRead = earningsRelease.IndexOf('\"', iStartRead);
                    lstEarningsRelease.Add(earningsRelease.Substring(iStartRead, iEndRead - iStartRead));
                }


            }

            #endregion

            #region Top Insider Trading

            Dictionary<String, List<String>> dicTopInsiderTrading = new Dictionary<String, List<String>>();

            iStart = finvizdata.IndexOf("Top Insider Trading");
            iEnd = finvizdata.IndexOf("</table>", iStart) - iStart;
            //iEnd = finvizdata.IndexOf("Top Insider Trading") - iStart;
            String topInsiderTrading = finvizdata.Substring(iStart, iEnd);
            
            isread = true;
            iStartRead = 0;
            iEndRead = 0;
            iLastRead = 0;
            while (isread)
            {
                iStartRead = topInsiderTrading.IndexOf("quote.ashx?t=", iStartRead) + 13;

                if (iStartRead > iLastRead)
                    iLastRead = iStartRead;
                else
                    isread = false;


                if ((iStartRead < 0) || (isread == false))
                    break;
                else
                {
                    iEndRead = topInsiderTrading.IndexOf('\"', iStartRead);
                    String stockSymbol = topInsiderTrading.Substring(iStartRead, iEndRead - iStartRead);
                    List<String> trading = new List<String>();
                    int iStartInsideRead = iStartRead;
                    int iEndInsideRead = iEndRead;

                    iStartInsideRead = topInsiderTrading.IndexOf("insidertrading.ashx?oc=", iEndInsideRead + 5) + 23;
                    iEndInsideRead = topInsiderTrading.IndexOf("&tc=", iStartInsideRead);
                    String companyNumber = topInsiderTrading.Substring(iStartInsideRead, iEndInsideRead - iStartInsideRead);

                    iStartInsideRead = topInsiderTrading.IndexOf("tab-link-nw", iStartRead) + 13;
                    iEndInsideRead = topInsiderTrading.IndexOf("</td>", iStartInsideRead);
                    String companyName = topInsiderTrading.Substring(iStartInsideRead, iEndInsideRead - iStartInsideRead);

                    iStartInsideRead = topInsiderTrading.IndexOf("<td>", iEndInsideRead + 4) + 4;
                    iEndInsideRead = topInsiderTrading.IndexOf("</td>", iStartInsideRead);
                    String Date = topInsiderTrading.Substring(iStartInsideRead, iEndInsideRead - iStartInsideRead);

                    iStartInsideRead = topInsiderTrading.IndexOf("center", iEndInsideRead) + 8;
                    iEndInsideRead = topInsiderTrading.IndexOf("</td>", iStartInsideRead);
                    String action = topInsiderTrading.Substring(iStartInsideRead, iEndInsideRead - iStartInsideRead);

                    iStartInsideRead = topInsiderTrading.IndexOf("right", iEndInsideRead) + 7;
                    iEndInsideRead = topInsiderTrading.IndexOf("</td>", iStartInsideRead);
                    String valueInUSD = topInsiderTrading.Substring(iStartInsideRead, iEndInsideRead - iStartInsideRead);

                    trading.Add(companyName);
                    trading.Add(companyNumber);
                    trading.Add(Date);
                    trading.Add(action);
                    trading.Add(valueInUSD);
                    if (dicTopInsiderTrading.ContainsKey(stockSymbol))
                        stockSymbol = stockSymbol + "_1";
                    dicTopInsiderTrading.Add(stockSymbol, trading);
                }


            }

            #endregion

            #region Major News

            Dictionary<String, String> dicMajorNews = new Dictionary<string, string>();

            String major_news = "major-news";

            iStart = finvizdata.IndexOf(major_news);
            iEnd = finvizdata.IndexOf("</table>", iStart) - iStart;
            String sTable = finvizdata.Substring(iStart, iEnd);
            String sCurrent = "";

            isread = true;
            iStartRead = 0;
            iEndRead = 0;
            iLastRead = 0;
            while (isread)
            {
                iStartRead = sTable.IndexOf("quote.ashx?t=", iStartRead) + 13;

                if (iStartRead > iLastRead)
                    iLastRead = iStartRead;
                else
                    isread = false;


                if ((iStartRead < 0) || (isread == false))
                    break;
                else
                {
                    iEndRead = sTable.IndexOf('\"', iStartRead);
                    int iPercentStart = sTable.IndexOf(">", sTable.IndexOf("color:#", iStartRead)) + 1;
                    int iPercentStop  = sTable.IndexOf("</span", iStartRead);
                    string sPercent   = sTable.Substring(iPercentStart, iPercentStop - iPercentStart);
                    dicMajorNews.Add(sTable.Substring(iStartRead, iEndRead - iStartRead), sPercent);
                }


            }


            #endregion

            #region Top Gainers, new high, overbought, unusual volume, upgrades, earnings before, indider buying

            List<String> lstMajorGainers = new List<string>();
            List<String> lstNewHigh = new List<string>();
            List<String> lstOverbought = new List<string>();
            List<String> lstUnusualVolume = new List<string>();
            List<String> lstUpgrades = new List<string>();
            List<String> lstEarningsBefore = new List<string>();
            List<String> lstInsiderBuying = new List<string>();
            String topgainers = "ta_topgainers";

            iStart = finvizdata.IndexOf(topgainers);
            iEnd = finvizdata.IndexOf("</table>", iStart) - iStart;
            sTable = finvizdata.Substring(iStart, iEnd);

            isread = true;
            iStartRead = 0;
            iEndRead = 0;
            iLastRead = 0;
            while (isread)
            {
                iStartRead = sTable.IndexOf("quote.ashx?t=", iStartRead) + 13;

                if (iStartRead > iLastRead)
                    iLastRead = iStartRead;
                else
                    isread = false;


                if ((iStartRead < 0) || (isread == false))
                    break;
                else
                {
                    iEndRead = sTable.IndexOf('\"', iStartRead);
                    if (sTable.IndexOf("ta_topgainers", iStartRead) > iLastRead)
                        lstMajorGainers.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_topgainers", iStartRead) < iLastRead) && (sTable.IndexOf("ta_newhigh", iStartRead) > iLastRead))
                        lstNewHigh.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_newhigh", iStartRead) < iLastRead) && (sTable.IndexOf("ta_overbought", iStartRead) > iLastRead))
                        lstOverbought.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_overbought", iStartRead) < iLastRead) && (sTable.IndexOf("ta_unusualvolume", iStartRead) > iLastRead))
                        lstUnusualVolume.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_unusualvolume", iStartRead) < iLastRead) && (sTable.IndexOf("n_upgrades", iStartRead) > iLastRead))
                        lstUpgrades.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("n_upgrades", iStartRead) < iLastRead) && (sTable.IndexOf("n_earningsbefore", iStartRead) > iLastRead))
                        lstEarningsBefore.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("n_earningsbefore", iStartRead) < iLastRead) && (sTable.IndexOf("it_latestbuys", iStartRead) > iLastRead))
                        lstInsiderBuying.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                }
            }


            #endregion

            #region Top Losers, new low, oversold, most volatile, most active, downgrades, earnings after, insider selling

            List<String> lstMajorLosers = new List<string>();
            List<String> lstNewLow = new List<string>();
            List<String> lstOversold = new List<string>();
            List<String> lstMostVolatile = new List<string>();
            List<String> lstMostActive = new List<string>();
            List<String> lstDowngrades = new List<string>();
            List<String> lstEarningsAfter = new List<string>();
            List<String> lstInsiderSelling = new List<string>();
            String toplosers = "ta_toplosers";

            iStart = finvizdata.IndexOf(toplosers);
            iEnd = finvizdata.IndexOf("</table>", iStart) - iStart;
            sTable = finvizdata.Substring(iStart, iEnd);

            isread = true;
            iStartRead = 0;
            iEndRead = 0;
            iLastRead = 0;
            while (isread)
            {
                iStartRead = sTable.IndexOf("quote.ashx?t=", iStartRead) + 13;

                if (iStartRead > iLastRead)
                    iLastRead = iStartRead;
                else
                    isread = false;


                if ((iStartRead < 0) || (isread == false))
                    break;
                else
                {
                    iEndRead = sTable.IndexOf('\"', iStartRead);
                    if (sTable.IndexOf("ta_toplosers", iStartRead) > iLastRead)
                        lstMajorLosers.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_toplosers", iStartRead) < iLastRead) && (sTable.IndexOf("ta_newlow", iStartRead) > iLastRead))
                        lstNewLow.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_newlow", iStartRead) < iLastRead) && (sTable.IndexOf("ta_oversold", iStartRead) > iLastRead))
                        lstOversold.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_oversold", iStartRead) < iLastRead) && (sTable.IndexOf("ta_mostvolatile", iStartRead) > iLastRead))
                        lstMostVolatile.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_mostvolatile", iStartRead) < iLastRead) && (sTable.IndexOf("ta_mostactive", iStartRead) > iLastRead))
                        lstMostActive.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_mostactive", iStartRead) < iLastRead) && (sTable.IndexOf("n_downgrades", iStartRead) > iLastRead))
                        lstDowngrades.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("n_downgrades", iStartRead) < iLastRead) && (sTable.IndexOf("n_earningsafter", iStartRead) > iLastRead))
                        lstEarningsAfter.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("n_earningsafter", iStartRead) < iLastRead) && (sTable.IndexOf("it_latestsales", iStartRead) > iLastRead))
                        lstInsiderSelling.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                }
            }

            #endregion

            #region  Signal Tickers "ta_p_tlsupport" and "ta_p_channelup"

            /* add here everything so we can include in a dictionary */
            //dictionary <string, list<string> // first element: "channel up" or silimar, //second element: list of stocks.


            #endregion

            #region News

            //list of news. find out how it needs to be done

            #endregion


            #region Insider trading

            //here all the last insider trading per stock. eg. when Lululemon will sell almost 30 million shares by owner on june 7

            #endregion



            return true;

        }



    }
}
