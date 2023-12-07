using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Program
{
    static async Task Main()
    {
        try
        {
            string[] cities = { "istanbul", "izmir", "ankara" };

            foreach (var city in cities)
            {
                string apiUrl = $"https://goweather.herokuapp.com/weather/{city}";
                WeatherData weatherData = await GetWeatherData(apiUrl);

                Console.WriteLine($"{city.ToUpper()} Hava Durumu ({weatherData.Date}):");
                PrintWeatherData(weatherData);
                PrintForecast(weatherData.Forecast);
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Bir Hata Meydana Geldi, Daha Sonra Tekrar Deneyiniz: {ex.Message}");
        }
    }

    static async Task<WeatherData> GetWeatherData(string apiUrl)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                WeatherData weatherData = JsonConvert.DeserializeObject<WeatherData>(json);
                weatherData.Date = DateTime.Now.ToString("yyyy-MM-dd");
                return weatherData;
            }
            else
            {
                throw new Exception($"Server'a Ulaşılamadı! Hata kodu: {response.StatusCode}");
            }
        }
    }

    static void PrintWeatherData(WeatherData weatherData)
    {
        Console.WriteLine($"Sıcaklık: {weatherData.Temperature}°C");
        Console.WriteLine($"Rüzgar saatte: {weatherData.Wind}");
        Console.WriteLine($"Durum: {TranslateDescription(weatherData.Description)}");
    }

    static void PrintForecast(Forecast[] forecast)
    {
        Console.WriteLine("\n3 Günlük Hava Durumu Tahminleri:");

        DateTime currentDate = DateTime.Now;

        foreach (var dayForecast in forecast)
        {
            currentDate = currentDate.AddDays(1);

            Console.WriteLine($"{currentDate.ToString("dd MMMM dddd")} : Sıcaklık: {dayForecast.Temperature}, Rüzgar: {dayForecast.Wind}");
        }
    }

    static string TranslateDescription(string englishDescription)
    {
        if (englishDescription != null)
        {
            string lowerDescription = englishDescription.ToLower();

            if (lowerDescription == "clear sky")
            {
                return "Açık Hava";
            }
            else if (lowerDescription == "partly cloudy")
            {
                return "Parçalı Bulutlu";
            }
            else if (lowerDescription == "light rain, mist" || lowerDescription == "light rain")
            {
                return "Hafif Yağmurlu, sisli";
            
           
            }
        }

        return englishDescription;
    }


    public class WeatherData
    {
        [JsonProperty("temperature")]
        public string Temperature { get; set; }

        [JsonProperty("wind")]
        public string Wind { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        public string Date { get; set; }

        [JsonProperty("forecast")]
        public Forecast[] Forecast { get; set; }
    }

    public class Forecast
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("wind")]
        public string Wind { get; set; }

        [JsonProperty("temperature")]
        public string Temperature { get; set; }
    }
}
