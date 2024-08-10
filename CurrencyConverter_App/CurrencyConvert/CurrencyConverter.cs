using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter_App.CurrencyConvert
{
    // CurrencyConverter - класс с логикой конвертации
    internal class CurrencyConverter
    {
        // флажок готов ли конвертер к работе
        private bool isReady;
        // базовая валюта для конвертации
        private string basicCurrencyCode;
        // коэффициенты валют по отношению к базовой: ключ - код валюты, значение - данные о валюте
        private Dictionary<string, CurrencyData> coefficients;   
        // провайдер для получения данных о валютах откуда-либо
        private readonly ICurrencyDataProvider _currencyDataProvider;

        // конструктор
        public CurrencyConverter(ICurrencyDataProvider currencyDataProvider) {
            _currencyDataProvider = currencyDataProvider;
            coefficients = new Dictionary<string, CurrencyData>();
            isReady = false;
        }

        // Convert - конвертация value из валюты fromCurrencyCode в валюту toCurrencyCode
        public decimal Convert(string fromCurrencyCode, string toCurrencyCode, decimal value)
        {
            CheckConverterIsReady();
            if (!coefficients.ContainsKey(fromCurrencyCode))
            {
                throw new InvalidOperationException($"no currency '{fromCurrencyCode}'");
            }
            if (!coefficients.ContainsKey(toCurrencyCode))
            {
                throw new InvalidOperationException($"no currency '{toCurrencyCode}'");
            }
            decimal fromCoef = coefficients[fromCurrencyCode].UnitCost;
            decimal toCoef = coefficients[toCurrencyCode].UnitCost;
            return value * fromCoef / toCoef;
        }

        // GetCurrencyCodes - получение списка доступных для конвертации кодов валют
        public List<string> GetCurrencyCodes()
        {
            CheckConverterIsReady();
            return coefficients.Keys.ToList(); // вернуть список ключей
        }

        // GetCurrencyData - получение списка данных о валютах со стоимостью по отношению к заданной единице
        public List<CurrencyData> GetCurrencyData(string basicCurrencyCode)
        {
            CheckConverterIsReady();
            if (!coefficients.ContainsKey(basicCurrencyCode))
            {
                throw new InvalidOperationException($"no currency '{basicCurrencyCode}'");
            }
            List<CurrencyData> currencyDatas = new List<CurrencyData>();
            foreach (string currency in GetCurrencyCodes())
            {
                decimal unitCost = Convert(currency, basicCurrencyCode, 1);
                currencyDatas.Add(new CurrencyData(currency, unitCost));
            }
            return currencyDatas;
        }

        // RefreshAsync - обновить коды валют (возможно через внешний источник)
        public async Task RefreshAsync()
        {
            List<CurrencyData> currenciesData = await _currencyDataProvider.GetCurrenciesDataAsync();
            coefficients.Clear();
            bool isBasicCurrencyMet = false;    // встречали ли уже базовую валюту
            foreach (CurrencyData currencyData in currenciesData)
            {
                coefficients[currencyData.CurrencyCode] = currencyData;
                if (currencyData.UnitCost == 1)
                {
                    if (isBasicCurrencyMet)
                    {
                        throw new InvalidOperationException($"Duplication of basic currency: " +
                            $"exists {basicCurrencyCode} and {currencyData.CurrencyCode} met");
                    }
                    // встрели базовую валюту
                    basicCurrencyCode = currencyData.CurrencyCode;
                    isBasicCurrencyMet = true;
                }
            }
            if (!isBasicCurrencyMet)
            {
                throw new InvalidOperationException("No basic currency in data");
            }
            // после успешного заполнения конвертер готов к работе
            isReady = true;
        }

        // CheckConverterIsReady - проверка готов ли конвертер
        private void CheckConverterIsReady()
        {
            if (!isReady)
            {
                throw new InvalidOperationException("converter is not ready");
            }
        }
    }
}
