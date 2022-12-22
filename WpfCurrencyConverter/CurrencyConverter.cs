using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        Dictionary<string, Dictionary<string, decimal>> rates;

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

        public Dictionary<string, Dictionary<string, decimal>> GetTimeSeries(string fromCurrency, string toCurrency)
        {
            DateTime dateTimeNow = DateTime.Now;
            DateTime dateTimeYesterday = DateTime.Now.AddDays(-6);
            string endDate = dateTimeNow.ToString("yyyy-MM-dd");
            string startDate = dateTimeYesterday.ToString("yyyy-MM-dd");

            if (rates == null)
            {
                rates = new Dictionary<string, Dictionary<string, decimal>>();
                string responseContent = GetResponseString($"exchangerates_data/timeseries?start_date={startDate}&end_date={endDate}&base={fromCurrency}&symbols={toCurrency}");

                Dictionary<string, object> responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);
                if ((bool)responseData["success"])
                {
                    var tempRates = (JObject)responseData["rates"];
                    rates = tempRates.ToObject<Dictionary<string, Dictionary<string, decimal>>>();
                }
            }

            return rates;
        }

        internal decimal Exchange(string fromCurrency, string toCurrency, double currencyAmount)
        {
            string responseContent = GetResponseString($"exchangerates_data/convert?to={toCurrency}&from={fromCurrency}&amount={currencyAmount}");

            Dictionary<string, object> responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);
            if ((bool)responseData["success"])
            {
                return Convert.ToDecimal(responseData["result"]);
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
            request.AddHeader("apikey", "QgwEBdggi9yE3FwEcRbqRRS8zyvuecrN");

            RestResponse response = client.Execute(request);
            return response.Content;
        }
    }
}
