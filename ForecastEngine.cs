using System.Collections.Generic;

namespace WeatherCockpit
{
    public static class ForecastEngine
    {
        public static string BuildForecastSummary(WeatherData d, int stormScore)
        {
            var parts = new List<string>();

            if (d.TempTrend <= -1.0) parts.Add("Temperatures are falling quickly.");
            else if (d.TempTrend <= -0.3) parts.Add("Temperatures are gradually cooling.");
            else if (d.TempTrend >= 1.0) parts.Add("Temperatures are rising rapidly.");
            else if (d.TempTrend >= 0.3) parts.Add("Temperatures are warming slightly.");
            else parts.Add("Temperatures remain steady.");

            if (d.HumidityTrend >= 6) parts.Add("Humidity is increasing sharply.");
            else if (d.HumidityTrend >= 3) parts.Add("Humidity is building.");
            else if (d.HumidityTrend <= -3) parts.Add("Humidity is easing.");
            else parts.Add("Humidity levels are stable.");

            if (d.PressureTrend <= -2.5)
                parts.Add("Pressure is dropping rapidly — storm activity likely.");
            else if (d.PressureTrend <= -1.0)
                parts.Add("Pressure is falling — unsettled weather may form.");
            else if (d.PressureTrend >= 1.0)
                parts.Add("Pressure is rising — stable conditions.");
            else
                parts.Add("Pressure remains steady.");

            if (d.WindSpeed < 1) parts.Add("Winds are calm.");
            else if (d.WindSpeed < 5) parts.Add("Light winds present.");
            else if (d.WindSpeed < 12) parts.Add("Moderate winds blowing.");
            else parts.Add("Strong winds developing.");

            if (d.Rain >= 10) parts.Add("Heavy rain falling.");
            else if (d.Rain >= 2) parts.Add("Steady rain present.");
            else if (d.Rain >= 0.2) parts.Add("Light rain present.");
            else if (d.RainTrend >= 0.5) parts.Add("Rain may begin developing soon.");
            else parts.Add("Conditions remain dry.");

            if (stormScore >= 70)
                parts.Add("Severe storm risk — strong instability signature detected.");
            else if (stormScore >= 50)
                parts.Add("Storm conditions may develop — instability increasing.");
            else if (stormScore >= 30)
                parts.Add("Atmospheric instability increasing slightly.");

            return string.Join(" ", parts);
        }

        public static string GetModeLabel(int stormScore, double rain)
        {
            string mode = "CLEAR SKY MODE";

            if (stormScore >= 40) mode = "STORM WATCH MODE";
            if (stormScore >= 70) mode = "STORM WARNING MODE";

            if (rain > 0.2 && stormScore < 40) mode = "RAIN MODE";

            return mode;
        }
    }
}
