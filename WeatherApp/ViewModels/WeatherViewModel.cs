using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Extensions;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.ViewModels
{
    public partial class WeatherViewModel : ViewModelBase
    {
        private readonly WeatherService _weatherService;

        [ObservableProperty]
        private string cityNameInput = string.Empty;

        [ObservableProperty]
        private Weather? currentWeather;

        [ObservableProperty]
        private ObservableCollection<Weather> weatherForecasts = new();



        public WeatherViewModel(WeatherService weatherService)
        {
            _weatherService = weatherService;
            
            //Запуск асинхронной задачи
            InitializationTask = Task.Run(async () =>
            {
                await LoadSavedDataAsync();
            });
        }

        [RelayCommand]
        private async Task GetWeatherAsync()
        {
            if (string.IsNullOrWhiteSpace(CityNameInput))
                return;

            var result = await _weatherService.GetWeatherByCityNameAsync(CityNameInput);
            if (result != null)
            {
                WeatherForecasts.Add(result);
                CityNameInput = string.Empty; 
                await SaveDataAsync();        
            }
        }

        /// <summary>
        /// Задача инициализации, необходима для 
        /// запуска асинхронной операции в конструкторе 
        /// </summary>
        TaskNotifier? _initializationTask;
        public Task? InitializationTask
        {
            get => _initializationTask;
            set => SetPropertyAndNotifyOnCompletion(ref _initializationTask, value);
        }

        private async Task LoadSavedDataAsync()
        {
            var savedCities = await Preferences.Load("saved_cities", new List<string>());

            foreach (var city in savedCities)
            {
                var weather = await _weatherService.GetWeatherByCityNameAsync(city);
                if (weather != null)
                {
                    // 5.2 Добавление в список через UI поток
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        WeatherForecasts.Add(weather);
                    });
                }
            }
        }
        private async Task SaveDataAsync()
        {
            var cityNames = WeatherForecasts.Select(w => w.City.Name).ToList();
            await Preferences.Save("saved_cities", cityNames);
        }


        [RelayCommand]
        private async Task DeleteWeather(Weather weather)
        {
            if (weather != null)
            {
                WeatherForecasts.Remove(weather);
                await SaveDataAsync();
            }
        }

        [RelayCommand]
        private async Task RefreshWeather(Weather weather)
        {
            if (weather == null)
                return;

            var updated = await _weatherService.GetWeatherByCityNameAsync(weather.City.Name);
            if (updated != null)
            {
                int index = WeatherForecasts.IndexOf(weather);
                WeatherForecasts[index] = updated;
                await SaveDataAsync();
            }
        }
    }
}
