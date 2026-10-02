using System;

namespace WeatherCockpit
{
    public static class StormEngine
    {
        public static int ComputeStormScore(WeatherData d)
        {
            int score = 0;

            if (d.PressureTrend <= -0.8) score += 10;
            if (d.PressureTrend <= -1.5) score += 20;
            if (d.PressureTrend <= -2.5) score += 30;

            if (d.HumidityTrend >= 3) score += 10;
            if (d.HumidityTrend >= 6) score += 15;

            if (d.AsiTrend >= 1.0) score += 10;
            if (d.AsiTrend >= 2.0) score += 20;

            if (d.WindSpeed >= 10) score += 10;
            if (d.WindSpeed >= 20) score += 20;

            if (Math.Abs(d.WindTrend) >= 4) score += 10;

            if (d.Dpd <= 10) score += 10;
            if (d.Dpd <= 6) score += 15;

            if (d.RainTrend >= 1.0) score += 10;
            if (d.Rain >= 2.0) score += 10;
            if (d.Rain >= 10.0) score += 20;

            if (d.Lightning24h > 0 && d.StormLightningDistance <= 40)
                score += 20;

            if (d.StormLightningDistance <= 15 && d.StormLightningIntensity >= 120)
                score += 30;

            return Math.Min(score, 100);
        }

        public static StormSignature BuildSignature(WeatherData d, int score)
        {
            string level = "Clear";
            if (score >= 30) level = "Low";
            if (score >= 50) level = "Moderate";
            if (score >= 70) level = "High";

            string pressure = "Pressure Stable";
            if (d.PressureTrend <= -1.0) pressure = "Pressure Falling";
            if (d.PressureTrend <= -2.5) pressure = "Rapid Pressure Drop";
            if (d.PressureTrend >= 1.0) pressure = "Pressure Rising";

            string humidity = "Humidity Stable";
            if (d.HumidityTrend >= 3) humidity = "Humidity Rising";
            if (d.HumidityTrend >= 6) humidity = "Humidity Rising Fast";
            if (d.HumidityTrend <= -3) humidity = "Humidity Falling";

            string wind = "Wind Calm";
            if (d.WindSpeed >= 5) wind = "Moderate Wind";
            if (d.WindSpeed >= 12) wind = "Strong Wind";
            if (Math.Abs(d.WindTrend) >= 4) wind = "Gust Front";

            string instability = "Low Instability";
            if (d.AsiTrend >= 1.0) instability = "Instability Rising";
            if (d.AsiTrend >= 2.0) instability = "Instability Surge";

            string dpd = $"DPD {d.Dpd:F1}°C";
            if (d.Dpd <= 10) dpd = "Moist Air";
            if (d.Dpd <= 6) dpd = "Very Moist Air";

            return new StormSignature
            {
                Level = level,
                PressureDriver = pressure,
                HumidityDriver = humidity,
                WindDriver = wind,
                InstabilityDriver = instability,
                DpdDriver = dpd
            };
        }
    }

    public class StormSignature
    {
        public string Level { get; set; } = "";
        public string PressureDriver { get; set; } = "";
        public string HumidityDriver { get; set; } = "";
        public string WindDriver { get; set; } = "";
        public string InstabilityDriver { get; set; } = "";
        public string DpdDriver { get; set; } = "";
    }
}
