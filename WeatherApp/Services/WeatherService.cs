using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherApp.Extensions;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    /// <summary>
    /// Сервис для получения информации о погоде
    /// </summary>
    public class WeatherService
    {
        /// <summary>
        /// Ключ для запросов к погодному API
        /// </summary>
        private readonly string _apiKey;

        public WeatherService()
        {
            var key = App.Configuration.GetSection("ApiKeys")["WeatherApi"];
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Weather API key not found in appsettings.json");
            }
            _apiKey = key;
        }

        /// <summary>
        /// Получение погоды в городе по наименованию
        /// </summary>
        /// <param name="cityName">название города</param>
        /// <returns>информация о погоде в городе</returns>
        public async Task<Weather> GetWeatherByCityNameAsync(string cityName)
        {
            // Опция, необходимая для корректной десериализации данных JSON из openweathermap
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new WeatherJsonConverter());

            string url = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={_apiKey}&units=metric&lang=ru";

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    var weatherData = JsonSerializer.Deserialize<Weather>(jsonResponse, options);

                    return weatherData;
                }
                else
                {
                    throw new Exception($"Не удалось получить данные о погоде: {response.StatusCode}");
                }
            }

        }

        /// <summary>
        /// Получение погоды в городе по координатам
        /// </summary>
        /// <param name="latitude">Широта</param>
        /// <param name="longitude">Долгота</param>
        /// <returns>информация о погоде в городе</returns>
        public async Task<Weather> GetWeatherByGeoAsync(double latitude, double longitude)
        {
            // Опция, необходимая для корректной десериализации данных JSON из openweathermap
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new WeatherJsonConverter());

            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&units=metric&lang=ru&appid={_apiKey}";

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    var weatherData = JsonSerializer.Deserialize<Weather>(jsonResponse, options);

                    return weatherData;
                }
                else
                {
                    throw new Exception("Не удалось получить данные о погоде по координатам");
                }
            }

        }

        /// <summary>
        /// Получение городов по названию
        /// </summary>
        /// <param name="cityName">название города</param>
        /// <param name="limit">максимум городов, который может вернуть метод</param>
        /// <returns>список городов с координатами</returns>
        public async Task<List<City>> GetGeoByCityNameAsync(string cityName, int limit=1)
        {
            // Опция, необходимая для корректной десериализации данных JSON из openweathermap 
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new CityJsonConverter());

            string url = $"http://api.openweathermap.org/geo/1.0/direct?q={cityName}&limit=5&appid={_apiKey}\"";

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    var cities = JsonSerializer.Deserialize<List<City>>(jsonResponse, options);

                    return cities ?? new List<City>();
                }
                else
                {
                    throw new Exception("Ошибка при поиске города");
                }

            }
        }
    }
}

