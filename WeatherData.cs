using System;

namespace WeatherCockpit
{
    public class WeatherData
    {
        public double Temp { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }

        public double TempTrend { get; set; }
        public double HumidityTrend { get; set; }
        public double PressureTrend { get; set; }

        public double WindSpeed { get; set; }
        public double WindTrend { get; set; }
        public string WindDirText { get; set; } = "--";
        public double WindDirectionDegrees { get; set; }
        public double Rain { get; set; }
        public double RainTrend { get; set; }

        public double AsiTrend { get; set; }
        public double Dpd { get; set; }

        public int Lightning24h { get; set; }
        public double StormLightningDistance { get; set; }
        public double StormLightningIntensity { get; set; }

        public string WeatherLabel { get; set; } = "";
        public string MoonPhase { get; set; } = "";
        public string MoonRise { get; set; } = "";
        public string MoonSet { get; set; } = "";
        public string AiForecast { get; set; } = "";
        public DateTime LastRefresh { get; set; }
        public string StatusText { get; set; } = "Loading...";
    

    }
}
