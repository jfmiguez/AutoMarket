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
            var testa = apiYahoo.GetHistoricalPrice("MSFT");
            //var test1 = apiYahoo.GetRawHistoricalPrice("MSFT");

            System.Threading.Thread.Sleep(1000);


            Task<YahooFinanceAPI.Models.QuotePrice> quotedata = YahooFinanceAPI.Quote.GetPriceAsync("MSFT");


            var quotedataasync = YahooFinanceAPI.Quote.GetPriceAsync("MSFT");
            var quoterawasync = YahooFinanceAPI.Quote.GetRawAsync("MSFT");



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
            var testa = apiGoogle.GetHistoricalPrice("MSFT");
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


        }
    }
}
