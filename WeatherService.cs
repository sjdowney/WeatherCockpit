using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace WeatherCockpit
{
    public static class WeatherService
    {
        private static readonly HttpClient _http = new()
        {
            BaseAddress = new Uri("http://192.168.1.106/") // point to your API
        };

        public static async Task<WeatherData> GetLiveAsync(CockpitViewModel viewModel)
        {
            // Map your existing live-weather.php JSON to WeatherData
            try
            {
                var dto = await _http.GetFromJsonAsync<LiveWeatherDto>("api/live-weather.php");
                viewModel.SetStatus("... weather data /bsuccessfully/B downloaded");
                return Map(dto);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return null;
            }
            
        }

        private static WeatherData Map(LiveWeatherDto d)
        {
            return new WeatherData
            {
                Temp = d.temp,
                Humidity = d.humidity,
                Pressure = d.pressure,
                TempTrend = d.temp_trend,
                HumidityTrend = d.humidity_trend,
                PressureTrend = d.pressure_trend,
                WindSpeed = d.wind_speed,
                WindTrend = d.wind_trend,
                WindDirText = d.wind_dir_text,
                WindDirectionDegrees = d.wind_dir,
                Rain = d.rain,
                RainTrend = d.rain_trend,
                AsiTrend = d.asi_trend,
                Dpd = d.dpd,
                Lightning24h = d.lightning_24h,
                StormLightningDistance = d.storm_lightning_distance,
                StormLightningIntensity = d.storm_lightning_intensity,
                WeatherLabel = d.weather_label,
                MoonPhase = d.moon_phase,
                MoonRise = d.moon_rise,
                MoonSet = d.moon_set,
                AiForecast = d.ai_forecast,
                LastRefresh = ParseDate(d.last_refresh)
            };
        }

        private static DateTime ParseDate(string raw)
        {
            if (DateTime.TryParse(raw, out var dt))
                return dt;

            return DateTime.Now;
        }

        // Match your PHP JSON fields
        public class LiveWeatherDto
        {
            public double temp { get; set; }
            public double humidity { get; set; }
            public double pressure { get; set; }
            public double temp_trend { get; set; }
            public double humidity_trend { get; set; }
            public double pressure_trend { get; set; }
            public double wind_speed { get; set; }
            public double wind_trend { get; set; }
            public string wind_dir_text { get; set; } = "--";
            public double wind_dir { get; set; }
            public double rain { get; set; }
            public double rain_trend { get; set; }
            public double asi_trend { get; set; }
            public double dpd { get; set; }
            public int lightning_24h { get; set; }
            public double storm_lightning_distance { get; set; }
            public double storm_lightning_intensity { get; set; }
            public int storm_score { get; set; }
            public string weather_label { get; set; } = "";
            public string moon_phase { get; set; } = "";
            public string moon_rise { get; set; } = "";
            public string moon_set { get; set; } = "";
            public string ai_forecast { get; set; } = "";
            public string last_refresh { get; set; } = "";   // <-- FIXED
        }
    }
}
