using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace ExchangeRate
{
    public class GetExchangeRateService
    {
        private const string Uri = "https://www.cbr.ru/scripts/XML_daily.asp";

        public async Task<ExchangeRateData> GetExchangeRateByDate(DateTime dateTime)
        {
            var client = new HttpClient();
            var builder = new UriBuilder(Uri);
            NameValueCollection query = HttpUtility.ParseQueryString(builder.Query);
            query["date_req"] = dateTime.ToString("dd/MM/yyyy");
            builder.Query = query.ToString();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            byte[] response = await client.GetByteArrayAsync(builder.ToString());

            string date;
            IReadOnlyCollection<CurrencyData> currencyDataCollection;
            using (var stream = new MemoryStream(response))
            {
                var serializer = new XmlSerializer(typeof(ValCurs));
                var valCurs = (ValCurs)serializer.Deserialize(stream);
                currencyDataCollection = valCurs.Valute
                    .Select(v => new CurrencyData(v.CharCode, v.Name, v.Nominal, Convert.ToDecimal(v.Value)))
                    .Append(new CurrencyData("RUB", "Российский рубль", 1, 1))
                    .ToArray();
                date = valCurs.Date;
            }

            return new ExchangeRateData(date, currencyDataCollection);
        }
    }

    [XmlType(AnonymousType = true)]
    public class ValCurs
    {
        [XmlElement("Valute")]
        public ValCursValute[] Valute { get; set; }

        [XmlAttribute]
        public string Date { get; set; }

        [XmlAttribute]
        public string name { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class ValCursValute
    {
        public short NumCode { get; set; }

        public string CharCode { get; set; }

        public short Nominal { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        [XmlAttribute]
        public string ID { get; set; }
    }
}
