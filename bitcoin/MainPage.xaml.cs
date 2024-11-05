using System.Net;
using System.Text.Json;

namespace bitcoin
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            LoadExchangeRates();
        }

        private async void LoadExchangeRates()
        {
            double usd = await GetUsdExchangeRate();
            var bitcoinRates = await GetBitcoinRates();

            lblUSD.Text = $"{bitcoinRates.bpi.USD.code}: {bitcoinRates.bpi.USD.rate_float?.ToString("N4")}";
            lblGBP.Text = $"{bitcoinRates.bpi.GBP.code}: {bitcoinRates.bpi.GBP.rate_float?.ToString("N4")}";
            lblEUR.Text = $"{bitcoinRates.bpi.EUR.code}: {bitcoinRates.bpi.EUR.rate_float?.ToString("N4")}";
            lblPLN.Text = $"PLN: {(usd * bitcoinRates.bpi.USD.rate_float ?? 0).ToString("N4")}";
        }

        private async Task<double> GetUsdExchangeRate()
        {
            string dzis = DateTime.Now.ToString("yyyy-MM-dd");
            string urlUSD = $"https://api.nbp.pl/api/exchangerates/rates/c/usd/{dzis}/?format=json";
            using (var webClient = new WebClient())
            {
                var json = await webClient.DownloadStringTaskAsync(urlUSD);
                USD dolar = JsonSerializer.Deserialize<USD>(json);
                return (double)dolar.rates[0].ask; // Pobranie kursu sprzedaży
            }
        }

        private async Task<Bitcoin> GetBitcoinRates()
        {
            string url = "https://api.coindesk.com/v1/bpi/currentprice.json";
            using (var webClient = new WebClient())
            {
                var json = await webClient.DownloadStringTaskAsync(url);
                return JsonSerializer.Deserialize<Bitcoin>(json);
            }
        }
    }

    public class BitcoinRate
    {
        public string? code { get; set; }
        public string? description { get; set; }
        public double? rate_float { get; set; }
    }

    public class BitcoinRate2
    {
        public BitcoinRate? USD { get; set; }
        public BitcoinRate? GBP { get; set; }
        public BitcoinRate? EUR { get; set; }
    }

    public class Bitcoin
    {
        public string? chartName { get; set; }
        public BitcoinRate2 bpi { get; set; }
    }

    public class USD
    {
        public string? code { get; set; }
        public IList<Rate> rates { get; set; }
    }

    public class Rate
    {
        public double? ask { get; set; }
        public double? bid { get; set; }
    }
}