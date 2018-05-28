using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMarket.API
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Security;
    using System.Threading;
    


    class ApiHttp
    {
        private readonly HttpClient http;

        //private String sessionID;
        //private SecureString userID;
        //private TimeSpan timeout;
        //private StreamerInfo streamerInfo;
        private Uri BaseAddress;

        public ApiHttp()
        {
            http = new HttpClient();
        }


        public ApiHttp(Uri BaseAddress)
        {
            this.BaseAddress = BaseAddress;
            this.http = new HttpClient
            {
                BaseAddress = this.BaseAddress
            };
        }


        /// <summary>
        /// An Asynchronous POST to the HTTP site is done and awaits for a response.
        /// The task is a blocking task, so until the response is aquired.
        /// </summary>
        /// <param name="Url">URL of the http for post</param>
        /// <param name="urlEncodedContent"></param>
        /// <returns>Returns the barebone message from the http</returns>
        public async Task<HttpResponseMessage> asyncPost(String Url, HttpContent urlEncodedContent)
        {
            System.Net.Http.HttpResponseMessage response = null;
            response = await http.PostAsync(Url, urlEncodedContent);



            return response;
        }


    }
}
