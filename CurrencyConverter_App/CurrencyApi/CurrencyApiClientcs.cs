using CurrencyConverter_App.CurrencyConvert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net;
using System.Xml;

namespace CurrencyConverter_App.CurrencyApi
{
    internal class CurrencyApiClient : ICurrencyDataProvider
    {
        private string URL = "https://open.er-api.com/v6/latest/RUB";
        HttpClient client;

        public CurrencyApiClient()
        {
            client = new HttpClient();
        }
        public async Task<List<CurrencyData>> GetCurrenciesDataAsync()
        {
            HttpResponseMessage response = await client.GetAsync(URL);

            var content = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<CurrencyRates>(content);

            List<CurrencyData> rates = new List<CurrencyData>();

            foreach (var rate in data.rates)
            {
                rates.Add(
                    new CurrencyData(
                        rate.Key,
                        rate.Value
                    )
                );
            }

            return rates;
        }
    }
}
