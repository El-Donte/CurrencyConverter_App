using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter_App.CurrencyConvert
{
    // CurrencyData - класс с данными о валюте, включая отношение к определенной другой валюте
    internal class CurrencyData
    {
        public string CurrencyCode { get; set; }            // код валюты
        public string CurrencyName { get; set; }            // наименование валюты
        public decimal UnitCost { get; set; }               // стоимость единицы по отношению к заданной валюте

        public CurrencyData(string currencyCode, decimal unitCost)
        {
            CurrencyCode = currencyCode;
            UnitCost = unitCost;
        }
    }
}
