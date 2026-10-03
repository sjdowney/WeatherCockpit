using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WeatherCockpit
{
    public static class TideService
    {
        private static readonly HttpClient _http = new()
        {
            BaseAddress = new Uri("http://192.168.1.106/")
        };

        public static async Task<TideData?> GetLiveAsync(CockpitViewModel viewModel)
        {
            try
            {
                var dto = await _http.GetFromJsonAsync<TidesDto>("api/tides.php");
                if (dto == null)
                    return null;

                // Push values into ViewModel
                viewModel.HighTideTime = dto.high1;
                viewModel.HighTide = Math.Round(dto.high1_height, 2);
                viewModel.LowTideTime = dto.low1;
                viewModel.LowTide = Math.Round(dto.low1_height, 2);

                return Map(dto);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TideService error: {ex}");
                return null;
            }
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
            return DateTime.TryParse(raw, out var dt)
                ? dt
                : DateTime.Now;
        }

        // Matches your PHP JSON fields
        public class TidesDto
        {
            public string high1 { get; set; } = "";
            public string low1 { get; set; } = "";
            public double high1_height { get; set; }
            public double low1_height { get; set; }

            public string high2 { get; set; } = "";
            public string low2 { get; set; } = "";
            public double high2_height { get; set; }
            public double low2_height { get; set; }
        }
    }
}
