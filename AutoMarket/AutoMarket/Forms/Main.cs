using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoMarket
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }

        private void btnTestYahoo_Click(object sender, EventArgs e)
        {
            API.ApiYahoo apiYahoo = new API.ApiYahoo();

            //get historical quotes from yahoo
           // List<API.Candle> mCandle = new List<API.Candle>();
            DateTime dtStart = System.Convert.ToDateTime(dtDateFrom.Text);
            DateTime dtEnd = System.Convert.ToDateTime(dtDateTo.Text);

            lstRealTimeQuotes.Items.Clear();
            lstTestGoogle.Items.Clear();

            //Convert interval to seconds
            string sFactor = cboInterval.Text.Substring(cboInterval.Text.Length - 1, 1);
            int seconds = 0;
            if (sFactor == "m")
                seconds = System.Convert.ToInt32(cboInterval.Text.Replace("m", "")) * 60;
            else if (sFactor == "h")
                seconds = System.Convert.ToInt32(cboInterval.Text.Replace("h", "")) * 60 * 60;
            else if (sFactor == "d")
                seconds = System.Convert.ToInt32(cboInterval.Text.Replace("d", "")) * 60 * 60 * 24;
            else if (sFactor == "k")
                seconds = System.Convert.ToInt32(cboInterval.Text.Replace("wk", "")) * 60 * 60 * 24 * 5;
            else if (sFactor == "o")
                seconds = System.Convert.ToInt32(cboInterval.Text.Replace("mo", "")) * 60 * 60 * 24 * 5 * 4;

            //pass variables to the historical price.
            var testa = apiYahoo.GetHistoricalPrice(txtSymbol.Text, seconds, dtStart, dtEnd); 
            for (int i = 0; i < testa.Count(); i++)
            {
                lstTestGoogle.Items.Add(
                    testa.ElementAt(i).Date + ", " +
                    testa.ElementAt(i).Open + ", " +
                    testa.ElementAt(i).High + ", " +
                    testa.ElementAt(i).Low + ", " +
                    testa.ElementAt(i).Close + ", " +
                    testa.ElementAt(i).Volume
                    );
            }

            //Count the items in historical values
            lblCount.Text = testa.Count().ToString();

            //Get the quote from yahoo
            var testb = apiYahoo.GetQuote("MSFT");
            lstRealTimeQuotes.Items.Add("D: " + testb.Date.ToString("MM/dd/yyyy hh:mm:ss"));
            lstRealTimeQuotes.Items.Add("O: " + testb.Open);
            lstRealTimeQuotes.Items.Add("H: " + testb.High);
            lstRealTimeQuotes.Items.Add("C: " + testb.Close);
            lstRealTimeQuotes.Items.Add("L: " + testb.Low);
            lstRealTimeQuotes.Items.Add("V: " + testb.Volume);
        }

        private void btnTestTDAmeritrade_Click(object sender, EventArgs e)
        {

            if ((txtUsername.Text != "") && (txtPassword.Text != ""))
            {
                //(String key = "DEMO", String name = "TD Ameritrade Client Library for .NET", String version = "2.0.0")
                API.ApiTDAmeritrade apiTDAmeritrade = new API.ApiTDAmeritrade("DEMO", "TD Ameritrade Client Library for .NET", "2.0.0");
                bool islogin = apiTDAmeritrade.login(txtUsername.Text, txtPassword.Text);

                if (!islogin)
                {
                    islogin = false;
                }
            }
        }

        private void btnTestGoogle_Click(object sender, EventArgs e)
        {
            API.ApiGoogle apiGoogle = new API.ApiGoogle();
            lstTestGoogle.Items.Clear();
            lstRealTimeQuotes.Items.Clear();


            DateTime dtStart = System.Convert.ToDateTime(dtDateFrom.Text);
            DateTime dtEnd = System.Convert.ToDateTime(dtDateTo.Text);
            //Convert interval to seconds
            string sFactor = cboInterval.Text.Substring(cboInterval.Text.Length - 1, 1);

            int seconds = 0;
            if (sFactor == "m")
                seconds = System.Convert.ToInt32(cboInterval.Text.Replace("m", "")) * 60;
            else if (sFactor == "h")
                seconds = System.Convert.ToInt32(cboInterval.Text.Replace("h", "")) * 60 * 60;
            else if (sFactor == "d")
                seconds = System.Convert.ToInt32(cboInterval.Text.Replace("d", "")) * 60 * 60 * 24;
            else if (sFactor == "k")
                seconds = System.Convert.ToInt32(cboInterval.Text.Replace("wk", "")) * 60 * 60 * 24 * 5;
            else if (sFactor == "o")
                seconds = System.Convert.ToInt32(cboInterval.Text.Replace("mo", "")) * 60 * 60 * 24 * 5 * 4;


            var testa = apiGoogle.GetHistoricalPrice(txtSymbol.Text, seconds, dtStart, dtEnd);

            //var testa = apiGoogle.GetHistoricalPrice("MSFT");
            lblCount.Text = testa.Count().ToString();
            for (int i= 0; i < testa.Count(); i++)
            {
                lstTestGoogle.Items.Add(
                    testa.ElementAt(i).Date + ", " + 
                    testa.ElementAt(i).Open + ", " +
                    testa.ElementAt(i).High + ", " +
                    testa.ElementAt(i).Low + ", " +
                    testa.ElementAt(i).Close + ", " +
                    testa.ElementAt(i).Volume
                    );
                
            }

            //Get the quote from Google
            var testRTQuote = apiGoogle.GetQuote("MSFT");
            lstRealTimeQuotes.Items.Add("D: " + testRTQuote.Date.ToString("MM/dd/yyyy hh:mm:ss"));
            lstRealTimeQuotes.Items.Add("O: " + testRTQuote.Open);
            lstRealTimeQuotes.Items.Add("H: " + testRTQuote.High);
            lstRealTimeQuotes.Items.Add("C: " + testRTQuote.Close);
            lstRealTimeQuotes.Items.Add("L: " + testRTQuote.Low);
            lstRealTimeQuotes.Items.Add("V: " + testRTQuote.Volume);

        }
    }
}
