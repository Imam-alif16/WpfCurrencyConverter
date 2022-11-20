using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace WpfCurrencyConverter
{
    internal class CurrencyConverter
    {
        Dictionary<string, string> symbols;
        public Dictionary<string, string> GetSymbols()
        {
            if (symbols == null)
            {
                symbols = new Dictionary<string, string>();
                string responseContent = GetResponseString("exchangerates_data/symbols");

                Dictionary<string,object> responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);
                if ((bool)responseData["success"])
                {
                    var tempSymbols = (JObject)responseData["symbols"];
                    symbols = tempSymbols.ToObject<Dictionary<string, string>>();
                }
            }

            return symbols;
        }

        internal double Convert(string fromCurrency, string toCurrency, double currencyAmount)
        {
            string responseContent = GetResponseString($"exchangerates_data/convert?to={toCurrency}&from={fromCurrency}&amount={currencyAmount}");

            Dictionary<string, object> responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);
            if ((bool)responseData["success"])
            {
                return (double)responseData["result"];
            }
            else
            {
                return -1;
            }
        }

        private string GetResponseString(string relativeURI)
        {
            var client = new RestClient("https://api.apilayer.com/");

            var request = new RestRequest(relativeURI, Method.Get);
            request.AddHeader("apikey", "e0hnK3tZLDHcnlMhHCMMbd3OZzOPABqh");

            RestResponse response = client.Execute(request);
            return response.Content;
        }
    }
}
