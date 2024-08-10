using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter_App.CurrencyConvert
{
    // ICurrencyDataProvider - интерфейс, описывающий провайдера данных о валютах
    internal interface ICurrencyDataProvider
    {
        // GetCurrenciesDataAsync - асинхронное получение данных о валютах в виде списка
        Task<List<CurrencyData>> GetCurrenciesDataAsync();
    }
}
