using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Source:
//  https://www.tdameritrade.com/api.page
//  https://developer.tdameritrade.com/
namespace AutoMarket.API
{
    public class ApiTDAmeritrade
    {
        /// key:         Organization's unique identifier to be passed as part of every request to the TD Ameritrade Trading Platform
        /// name:        Organization's name to be passed to the TD Ameritrade Trading Platform during authentication
        /// version:     The package's version to be passed to the TD Ameritrade Trading Platform during authentication
        /// BaseAddress: Initial address of TD Ameritrade API location
        private readonly string key;
        private readonly string name;
        private readonly string version;
        private Uri BaseAddress = new Uri("https://apis.tdameritrade.com");
        ApiHttp http;

        //public ApiTDAmeritrade(String key = "DEMO", String name = "TD Ameritrade Client Library for .NET", String version = "2.0.0")
        public ApiTDAmeritrade(String key = "FEA15AEF51FFF@AMER.OAUTHAP", String name = "TD Ameritrade Client Library for .NET", String version = "1.0.0")
        {
            this.key = key;
            this.name = name;
            this.version = version;
            //http = new ApiHttp(BaseAddress);
            http = new ApiHttp();
        }


        //Login to the TDAmeritrade system
        public Boolean login(String userName, String password)
        {
            Boolean hasLoggedinSuccessfully = false;
            

            String UrlAuthentication = "https://auth.tdameritrade.com/auth?response_type=code&redirect_uri={URLENCODED REDIRECT URI}&client_id={URLENCODED OAUTH USER ID}";
            UrlAuthentication = UrlAuthentication.Replace("{URLENCODED REDIRECT URI}" , "http://localhost");
            UrlAuthentication = UrlAuthentication.Replace("{URLENCODED OAUTH USER ID}", this.key);
            System.Net.Http.HttpContent content = null;
            var isAuth = http.asyncPost(UrlAuthentication, content);

            String urlAddress = "/apps/300/LogIn?source=" + Uri.EscapeDataString(this.key) + "&version=" + Uri.EscapeDataString(this.version);
            urlAddress = BaseAddress + urlAddress;

            System.Net.Http.HttpContent httpContent = new System.Net.Http.FormUrlEncodedContent(new[]
                            {
                                            new KeyValuePair<string, string>("userid", userName),
                                            new KeyValuePair<string, string>("password", password),
                                            new KeyValuePair<string, string>("source", this.key),
                                            new KeyValuePair<string, string>("version", this.version)
                            });

            

            var result = http.asyncPost(urlAddress, httpContent); 

            return hasLoggedinSuccessfully;
        }



    }
}
