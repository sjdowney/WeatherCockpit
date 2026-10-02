using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace WeatherCockpit
{
    public static class TideService
    {
        private static readonly HttpClient _http = new()
        {
            BaseAddress = new Uri("http://192.168.1.106/") // point to your API
        };

        public static async Task<TideData> GetLiveAsync()
        {
            // Map your existing live-weather.php JSON to WeatherData
            try
            {
                var dto = await _http.GetFromJsonAsync<TidesDto>("api/tides.php");
                return Map(dto);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            return null;
        }

        private static TideData Map(TidesDto d)
        {
            return new TideData
            {
                High1Time = ParseDate(d.high1),
                Low1Time = ParseDate(d.low1),
                High1Height = d.high1_height,
                Low1Height = d.low1_height,
                High2Time = ParseDate(d.high2),
                Low2Time = ParseDate(d.low2),
                High2Height = d.high2_height,
                Low2Height = d.low2_height
            };
        }
        private static DateTime ParseDate(string raw)
        {
            if (DateTime.TryParse(raw, out var dt))
                return dt;

            return DateTime.Now;
        }

        // Match your PHP JSON fields
        public class TidesDto
        {
            public required string high1 { get; set; }
            public required string low1 { get; set; }
            public required double high1_height { get; set; }
            public required double low1_height { get; set; }
            public required string high2 { get; set; }
            public required string low2 { get; set; }
            public required double high2_height { get; set; }
            public required double low2_height { get; set; }
        }
    }
}
