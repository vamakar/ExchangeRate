using System.Collections.Generic;

namespace ExchangeRate
{
    public class ExchangeRateData
    {
        public string Date { get; }
        public IReadOnlyCollection<CurrencyData> DataCollection { get; }
        public ExchangeRateData(string date, IReadOnlyCollection<CurrencyData> dataCollection)
        {
            Date = date;
            DataCollection = dataCollection;
        }
    }

    public class CurrencyData
    {
        public string Code { get; }
        public string Name { get; }
        public decimal Nominal { get; }
        public decimal Value { get; }
        public CurrencyData(string code, string name, decimal nominal, decimal value)
        {
            Code = code;
            Name = name;
            Nominal = nominal;
            Value = value;
        }
    }
}
