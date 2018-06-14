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
                    if (dicTopInsiderTrading.ContainsKey(stockSymbol))
                        stockSymbol = stockSymbol.Replace("_1", "_2");
                    if (dicTopInsiderTrading.ContainsKey(stockSymbol))
                        stockSymbol = stockSymbol.Replace("_2", "_3");
                    if (dicTopInsiderTrading.ContainsKey(stockSymbol))
                        stockSymbol = stockSymbol.Replace("_3", "_4");
                    if (dicTopInsiderTrading.ContainsKey(stockSymbol))
                        stockSymbol = stockSymbol.Replace("_4", "_5");
                    if (dicTopInsiderTrading.ContainsKey(stockSymbol))
                        stockSymbol = stockSymbol.Replace("_5", "_6");


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
            Dictionary<String, List<String>> dicSignalSupport = new Dictionary<String, List<String>>();
            List<String> lstTrendlineSupport = new List<string>();
            List<String> lstTrendlineResistance = new List<string>();
            List<String> lstHorizontalSR = new List<string>();
            List<String> lstWedgeUp = new List<string>();
            List<String> lstWedge = new List<string>();
            List<String> lstWedgeDown = new List<string>();
            List<String> lstTriangelAscending = new List<string>();
            List<String> lstTriangelDescending = new List<string>();
            List<String> lstChannelUp = new List<string>();
            List<String> lstChannel = new List<string>();
            List<String> lstChannelDown = new List<string>();
            List<String> lstDoubleTop = new List<string>();
            List<String> lstMultipleTop = new List<string>();
            List<String> lstDoubleBottom = new List<string>();
            List<String> lstMultipleBottom = new List<string>();
            List<String> lstHeadAndShoulders = new List<string>();

            String tableTag = "ta_p_tlsupport";

            iStart = finvizdata.IndexOf(tableTag);
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
                    if (sTable.IndexOf("ta_p_tlsupport", iStartRead) > iLastRead)
                        lstTrendlineSupport.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_p_tlsupport", iStartRead) < iLastRead) && (sTable.IndexOf("ta_p_tlresistance", iStartRead) > iLastRead))
                        lstTrendlineResistance.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_p_tlresistance", iStartRead) < iLastRead) && (sTable.IndexOf("ta_p_horizontal", iStartRead) > iLastRead))
                        lstHorizontalSR.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_p_horizontal", iStartRead) < iLastRead) && (sTable.IndexOf("ta_p_wedgeup", iStartRead) > iLastRead))
                        lstWedgeUp.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_p_wedgeup", iStartRead) < iLastRead) && (sTable.IndexOf("ta_p_wedge\"", iStartRead) > iLastRead))
                        lstWedge.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_p_wedge\"", iStartRead) < iLastRead) && (sTable.IndexOf("ta_p_wedgedown", iStartRead) > iLastRead))
                        lstWedgeDown.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_p_wedgedown", iStartRead) < iLastRead) && (sTable.IndexOf("ta_p_wedgeresistance", iStartRead) > iLastRead))
                        lstTriangelAscending.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_p_wedgeresistance", iStartRead) < iLastRead) && (sTable.IndexOf("ta_p_wedgesupport", iStartRead) > iLastRead))
                        lstTriangelDescending.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                }
            }
            

            tableTag = "ta_p_channelup";

            iStart = finvizdata.IndexOf(tableTag);
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
                    if (sTable.IndexOf("ta_p_channelup", iStartRead) > iLastRead)
                        lstChannelUp.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_p_channelup", iStartRead) < iLastRead) && (sTable.IndexOf("ta_p_channel\"", iStartRead) > iLastRead))
                        lstChannel.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_p_channel\"", iStartRead) < iLastRead) && (sTable.IndexOf("ta_p_channeldown", iStartRead) > iLastRead))
                        lstChannelDown.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_p_channeldown", iStartRead) < iLastRead) && (sTable.IndexOf("ta_p_doubletop", iStartRead) > iLastRead))
                        lstDoubleTop.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_p_doubletop", iStartRead) < iLastRead) && (sTable.IndexOf("ta_p_multipletop", iStartRead) > iLastRead))
                        lstMultipleTop.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_p_multipletop", iStartRead) < iLastRead) && (sTable.IndexOf("ta_p_doublebottom", iStartRead) > iLastRead))
                        lstDoubleBottom.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_p_doublebottom", iStartRead) < iLastRead) && (sTable.IndexOf("ta_p_multiplebottom", iStartRead) > iLastRead))
                        lstMultipleBottom.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                    if ((sTable.IndexOf("ta_p_multiplebottom", iStartRead) < iLastRead) && (sTable.IndexOf("ta_p_headandshoulders", iStartRead) > iLastRead))
                        lstHeadAndShoulders.Add(sTable.Substring(iStartRead, iEndRead - iStartRead));
                }
            }
            
            // Add all lists to the dictionary
            dicSignalSupport.Add("Trendline Support", lstTrendlineSupport);
            dicSignalSupport.Add("Trendline Resistance", lstTrendlineResistance);
            dicSignalSupport.Add("Horizontal SR", lstHorizontalSR);
            dicSignalSupport.Add("Wedge Up", lstWedgeUp);
            dicSignalSupport.Add("Wedge", lstWedge);
            dicSignalSupport.Add("Wedge Down", lstWedgeDown);
            dicSignalSupport.Add("Triangle Ascending", lstTriangelAscending);
            dicSignalSupport.Add("Triangle Descending", lstTriangelDescending);
            dicSignalSupport.Add("Channel Up", lstChannelUp);
            dicSignalSupport.Add("Channel", lstChannel);
            dicSignalSupport.Add("Channel Down", lstChannelDown);
            dicSignalSupport.Add("Double Top", lstDoubleTop);
            dicSignalSupport.Add("Multiple Top", lstMultipleTop);
            dicSignalSupport.Add("Double Bottom", lstDoubleBottom);
            dicSignalSupport.Add("Multiple Bottom", lstMultipleBottom);
            dicSignalSupport.Add("Head and Shoulders", lstHeadAndShoulders);
            
            // Remove all the lists to save some memory
            lstTrendlineSupport = null;
            lstTrendlineResistance = null;
            lstHorizontalSR = null;
            lstWedgeUp = null;
            lstWedge = null;
            lstWedgeDown = null;
            lstTriangelAscending = null;
            lstTriangelDescending = null;
            lstChannelUp = null;
            lstChannel = null;
            lstChannelDown = null;
            lstDoubleTop = null;
            lstMultipleTop = null;
            lstDoubleBottom = null;
            lstMultipleBottom = null;
            lstHeadAndShoulders = null;

            #endregion

            #region News

            ////list of news. find out how it needs to be done
            Dictionary<String, List<String>> dicTodaysNews = new Dictionary<String, List<String>>();

            major_news = "nn-home-first";

            iStart = finvizdata.IndexOf(major_news);
            iEnd = finvizdata.IndexOf("t-home-table", iStart) - iStart;
            sTable = finvizdata.Substring(iStart, iEnd);

            isread = true;
            iStartRead = 0;
            iEndRead = 0;
            iLastRead = 0;
            while (isread)
            {
                iStartRead = sTable.IndexOf("nn-date", iStartRead) + 24;

                if (iStartRead > iLastRead)
                    iLastRead = iStartRead;
                else
                    isread = false;
                
                if ((iStartRead < 0) || (isread == false))
                    break;
                else
                {
                    List<String> sNewsFeed = new List<String>();
                    //iEndRead = sTable.IndexOf("PM", iStartRead);
                    iEndRead = sTable.IndexOf("</td>", iStartRead);
                    //int iNewsTimeStart = sTable.IndexOf("PM", iEndRead - 8) - 5;
                    int iNewsTimeStart = sTable.IndexOf("</td>", iEndRead - 9) - 7;
                    int iNewsTimeEnd = sTable.IndexOf("</td>", iStartRead);
                    int iSummaryStart = sTable.IndexOf("tooltip_tab", iNewsTimeEnd) + 13;
                    int iSummaryEnd = sTable.IndexOf("</td>", iSummaryStart);
                    int iLinkStart = sTable.IndexOf("<a href=", iSummaryEnd) + 9;
                    int iLinkEnd = sTable.IndexOf("\"", iLinkStart + 7);
                    int iNewsTitleStart = sTable.IndexOf("nn-tab-link", iLinkEnd) + 13;
                    int iNewsTitleEnd = sTable.IndexOf("</a>", iNewsTitleStart);
                    String NewsTime = sTable.Substring(iNewsTimeStart, iNewsTimeEnd - iNewsTimeStart);
                    String Summary = sTable.Substring(iSummaryStart, iSummaryEnd - iSummaryStart);
                    String Link = sTable.Substring(iLinkStart, iLinkEnd - iLinkStart);
                    String Title = sTable.Substring(iNewsTitleStart, iNewsTitleEnd - iNewsTitleStart);
                    sNewsFeed.Add(NewsTime);
                    sNewsFeed.Add(Link);
                    sNewsFeed.Add(Title);
                    sNewsFeed.Add(Summary);
                    if (!dicTodaysNews.ContainsKey(NewsTime))    //<<don't care if we miss one news, we can get it later on.
                        dicTodaysNews.Add(NewsTime, sNewsFeed);
                }
            }

            #endregion

            #region Important current Economical Calendar

            Dictionary<String, List<String>> dicCalendarofEvents = new Dictionary<String, List<String>>();

            major_news = "calendar";

            iStart = finvizdata.IndexOf(major_news);
            iEnd = finvizdata.IndexOf("</table>", iStart) - iStart;
            sTable = finvizdata.Substring(iStart, iEnd);

            isread = true;
            iStartRead = 0;
            iEndRead = 0;
            iLastRead = 0;
            while (isread)
            {
                iStartRead = sTable.IndexOf("table-light-cp-row", iStartRead) + 24;

                if (iStartRead > iLastRead)
                    iLastRead = iStartRead;
                else
                    isread = false;

                if ((iStartRead < 0) || (isread == false))
                    break;
                else
                {
                    List<String> sNewsFeed = new List<String>();
                    iEndRead = sTable.IndexOf("right", iStartRead);
                    int iDateStart = sTable.IndexOf("right", iEndRead) + 7;
                    int iDateEnd = sTable.IndexOf("</td>", iStartRead);
                    int iTimeStart = sTable.IndexOf("right", iDateEnd) + 7;
                    int iTimeEnd = sTable.IndexOf("</td>", iTimeStart);
                    int iReleaseStart = sTable.IndexOf("left", iTimeEnd) + 6;
                    int iReleaseEnd = sTable.IndexOf("</td>", iReleaseStart);
                    int iImpactStart = sTable.IndexOf("impact_", iReleaseEnd) + 7;
                    int iImpactEnd = sTable.IndexOf(".", iImpactStart);
                    int iForStart = sTable.IndexOf("left", iImpactEnd) + 6;
                    int iForEnd = sTable.IndexOf("</td>", iForStart);
                    int iActualStart = sTable.IndexOf("right", iForEnd) + 7;
                    int iActualEnd = sTable.IndexOf("</td>", iActualStart);
                    if (sTable.Substring(sTable.IndexOf("right", iForEnd) + 7, 12) == "<span style=")
                    {
                        iActualStart = sTable.IndexOf("<span style", iForEnd) + 29;
                        iActualEnd = sTable.IndexOf("</span>", iActualStart);
                    }
                    int iExpectedStart = sTable.IndexOf("right", iActualEnd) + 7;
                    int iExpectedEnd = sTable.IndexOf("</td>", iExpectedStart);
                    int iPriorStart = sTable.IndexOf("right", iExpectedEnd) + 7;
                    int iPriorEnd = sTable.IndexOf("</td>", iPriorStart);
                    String Date     = sTable.Substring(iDateStart, iDateEnd - iDateStart);
                    String Time     = sTable.Substring(iTimeStart, iTimeEnd - iTimeStart);
                    String Release  = sTable.Substring(iReleaseStart, iReleaseEnd - iReleaseStart);
                    String Impact   = sTable.Substring(iImpactStart, iImpactEnd - iImpactStart);
                    switch( Impact ) {
                        case "1": Impact = "Low"   ; break;
                        case "2": Impact = "Medium"; break;
                        case "3": Impact = "High"  ; break; }
                    String For      = sTable.Substring(iForStart, iForEnd - iForStart);
                    String Actual   = sTable.Substring(iActualStart, iActualEnd - iActualStart);
                    String Expected = sTable.Substring(iExpectedStart, iExpectedEnd - iExpectedStart);
                    String Prior    = sTable.Substring(iPriorStart, iPriorEnd - iPriorStart);
                    iStartRead = iPriorEnd;
                    sNewsFeed.Add(Date);
                    sNewsFeed.Add(Time);
                    sNewsFeed.Add(Release);
                    sNewsFeed.Add(Impact);
                    sNewsFeed.Add(For);
                    sNewsFeed.Add(Actual);
                    sNewsFeed.Add(Expected);
                    sNewsFeed.Add(Prior);
                    dicCalendarofEvents.Add(Release, sNewsFeed);
                }
            }

            #endregion

            #region insider trading

            //here all the last insider trading per stock. eg. when Lululemon will sell almost 30 million shares by owner on june 7
            Dictionary<int, List<String>> dicInsiderTrading = new Dictionary<int, List<string>>();
            int iSequentialNum = 0;
            
            major_news = "Latest Insider Trading";

            iStart = finvizdata.IndexOf(major_news);
            iEnd = finvizdata.IndexOf("</table>", iStart) - iStart;
            sTable = finvizdata.Substring(iStart, iEnd);

            isread = true;
            iStartRead = 0;
            iEndRead = 0;
            iLastRead = 0;
            while (isread)
            {
                iSequentialNum++;

                iStartRead = sTable.IndexOf("insider-light-row-cp-h", iStartRead) + 24;

                if (iStartRead > iLastRead)
                    iLastRead = iStartRead;
                else
                    isread = false;

                if ((iStartRead < 0) || (isread == false))
                    break;
                else
                {
                    List<String> lstInsiderTrading = new List<String>();
                    iEndRead = sTable.IndexOf("tab-link", iStartRead);
                    int iTickerStart = sTable.IndexOf("tab-link", iEndRead) + 10;
                    int iTickerEnd = sTable.IndexOf("</a>", iStartRead);
                    int iTraderStart = sTable.IndexOf("tab-link-nw", iTickerEnd) + 13;
                    int iTraderEnd = sTable.IndexOf("</td>", iTraderStart);
                    int iRelationshipStart = sTable.IndexOf("nowrap", iTraderEnd) + 8;
                    int iRelationshipEnd = sTable.IndexOf("</td>", iRelationshipStart);
                    int iDateStart = sTable.IndexOf("nowrap", iRelationshipEnd) + 8;
                    int iDateEnd = sTable.IndexOf("</td>", iDateStart);
                    int iTransactionStart = sTable.IndexOf("center", iDateEnd) + 8;
                    int iTransactionEnd = sTable.IndexOf("</td>", iTransactionStart);
                    int iCostStart = sTable.IndexOf("right", iTransactionEnd) + 7;
                    int iCostEnd = sTable.IndexOf("</td>", iCostStart);
                    int iNumSharesStart = sTable.IndexOf("right", iCostEnd) + 7;
                    int iNumSharesEnd = sTable.IndexOf("</td>", iNumSharesStart);
                    int iValueStart = sTable.IndexOf("right", iNumSharesEnd) + 7;
                    int iValueEnd = sTable.IndexOf("</td>", iValueStart);
                    String Ticker = sTable.Substring(iTickerStart, iTickerEnd - iTickerStart);
                    String Trader = sTable.Substring(iTraderStart, iTraderEnd - iTraderStart);
                    String Relationship = sTable.Substring(iRelationshipStart, iRelationshipEnd - iRelationshipStart);
                    String Date = sTable.Substring(iDateStart, iDateEnd - iDateStart);
                    String Transaction = sTable.Substring(iTransactionStart, iTransactionEnd - iTransactionStart);
                    String Cost = sTable.Substring(iCostStart, iCostEnd - iCostStart);
                    String NumofShares = sTable.Substring(iNumSharesStart, iNumSharesEnd - iNumSharesStart);
                    String Value = sTable.Substring(iValueStart, iValueEnd - iValueStart);
                    //iStartRead = iValueEnd;
                    lstInsiderTrading.Add(Ticker);
                    lstInsiderTrading.Add(Trader);
                    lstInsiderTrading.Add(Relationship);
                    lstInsiderTrading.Add(Date);
                    lstInsiderTrading.Add(Transaction);
                    lstInsiderTrading.Add(Cost);
                    lstInsiderTrading.Add(NumofShares);
                    lstInsiderTrading.Add(Value);
                    
                    dicInsiderTrading.Add(iSequentialNum, lstInsiderTrading);
                }
            }

            #endregion


            //String futures = http.Get("http://www.finviz.com/futures.ashx");


            #region Futures JSON String

            /*

            // For Groups

            public class Contract
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public string cot { get; set; }
            }

            public class RootObject
            {
                public string ticker { get; set; }
                public string label { get; set; }
                public List<Contract> contracts { get; set; }
            }


            
            
            
            // For Tiles




            public class SparklineDateChanges
            {
            }

            public class __invalid_type__6A
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges2
            {
            }

            public class __invalid_type__6B
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges2 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges3
            {
            }

            public class __invalid_type__6C
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges3 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges4
            {
            }

            public class __invalid_type__6E
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges4 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges5
            {
            }

            public class __invalid_type__6J
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges5 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges6
            {
            }

            public class __invalid_type__6N
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges6 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges7
            {
            }

            public class __invalid_type__6S
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges7 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges8
            {
            }

            public class CC
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges8 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges9
            {
            }

            public class CL
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges9 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges10
            {
            }

            public class CT
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges10 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges11
            {
            }

            public class DX
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges11 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges12
            {
            }

            public class DY
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges12 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges13
            {
            }

            public class ER2
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges13 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges14
            {
            }

            public class ES
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges14 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges15
            {
            }

            public class EX
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges15 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges16
            {
            }

            public class FC
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges16 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges17
            {
            }

            public class GC
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges17 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges18
            {
            }

            public class HG
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges18 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges19
            {
            }

            public class HO
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges19 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges20
            {
            }

            public class JO
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges20 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges21
            {
            }

            public class KC
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges21 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges22
            {
            }

            public class LB
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges22 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges23
            {
            }

            public class LC
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges23 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges24
            {
            }

            public class LH
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges24 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges25
            {
            }

            public class NG
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges25 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges26
            {
            }

            public class NKD
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges26 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges27
            {
            }

            public class NQ
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges27 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges28
            {
            }

            public class PA
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges28 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges29
            {
            }

            public class PL
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges29 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges30
            {
            }

            public class QA
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges30 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges31
            {
            }

            public class RB
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges31 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges32
            {
            }

            public class RS
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges32 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges33
            {
            }

            public class SB
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges33 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges34
            {
            }

            public class SI
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges34 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges35
            {
            }

            public class VX
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges35 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges36
            {
            }

            public class YM
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges36 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges37
            {
            }

            public class ZB
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges37 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges38
            {
            }

            public class ZC
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges38 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges39
            {
            }

            public class ZF
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges39 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges40
            {
            }

            public class ZK
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges40 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges41
            {
            }

            public class ZL
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges41 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges42
            {
            }

            public class ZM
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges42 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges43
            {
            }

            public class ZN
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges43 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges44
            {
            }

            public class ZO
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges44 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges45
            {
            }

            public class ZR
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges45 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges46
            {
            }

            public class ZS
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges46 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges47
            {
            }

            public class ZT
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges47 sparklineDateChanges { get; set; }
            }

            public class SparklineDateChanges48
            {
            }

            public class ZW
            {
                public string label { get; set; }
                public string ticker { get; set; }
                public double last { get; set; }
                public double change { get; set; }
                public double prevClose { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public List<object> sparkline { get; set; }
                public SparklineDateChanges48 sparklineDateChanges { get; set; }
            }

            public class RootObject
            {
                public __invalid_type__6A __invalid_name__6A { get; set; }
                public __invalid_type__6B __invalid_name__6B { get; set; }
                public __invalid_type__6C __invalid_name__6C { get; set; }
                public __invalid_type__6E __invalid_name__6E { get; set; }
                public __invalid_type__6J __invalid_name__6J { get; set; }
                public __invalid_type__6N __invalid_name__6N { get; set; }
                public __invalid_type__6S __invalid_name__6S { get; set; }
                public CC CC { get; set; }
                public CL CL { get; set; }
                public CT CT { get; set; }
                public DX DX { get; set; }
                public DY DY { get; set; }
                public ER2 ER2 { get; set; }
                public ES ES { get; set; }
                public EX EX { get; set; }
                public FC FC { get; set; }
                public GC GC { get; set; }
                public HG HG { get; set; }
                public HO HO { get; set; }
                public JO JO { get; set; }
                public KC KC { get; set; }
                public LB LB { get; set; }
                public LC LC { get; set; }
                public LH LH { get; set; }
                public NG NG { get; set; }
                public NKD NKD { get; set; }
                public NQ NQ { get; set; }
                public PA PA { get; set; }
                public PL PL { get; set; }
                public QA QA { get; set; }
                public RB RB { get; set; }
                public RS RS { get; set; }
                public SB SB { get; set; }
                public SI SI { get; set; }
                public VX VX { get; set; }
                public YM YM { get; set; }
                public ZB ZB { get; set; }
                public ZC ZC { get; set; }
                public ZF ZF { get; set; }
                public ZK ZK { get; set; }
                public ZL ZL { get; set; }
                public ZM ZM { get; set; }
                public ZN ZN { get; set; }
                public ZO ZO { get; set; }
                public ZR ZR { get; set; }
                public ZS ZS { get; set; }
                public ZT ZT { get; set; }
                public ZW ZW { get; set; }
            }



            */

            /*

            string groups = 
            [
                { "ticker":"INDICES","label":"Indices","contracts":
                    [
                        {"label":"DJIA","ticker":"YM","cot":"124601,124603"},
                        {"label":"S&P 500","ticker":"ES","cot":"138741,13874A"},
                        {"label":"Nasdaq 100","ticker":"NQ","cot":"209741,209742"},
                        {"label":"Russell 2000","ticker":"ER2","cot":"23977A"},
                        {"label":"Nikkei 225","ticker":"NKD","cot":"240741"},
                        {"label":"Euro Stoxx 50","ticker":"EX","cot":""},
                        {"label":"DAX","ticker":"DY","cot":""},
                        {"label":"VIX","ticker":"VX","cot":"1170E1"}
                    ]
                },
                {"ticker":"ENERGY","label":"Energy","contracts":
                    [
                        {"label":"Crude Oil","ticker":"CL","cot":"067651"},
                        {"label":"Crude Oil Brent","ticker":"QA","cot":""},
                        {"label":"Gasoline RBOB","ticker":"RB","cot":"111659"},
                        {"label":"Heating Oil","ticker":"HO","cot":"022651"},
                        {"label":"Natural Gas","ticker":"NG","cot":"023651"},
                        {"label":"Ethanol","ticker":"ZK","cot":"025601"}
                    ]
                },
                {"ticker":"METALS","label":"Metals","contracts":
                    [
                        {"label":"Gold","ticker":"GC","cot":"088691"},
                        {"label":"Silver","ticker":"SI","cot":"084691"},
                        {"label":"Platinum","ticker":"PL","cot":"076651"},
                        {"label":"Copper","ticker":"HG","cot":"085692"},
                        {"label":"Palladium","ticker":"PA","cot":"075651"}
                    ]
                },
                {"ticker":"MEATS","label":"Meats","contracts":
                    [
                        {"label":"Live Cattle","ticker":"LC","cot":"057642"},
                        {"label":"Feeder Cattle","ticker":"FC","cot":"061641"},
                        {"label":"Lean Hogs","ticker":"LH","cot":"054642"}
                    ]
                },
                {"ticker":"GRAINS","label":"Grains","contracts":
                    [
                        {"label":"Corn","ticker":"ZC","cot":"002602"},
                        {"label":"Soybean Oil","ticker":"ZL","cot":"007601"},
                        {"label":"Soybean Meal","ticker":"ZM","cot":"026603"},
                        {"label":"Oats","ticker":"ZO","cot":"004603"},
                        {"label":"Rough Rice","ticker":"ZR","cot":"039601"},
                        {"label":"Soybeans","ticker":"ZS","cot":"005602"},
                        {"label":"Wheat","ticker":"ZW","cot":"001602"},
                        {"label":"Canola","ticker":"RS","cot":""}
                    ]
                    },
                {"ticker":"SOFTS","label":"Softs","contracts":
                    [
                        {"label":"Cocoa","ticker":"CC","cot":"073732"},
                        {"label":"Cotton","ticker":"CT","cot":"033661"},
                        {"label":"Orange Juice","ticker":"JO","cot":"040701"},
                        {"label":"Coffee","ticker":"KC","cot":"083731"},
                        {"label":"Lumber","ticker":"LB","cot":"058643"},
                        {"label":"Sugar","ticker":"SB","cot":"080732"}
                    ]
                },
                {"ticker":"BONDS","label":"Bonds","contracts":
                    [
                        {"label":"30 Year Bond","ticker":"ZB","cot":"020601"},
                        {"label":"10 Year Note","ticker":"ZN","cot":"043602"},
                        {"label":"5 Year Note","ticker":"ZF","cot":"044601"},
                        {"label":"2 Year Note","ticker":"ZT","cot":"042601"}
                    ]
                },
                {"ticker":"CURRENCIES","label":"Currencies","contracts":
                    [
                        {"label":"USD","ticker":"DX","cot":"098662"},
                        {"label":"EUR","ticker":"6E","cot":"099741"},
                        {"label":"JPY","ticker":"6J","cot":"097741"},
                        {"label":"GBP","ticker":"6B","cot":"096742"},
                        {"label":"CAD","ticker":"6C","cot":"090741"},
                        {"label":"CHF","ticker":"6S","cot":"092741"},
                        {"label":"AUD","ticker":"6A","cot":"232741"},
                        {"label":"NZD","ticker":"6N","cot":"112741"}
                    ]
                }
            ];
            string tiles = 
            { "6A":
            {   "label":"AUD",
                "ticker":"6A",
                "last":0.7564,
                "change":0.08,
                "prevClose":0.7558,
                "high":0.7584,
                "low":0.7553,
                "sparkline":[],
                "sparklineDateChanges":{}
            },
                "6B":
                {"label":"GBP",
                "ticker":"6B",
                "last":1.3392,
                "change":0.24,
                "prevClose":1.336,
                "high":1.3392,
                "low":1.3372,
                "sparkline":[],
                "sparklineDateChanges":{}
            },
                "6C":
            {
                "label":"CAD",
                "ticker":"6C","last":0.7706,"change":0.17,"prevClose":0.7693,"high":0.77105,"low":0.77,"sparkline":[],"sparklineDateChanges":{}},"6E":{"label":"EUR","ticker":"6E","last":1.1806,"change":0.24,"prevClose":1.17775,"high":1.18075,"low":1.17915,"sparkline":[],"sparklineDateChanges":{}},"6J":{"label":"JPY","ticker":"6J","last":0.90755000000000008,"change":0.29,"prevClose":0.90495,"high":0.9085,"low":0.9063,"sparkline":[],"sparklineDateChanges":{}},"6N":{"label":"NZD","ticker":"6N","last":0.703,"change":0.31,"prevClose":0.7008,"high":0.7031,"low":0.7016,"sparkline":[],"sparklineDateChanges":{}},"6S":{"label":"CHF","ticker":"6S","last":1.0158,"change":0.16,"prevClose":1.0142,"high":1.0163,"low":1.0147,"sparkline":[],"sparklineDateChanges":{}},"CC":{"label":"Cocoa","ticker":"CC","last":2391.0,"change":-2.17,"prevClose":2444.0,"high":2475.0,"low":2380.0,"sparkline":[],"sparklineDateChanges":{}},"CL":{"label":"Crude Oil WTI","ticker":"CL","last":66.61,"change":-0.05,"prevClose":66.64,"high":66.78,"low":66.53,"sparkline":[],"sparklineDateChanges":{}},"CT":{"label":"Cotton","ticker":"CT","last":92.06,"change":-0.94,"prevClose":92.93,"high":92.9,"low":91.64,"sparkline":[],"sparklineDateChanges":{}},"DX":{"label":"USD","ticker":"DX","last":93.13,"change":-0.25,"prevClose":93.359,"high":93.24,"low":93.13,"sparkline":[],"sparklineDateChanges":{}},"DY":{"label":"DAX","ticker":"DY","last":12863.0,"change":0.17,"prevClose":12841.0,"high":12907.5,"low":12777.0,"sparkline":[],"sparklineDateChanges":{}},"ER2":{"label":"Russell 2000","ticker":"ER2","last":1679.8,"change":-0.05,"prevClose":1680.7,"high":1682.7,"low":1679.4,"sparkline":[],"sparklineDateChanges":{}},"ES":{"label":"S&P 500","ticker":"ES","last":2779.25,"change":0.01,"prevClose":2779.0,"high":2783.5,"low":2777.75,"sparkline":[],"sparklineDateChanges":{}},"EX":{"label":"Euro Stoxx 50","ticker":"EX","last":3473.0,"change":0.0,"prevClose":3473.0,"high":3488.0,"low":3464.0,"sparkline":[],"sparklineDateChanges":{}},"FC":{"label":"Feeder Cattle","ticker":"FC","last":145.625,"change":-0.12,"prevClose":145.8,"high":146.4,"low":145.1,"sparkline":[],"sparklineDateChanges":{}},"GC":{"label":"Gold","ticker":"GC","last":1302.5,"change":0.09,"prevClose":1301.3,"high":1304.7,"low":1301.0,"sparkline":[],"sparklineDateChanges":{}},"HG":{"label":"Copper","ticker":"HG","last":3.241,"change":-0.4,"prevClose":3.254,"high":3.256,"low":3.2375,"sparkline":[],"sparklineDateChanges":{}},"HO":{"label":"Heating Oil","ticker":"HO","last":2.1804,"change":-0.22,"prevClose":2.1851,"high":2.1844,"low":2.1769,"sparkline":[],"sparklineDateChanges":{}},"JO":{"label":"Orange Juice","ticker":"JO","last":155.55,"change":0.81,"prevClose":154.3,"high":155.55,"low":153.15,"sparkline":[],"sparklineDateChanges":{}},"KC":{"label":"Coffee","ticker":"KC","last":118.5,"change":-0.84,"prevClose":119.5,"high":119.35,"low":118.45,"sparkline":[],"sparklineDateChanges":{}},"LB":{"label":"Lumber","ticker":"LB","last":554.6,"change":-2.63,"prevClose":569.6,"high":562.8,"low":554.6,"sparkline":[],"sparklineDateChanges":{}},"LC":{"label":"Live Cattle","ticker":"LC","last":103.85,"change":-0.43,"prevClose":104.3,"high":104.725,"low":103.575,"sparkline":[],"sparklineDateChanges":{}},"LH":{"label":"Lean Hogs","ticker":"LH","last":82.8,"change":1.66,"prevClose":81.45,"high":83.3,"low":81.75,"sparkline":[],"sparklineDateChanges":{}},"NG":{"label":"Natural Gas","ticker":"NG","last":2.958,"change":-0.17,"prevClose":2.963,"high":2.962,"low":2.954,"sparkline":[],"sparklineDateChanges":{}},"NKD":{"label":"Nikkei 225","ticker":"NKD","last":22820.0,"change":0.0,"prevClose":22820.0,"high":22880.0,"low":22760.0,"sparkline":[],"sparklineDateChanges":{}},"NQ":{"label":"Nasdaq 100","ticker":"NQ","last":7227.25,"change":-0.05,"prevClose":7231.0,"high":7242.75,"low":7223.0,"sparkline":[],"sparklineDateChanges":{}},"PA":{"label":"Palladium","ticker":"PA","last":1004.6,"change":-0.26,"prevClose":1007.2,"high":1007.1,"low":1003.5,"sparkline":[],"sparklineDateChanges":{}},"PL":{"label":"Platinum","ticker":"PL","last":900.9,"change":-0.18,"prevClose":902.5,"high":906.6,"low":899.2,"sparkline":[],"sparklineDateChanges":{}},"QA":{"label":"Crude Oil Brent","ticker":"QA","last":76.51,"change":-0.3,"prevClose":76.74,"high":76.78,"low":76.42,"sparkline":[],"sparklineDateChanges":{}},"RB":{"label":"Gasoline RBOB","ticker":"RB","last":2.1197,"change":-0.26,"prevClose":2.1252,"high":2.1236,"low":2.1174,"sparkline":[],"sparklineDateChanges":{}},"RS":{"label":"Canola","ticker":"RS","last":514.0,"change":-0.1,"prevClose":514.5,"high":516.1,"low":513.9,"sparkline":[],"sparklineDateChanges":{}},"SB":{"label":"Sugar","ticker":"SB","last":12.49,"change":1.13,"prevClose":12.35,"high":12.54,"low":12.24,"sparkline":[],"sparklineDateChanges":{}},"SI":{"label":"Silver","ticker":"SI","last":17.015,"change":0.14,"prevClose":16.991,"high":17.085,"low":16.965,"sparkline":[],"sparklineDateChanges":{}},"VX":{"label":"VIX","ticker":"VX","last":13.25,"change":0.19,"prevClose":13.225,"high":13.25,"low":13.0,"sparkline":[],"sparklineDateChanges":{}},"YM":{"label":"DJIA","ticker":"YM","last":25212.0,"change":0.02,"prevClose":25208.0,"high":25253.0,"low":25200.0,"sparkline":[],"sparklineDateChanges":{}},"ZB":{"label":"30 Year Bond","ticker":"ZB","last":143.09375,"change":0.35,"prevClose":142.59375,"high":143.15625,"low":142.78125,"sparkline":[],"sparklineDateChanges":{}},"ZC":{"label":"Corn","ticker":"ZC","last":373.0,"change":-0.8,"prevClose":376.0,"high":374.75,"low":370.75,"sparkline":[],"sparklineDateChanges":{}},"ZF":{"label":"5 Year Note","ticker":"ZF","last":113.1328125,"change":0.07,"prevClose":113.0546875,"high":113.15625,"low":113.0390625,"sparkline":[],"sparklineDateChanges":{}},"ZK":{"label":"Ethanol","ticker":"ZK","last":1.437,"change":-0.48,"prevClose":1.444,"high":1.45,"low":1.43,"sparkline":[],"sparklineDateChanges":{}},"ZL":{"label":"Soybean oil","ticker":"ZL","last":30.14,"change":0.2,"prevClose":30.08,"high":30.15,"low":30.03,"sparkline":[],"sparklineDateChanges":{}},"ZM":{"label":"Soybean Meal","ticker":"ZM","last":348.2,"change":0.14,"prevClose":347.7,"high":348.6,"low":346.7,"sparkline":[],"sparklineDateChanges":{}},"ZN":{"label":"10 Year Note","ticker":"ZN","last":119.34375,"change":0.13,"prevClose":119.1875,"high":119.375,"low":119.1875,"sparkline":[],"sparklineDateChanges":{}},"ZO":{"label":"Oats","ticker":"ZO","last":232.0,"change":-0.43,"prevClose":233.0,"high":233.5,"low":232.0,"sparkline":[],"sparklineDateChanges":{}},"ZR":{"label":"Rough Rice","ticker":"ZR","last":12.135,"change":0.37,"prevClose":12.09,"high":12.14,"low":12.09,"sparkline":[],"sparklineDateChanges":{}},"ZS":{"label":"Soybeans","ticker":"ZS","last":937.75,"change":0.19,"prevClose":936.0,"high":937.75,"low":931.25,"sparkline":[],"sparklineDateChanges":{}},"ZT":{"label":"2 Year Note","ticker":"ZT","last":105.7890625,"change":0.02,"prevClose":105.765625,"high":105.796875,"low":105.7578125,"sparkline":[],"sparklineDateChanges":{}},"ZW":{"label":"Wheat","ticker":"ZW","last":512.0,"change":-0.87,"prevClose":516.5,"high":517.0,"low":505.75,"sparkline":[],"sparklineDateChanges":{}}};
            */

            #endregion


            #region Futures

            //get all futures
            //String sAllGroupsAndTiles = futures.Substring(futures.IndexOf("var groups"), futures.IndexOf("</script>", futures.IndexOf("var groups") - futures.IndexOf("var groups")));
            //String sOnlyGroups = sAllGroupsAndTiles.Substring(futures.IndexOf("var groups"), futures.IndexOf(";", futures.IndexOf("var groups")) - 1);
            //String sOnlyTiles = sAllGroupsAndTiles.Substring(futures.IndexOf("var tiles"), futures.IndexOf(";", futures.IndexOf("var tiles")) - 1);

            //String sGroupsJSONString = sOnlyGroups.Replace("var groups = ", "").Replace(";", "").Trim();
            //String sTilesJSONString = sOnlyTiles.Replace("var tiles = ", "").Replace(";", "").Trim();

            #endregion


            return true;

        }



    }


    /*
     
        Classes to parse the JSON string of the futures.
        

    */

    public class Contract
    {
        public string label { get; set; }
        public string ticker { get; set; }
        public string cot { get; set; }
    }

    public class RootObject
    {
        public string ticker { get; set; }
        public string label { get; set; }
        public List<Contract> contracts { get; set; }
    }


}
