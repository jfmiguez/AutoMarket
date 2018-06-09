using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using Newtonsoft.Json;

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
        public Boolean login2(String userName, String password)
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

        public bool login(String userName, String password)
        {
            Http.RestCaller restCaller;
            restCaller = new Http.RestCaller();

            var requestBuilder = new UriBuilder("https://api.tdameritrade.com/v1/oauth2/token");

            var values = new Dictionary<string, string>()
                {
                    {"grant_type", "authorization_code"},
                    {"access_type","offline" },
                    //{"code", Request.Query["code"].ToString()},
                    //{"code", "0".ToString()},
                    {"code", ""},
                    {"client_id", "" },
                    {"redirect_uri", "http://127.0.0.1/" }
                };

            //HttpContent content = new FormUrlEncodedContent(values);
            FormUrlEncodedContent content = new FormUrlEncodedContent(values);
            //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = restCaller.Post(requestBuilder.ToString(), content);

            return true;
        }

        /* 
        
        public bool OAuthCallback(CancellationToken token)
        {
            ActionResult result = null;
            TokenModel tokenData = null;
            using (var client = new HttpClient())
            {
                var requestBuilder = new UriBuilder("https://api.tdameritrade.com/v1/oauth2/token");
                var requestQuery = HttpUtility.ParseQueryString(string.Empty);
                requestBuilder.Query = requestQuery.ToString();
                client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
                var values = new Dictionary<string, string>()
                {
                    {"grant_type", "authorization_code"},
                    {"access_type","offline" },
                    {"code", Request.Query["code"].ToString()},
                    {"client_id", "OAuth User ID" },
                    {"redirect_uri", "Redirect URI" }
                };

                try
                {
                    var tokenResult = await client.PostAsync(requestBuilder.ToString(), new FormUrlEncodedContent(values));
                    var content = await tokenResult.Content.ReadAsStringAsync();
                    if (tokenResult.StatusCode == HttpStatusCode.OK)
                    {
                        tokenData = JsonConvert.DeserializeObject<TokenModel>(content);
                        result = View("Success", tokenData);
                    }
                    else
                    {
                        result = new ContentResult { StatusCode = (int)tokenResult.StatusCode, Content = content };
                    }
                }
                catch (Exception ex)
                {
                    result = new ContentResult { StatusCode = (int)HttpStatusCode.InternalServerError, Content = ex.ToString() };
                }
            }

            return true;
        }
        
        */

    }
}

public class TokenModel
{
    public string access_token { get; set; }
    public string refresh_token { get; set; }
    public int expires_in { get; set; }
    public int refresh_token_expires_in { get; set; }
    public string token_type { get; set; }
}

