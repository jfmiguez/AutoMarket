using System;
using System.Linq;
using AutoMarket.API;

namespace AutoMarket.Bases
{

    /*
     

        ToDo: THE DESIGN NEEDS TO BE REDONE. NEEDS TO START FROM THE END (CLOSEST DATE)
         AND GO TO THE BEGINNING, OTHERWISE WE WILL NEVER KNOW THE BASES AVAILABLE.

        
        
     */



    class CupWithHandler : Base
    {
        private String  BASE_NAME = "";
        private Decimal BASE_DEPTH_MINIMUM = 0;
        private Decimal BASE_HANDLE_DEPTH_MINIMUM = 0;
        private Decimal BASE_HANDLE_RISING_COUNT = 0;
        private Boolean m_HasFormedBase = false;
        private Boolean m_IsFormingBase = false;
        private BaseData m_Data;

        /// <summary>
        /// Constructor for base parameter initialization
        /// </summary>
        public CupWithHandler()
        {
            /// AT least the depth of the base needs to be 14%
            BASE_DEPTH_MINIMUM = 14;
            /// The base of the handle must go down at least 4%
            BASE_HANDLE_DEPTH_MINIMUM = 4;
            /// The  total count of days rising from the bottom of the handle must be 4
            BASE_HANDLE_RISING_COUNT = 4;

            BASE_NAME = "Cup with Handle";

            m_Data = new BaseData();
            m_Data.BaseName = BASE_NAME;
        }
        

        /// <summary>
        /// Returns true if the base has been formed
        /// </summary>
        public Boolean IsBaseFormed { get { return m_HasFormedBase; } }


        /// <summary>
        /// Returns true if the base is being formed
        /// </summary>
        public Boolean IsBaseForming { get { return m_IsFormingBase; } }


        /// <summary>
        /// Returns the name of the base.
        /// </summary>
        public String BaseName { get { return BASE_NAME; } }

        /// <summary>
        /// Returns the parameters of a base. This is particular of
        ///  a base and a base may have more or less parameters depending
        ///  on what base it is and what parameters we want to make public.
        /// </summary>
        public BaseData Data { get { return m_Data; } }


        /// <summary>
        /// Is this a base of the fetched type? Must do some data mining to find out.
        /// </summary>
        /// <param name="candle">Collection of OHLC parameters each with date and volume</param>
        /// <returns></returns>
        public BaseData FindBase(System.Collections.Generic.IEnumerable<Candle> candle)
        {
            Decimal CupLeftSide = 0;
            Decimal CupRightSide = 0;
            Decimal CupDeepest = 0;
            Decimal CupHandleBottom = 0;
            Decimal PivotPoint = 0;
            Decimal decBasePercentDeep = 0;
            Decimal decBaseMidValue = 0;
            Decimal decBaseHandlePercentDeep = 0;
            DateTime dtBaseStart = candle.ElementAt(0).Date;
            DateTime dtBaseEnds = candle.ElementAt(0).Date;
            DateTime dtStartDate = candle.ElementAt(0).Date;
            DateTime dtCurrentDate = dtStartDate;
            DateTime dtPivotPoint = dtStartDate;
            TimeSpan tsDates;
            Double  dCurrentNumberofDays = 0;
            Boolean isBaseDuration = false;
            Boolean isBaseDeepEnough = false;
            Boolean isBaseHandleDeepEnough = false;
            Boolean isCalculatingHandle = false;
            Boolean isHandleRising = false;


            for ( int i = 0; i < candle.Count(); i++ )
            {
                //Calculate the left side of the cup (highest point)
                // The base starts at the left side of the cup.
                if (CupLeftSide < candle.ElementAt(i).High)
                {
                    dtStartDate = candle.ElementAt(i).Date;
                    CupLeftSide = candle.ElementAt(i).High;
                    CupDeepest = CupLeftSide;
                    CupRightSide = 0;
                    CupHandleBottom = 0;
                    PivotPoint = 0;
                }

                //Calculate the deepest part of the cup 
                // its dropped percentage
                // and the mid value (50% of depth)
                // we are not interested in dates a low point of the cup, only when rising.
                if (CupDeepest > candle.ElementAt(i).Low)
                {
                    dtCurrentDate = dtStartDate;
                    dtBaseStart = dtCurrentDate;
                    CupDeepest = candle.ElementAt(i).Low;
                    CupRightSide = CupDeepest;
                    CupHandleBottom = CupDeepest;
                    PivotPoint = CupDeepest;
                    decBasePercentDeep = (1 - CupDeepest / CupLeftSide) * 100;
                    decBaseMidValue = CupDeepest + (CupLeftSide - CupDeepest) / 2;
                    if (decBasePercentDeep > BASE_DEPTH_MINIMUM)
                        isBaseDeepEnough = true;
                }
                

                //The right side of the cup. Calculate this by looking at the deepest
                // and if bigger than deepest then cup is rising, but see if the base
                // is deep enough first.
                if ( CupDeepest < candle.ElementAt(i).High )
                {
                    if (isBaseDeepEnough && !isCalculatingHandle)
                    {
                        CupRightSide = candle.ElementAt(i).High;
                        if (decBaseMidValue < candle.ElementAt(i).Close)
                        {
                            dtCurrentDate = candle.ElementAt(i).Date;
                            dtBaseEnds = dtCurrentDate;
                        }
                    }
                }


                // Handle can form on the upper part of the cup to be effective
                // to calculate the base, at a minimum we want to rise 50% of depth.
                if (CupRightSide > decBaseMidValue)
                {
                    //Check if the closing is lower for 4 consecutive days and calculate 
                    // the depth of the handle by looking at the lowest point of the handle.
                    //Trace back and see when is the highest point from those 4 consecutive days.
                    if ((i + 5) < candle.Count())
                    {
                        if ((candle.ElementAt(i + 0).Close >= candle.ElementAt(i + 1).Close) &&
                            (candle.ElementAt(i + 1).Close >= candle.ElementAt(i + 2).Close) &&
                            (candle.ElementAt(i + 2).Close >= candle.ElementAt(i + 3).Close) &&
                            (candle.ElementAt(i + 3).Close >= candle.ElementAt(i + 4).Close))
                        {
                            //we are now in the handle part area
                            isCalculatingHandle = true;
                        }
                    }
                }

                //calculate depth of handle
                if (isCalculatingHandle)
                {
                    if ( (PivotPoint == 0) || (PivotPoint == CupHandleBottom) )
                    {
                        PivotPoint = candle.ElementAt(i).High + System.Convert.ToDecimal(0.10);
                        CupHandleBottom = PivotPoint;
                        dtPivotPoint = candle.ElementAt(i).Date;
                    }
                    if (CupHandleBottom > candle.ElementAt(i).Low)
                    {
                        CupHandleBottom = candle.ElementAt(i).Low;
                        decBaseHandlePercentDeep = ( 1 - ( CupHandleBottom / PivotPoint ) ) * 100;
                        if (decBaseHandlePercentDeep > BASE_HANDLE_DEPTH_MINIMUM)
                            isBaseHandleDeepEnough = true;
                    }
                }
                                
                //Calculate time of base
                tsDates = dtCurrentDate - dtStartDate;
                dCurrentNumberofDays = tsDates.TotalDays;
                if (dCurrentNumberofDays > 49) //7 weeks
                    isBaseDuration = true;
            }

            //is handle rising?
            int iTimesRising = 0;
            for (int i = 1; i < candle.Count(); i++)
            {
                if (dtPivotPoint < candle.ElementAt(i).Date)
                {
                    if (candle.ElementAt(i).Close > candle.ElementAt(i-1).Close)
                    {
                        iTimesRising++;
                    }
                }

                //is close to pivot point
                if (iTimesRising >= BASE_HANDLE_RISING_COUNT)
                    isHandleRising = true;
            }

            
            // Denote base formation

            if ( isBaseDuration && isBaseDeepEnough )
                m_IsFormingBase = true;

            if (isBaseHandleDeepEnough)
                m_HasFormedBase = true;
            
            m_Data.DateBaseStarts = dtBaseStart;
            m_Data.DateBaseEnds = dtBaseEnds;
            m_Data.HeightBaseStarts = CupLeftSide;
            m_Data.HeightBaseEnds = CupRightSide;
            m_Data.HeightMidpointBase = decBaseMidValue;
            m_Data.DepthBasePercent = decBasePercentDeep;
            m_Data.PivotPoint = PivotPoint;
            m_Data.DateHandleStarts = dtPivotPoint;
            m_Data.DepthHandlePercent = decBaseHandlePercentDeep;

            return m_Data;
        }


        public virtual String Findsomething(String parameter1, int parameter2)
        {
            return parameter1 + parameter2.ToString();
        }

        public String FindIt()
        {
            return "test2";
        }


    }
}
