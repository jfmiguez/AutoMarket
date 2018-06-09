using System;
using System.Linq;
using AutoMarket.API;


namespace AutoMarket.Bases
{

    class TightThreeWeeks : Base
    {
        private String BASE_NAME = "";
        private Decimal BASE_DEPTH_MINIMUM = 0;
        private Decimal BASE_HANDLE_DEPTH_MINIMUM = 0;
        private Decimal BASE_HANDLE_RISING_COUNT = 0;
        private Boolean m_HasFormedBase = false;
        private Boolean m_IsFormingBase = false;
        private BaseData m_Data;

        /// <summary>
        /// Constructor for base parameter initialization
        /// </summary>
        public TightThreeWeeks()
        {
            /// AT least the depth of the base needs to be 14%
            BASE_DEPTH_MINIMUM = 20;
            /// The base of the handle must go down at least 4%
            BASE_HANDLE_DEPTH_MINIMUM = 0;
            /// The  total count of days rising from the bottom of the handle must be 4
            BASE_HANDLE_RISING_COUNT = 0;

            BASE_NAME = "Tight Three Weeks";
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
            return m_Data;
        }


        public virtual String Findsomething(String parameter1, int parameter2)
        {
            return parameter1 + parameter2.ToString();
        }

        public String FindIt()
        {
            return "Tight Three Weeks";
        }



    }



}







