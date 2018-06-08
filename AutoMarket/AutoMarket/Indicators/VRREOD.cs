using System;
using System.Linq;
using System.Threading;
using System.ComponentModel;


namespace AutoMarket.Indicators
{
    /// <summary>
    /// Collects VRREOD data and places it in a variable m_Data where this is taken from
    ///  the class IndicatorData and loops within a background worker thread to forecast
    ///  volume data at the end of the day. Very important for breakouts.
    /// </summary>
    /// <example>
    /// oVRREOD = new Indicators.VRREOD("MSFT");        //make a new indicator, pass only the symbol.
    /// oVRREOD.StreamStatus = true;                    //start streaming
    /// oVRREOD.Start();                                //Async call. 
    /// while (oVRREOD.StreamStatus == true)            //pause until the status is true
    ///     Console.WriteLine(oVRREOD.Data.VRREOD);     //do something here like getting the average or the VRREOD.
    /// oVRREOD.Stop();                                 //Stop in case things go south.
    /// oVRREOD = null;                                 //remove object from memory.
    /// </example>
    public class VRREOD
    {
        private const Int32 INDICATOR_TIME_DWELL = 60000; // expressed in seconds.
        private DateTime m_TimeOut;
        private Boolean m_isTimeOut;
        private Boolean isStreaming = false;
        public event TickHandler m_Handler;
        public EventArgs e = null;
        public delegate void TickHandler(VRREOD m, EventArgs e);
        public Decimal VolumeAverage50d = 0;
        private IndicatorData m_Data;
        private String m_Symbol = "";


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="symbol">Need to pass the stock symbol because of speeding things up.</param>
        public VRREOD(String symbol)
        {
            //Times out when the market closes
            m_TimeOut = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 0, 0);

            //There is nothing to stream yet, set the flag to false.
            isStreaming = false;

            //There is no timeout yet
            m_isTimeOut = false;

            //Get the new placeholder to save the data
            m_Data = new IndicatorData();

            //Load the symbol in the data indicator
            m_Symbol = symbol;
            m_Data.Symbol = m_Symbol;
        }


        /// <summary>
        /// Returns all the collected data up to that point
        /// </summary>
        public IndicatorData Data { get { return m_Data; } }

        
        /// <summary>
        /// This collect function queries data from one of the APIs and
        ///  gets the volume and the 50d volume average. Then calculates
        ///  the time of the day and makes a projection of the volume
        ///  at the end of the day (aka forecast).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CollectFunction(object sender, EventArgs e)
        {
            do
            {
                //Query required data from google or yahoo
                API.ApiGoogle apiGoogle = new API.ApiGoogle();
                var RealTimeQuote = apiGoogle.GetQuote(m_Symbol);
                var Historical = apiGoogle.GetHistoricalPrice(m_Symbol, 60 * 60 * 24, DateTime.Now - TimeSpan.FromDays(50), DateTime.Now);
                m_Data.CurrentDate = DateTime.Now;

                //grab the current daily volume
                Decimal decCurrentVolume = RealTimeQuote.Volume;

                //grab the average volume 50 days
                Decimal decVolume50 = 0;
                for (int i = 0; i < Historical.Count(); i++)
                {
                    decVolume50 += Historical.ElementAt(i).Volume;
                }
                decVolume50 /= Historical.Count();

                //grab the delta of date and time in minutes
                Decimal decMinutesInProgress = (m_Data.CurrentDate.Hour - 9) * 60 + m_Data.CurrentDate.Minute;
                //decMinutesInProgress = (m_Data.CurrentDate.Hour - 9 - 9) * 60 + m_Data.CurrentDate.Minute; //simulating
                Decimal decTotalMinutes = (16M - 9.5M) * 60M;

                //do the slope in minutes=
                Decimal decMinuteFactor = (decTotalMinutes - decMinutesInProgress);

                // volume forecast
                Decimal decVolumeForecast = decCurrentVolume * decTotalMinutes / (decTotalMinutes - decMinutesInProgress);

                //volumeforecast / volumeaverage_50d = VRREOD
                Decimal decVRREOD = decVolumeForecast / decVolume50 * 100;

                //save this in the indicatorData
                m_Data.Volume = decVolumeForecast;
                m_Data.VolumeAverage = decVolume50;
                m_Data.VRREOD = decVRREOD;

                //verify timeout
                if (m_Data.CurrentDate > m_TimeOut)
                {
                    this.StreamStatus = false;
                    m_isTimeOut = true;
                    Stop();
                }

                Thread.Sleep(INDICATOR_TIME_DWELL);
            } while (this.StreamStatus == true);
        }


        /// <summary>
        /// If true, the thread will keep working, set to false if needs thread to stop.
        /// </summary>
        public Boolean StreamStatus { get { return isStreaming; } set { isStreaming = value; } }


        /// <summary>
        /// Returns true if there was a timeout, false otherwise.
        /// </summary>
        public Boolean TimeOut { get { return m_isTimeOut; } }


        /// <summary>
        /// Use this to run a background worker thread
        ///  and create an event handler to start an
        ///  asynchronous call.
        /// </summary>
        public void Start()
        {
            StreamStatus = true;
            
            //Generate a background worker
            BackgroundWorker bg = new BackgroundWorker();

            //Create the background event handler
            bg.DoWork += new DoWorkEventHandler(StartAsync);

            //Run the background worker
            bg.RunWorkerAsync();
        }


        /// <summary>
        /// Starts the thread in asynchronous mode from the background worker.
        /// </summary>
        /// <param name="sender">Object calling this function</param>
        /// <param name="e">Parameters that are needed (if any) for this function</param>
        /// <remarks>From the parent caller you may start also your own asynchronous call.
        /// Refer to the Start() function.
        /// </remarks>
        public void StartAsync(object sender, EventArgs e)
        {
            //Lets the softwawre know the stream is on.
            StreamStatus = true;

            //Create the event handler
            m_Handler += new TickHandler(CollectFunction);

            //Call the handler, this will be a loop but we
            // do not care because the startasync() is already 
            // in a background worker thread.
            m_Handler(this, e);
        }


        /// <summary>
        /// Gracefully stops the threads and any issues before disposing.
        /// </summary>
        public void Stop()
        {
            StreamStatus = false;
            System.Threading.Thread.Sleep(3000);
            m_Handler = null;
        }





    }
}
