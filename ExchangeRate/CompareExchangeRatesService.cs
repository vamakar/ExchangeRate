using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExchangeRate
{
    internal class CompareExchangeRatesService
    {

        public string CompareExchangeRates(CurrencyData selected, IReadOnlyCollection<CurrencyData> currencyRateToday, IReadOnlyCollection<CurrencyData> currencyRateByDate)
        {
            CurrencyData usdToday = currencyRateToday.FirstOrDefault(x => x.Code == "USD");
            CurrencyData usdByDate = currencyRateByDate.FirstOrDefault(x => x.Code == "USD");
            CurrencyData selectedToday = currencyRateToday.FirstOrDefault(x => x.Code == selected.Code);

            if (usdToday == null || usdByDate == null || selectedToday == null)
                throw new InvalidOperationException();
            
            var builder = new StringBuilder();
            if (selected.Code != "RUB")
            {
                builder.Append(selected.Nominal);
                builder.Append(" ");
                builder.Append(selected.Code);
                builder.Append(" = ");
                builder.Append(Math.Round(selected.Value, 4));
                builder.Append(" RUB");
                decimal deltaRub = Math.Round(selected.Value - selectedToday.Value, 4);
                builder.Append(deltaRub > 0 ? " \t+" : " \t");
                builder.Append(deltaRub);
                builder.Append(" RUB");
                builder.AppendLine(); 
            }

            if (selected.Code != "USD")
            {
                builder.Append(selected.Nominal);
                builder.Append(" ");
                builder.Append(selected.Code);
                builder.Append(" = ");
                decimal valueUsd = Math.Round(selected.Value / usdByDate.Value * usdByDate.Nominal, 4);
                builder.Append(valueUsd);
                builder.Append(" USD");
                decimal valueUsdToday = selectedToday.Value / usdToday.Value * usdToday.Nominal;
                decimal deltaUsd = Math.Round(valueUsd - valueUsdToday, 4);
                builder.Append(deltaUsd > 0 ? " \t+" : " \t");
                builder.Append(deltaUsd);
                builder.Append(" USD");

                return builder.ToString();
            }

            return builder.ToString();
        }
    }
}
