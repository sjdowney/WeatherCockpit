using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WeatherCockpit
{
    public class CockpitViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // -----------------------------
        //  DISPLAY PROPERTIES
        // -----------------------------

        public string LiveLabel { get; private set; } = "Live";
        public string TempDisplay { get; private set; } = "-- °C";
        public string HumidityDisplay { get; private set; } = "-- %";
        public string PressureDisplay { get; private set; } = "-- hPa";

        public string StormLevel { get; private set; } = "Loading...";
        public string PressureDriver { get; private set; } = "";
        public string HumidityDriver { get; private set; } = "";
        public string WindDriver { get; private set; } = "";
        public string InstabilityDriver { get; private set; } = "";
        public string DpdDriver { get; private set; } = "";

        public string ModeLabel { get; private set; } = "";
        public string ForecastSummary { get; private set; } = "";

        public string StripTemp { get; private set; } = "";
        public string StripHum { get; private set; } = "";
        public string StripPress { get; private set; } = "";
        public string StripWind { get; private set; } = "";
        public string StripRain { get; private set; } = "";

        public string MoonPhaseImage { get; private set; } = "";
        public string MoonPhaseText { get; private set; } = "";
        public string MoonRise { get; private set; } = "";
        public string MoonSet { get; private set; } = "";
        public string LocalForecast { get; private set; } = "";

        public double StormLightningDistance { get; private set; }
        public double StormLightningIntensity { get; private set; }

        public string HighTideTime { get; set; } = "";
        public double HighTide { get; set; }
        public string LowTideTime { get; set; } = "";
        public double LowTide { get; set; }

        // -----------------------------
        //  STATUS SYSTEM
        // -----------------------------

        private List<MessageToken> _statusTokens = new();
        public List<MessageToken> StatusTokens
        {
            get => _statusTokens;
            private set
            {
                _statusTokens = value;
                OnPropertyChanged(nameof(StatusTokens));
            }
        }

        private string _statusMessage = "";
        public string StatusMessage
        {
            get => _statusMessage;
            private set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged(nameof(StatusMessage));
                }
            }
        }

        // -----------------------------
        //  CONSTRUCTOR
        // -----------------------------

        public CockpitViewModel()
        {
            SetStatus("OK!");
        }

        // -----------------------------
        //  STATUS UPDATE
        // -----------------------------

        public void SetStatus(string raw)
        {
            StatusTokens = MessageParser.Parse(raw);
            StatusMessage = MessageParser.StripCodes(raw);
        }

        // -----------------------------
        //  WEATHER UPDATE
        // -----------------------------

        public void Update(WeatherData d)
        {
            int stormScore = StormEngine.ComputeStormScore(d);
            var sig = StormEngine.BuildSignature(d, stormScore);

            ForecastSummary = ForecastEngine.BuildForecastSummary(d, stormScore);
            ModeLabel = ForecastEngine.GetModeLabel(stormScore, d.Rain);

            TempDisplay = $"{d.Temp:F1} °C";
            HumidityDisplay = $"{d.Humidity:F1} %";
            PressureDisplay = $"{d.Pressure:F1} hPa";

            StormLevel = sig.Level;
            PressureDriver = sig.PressureDriver;
            HumidityDriver = sig.HumidityDriver;
            WindDriver = sig.WindDriver;
            InstabilityDriver = sig.InstabilityDriver;
            DpdDriver = sig.DpdDriver;

            StripTemp = d.Temp.ToString("F1");
            StripHum = d.Humidity.ToString("F1");
            StripPress = d.Pressure.ToString("F1");
            StripWind = $"{d.WindDirText} {d.WindSpeed:F1}";
            StripRain = d.Rain.ToString("F1");

            LiveLabel = $"Live · {d.LastRefresh:ddd dd MMM yyyy, h:mm tt}";

            MoonPhaseText = d.MoonPhase;
            MoonRise = d.MoonRise;
            MoonSet = d.MoonSet;
            LocalForecast = ForecastSummary;

            StormLightningDistance = d.StormLightningDistance;
            StormLightningIntensity = d.StormLightningIntensity; // FIXED BUG

            var phaseKey = d.MoonPhase.ToLower().Replace(" ", "_");
            MoonPhaseImage = $"ms-appx:///Assets/Images/moon/moon_{phaseKey}.png";

            OnPropertyChanged(null); // refresh all bindings
        }

        // -----------------------------
        //  LIGHTNING DISPLAY
        // -----------------------------

        public string LightningActivityDisplay
        {
            get
            {
                int distance = (int)StormLightningDistance;
                int intensity = (int)StormLightningIntensity;

                if (intensity <= 0 || distance <= 0)
                    return "No lightning detected";

                string strength = intensity switch
                {
                    <= 50 => "Weak",
                    <= 120 => "Moderate",
                    <= 200 => "Strong",
                    _ => "Severe"
                };

                string range = distance switch
                {
                    <= 5 => "Very close",
                    <= 15 => "Nearby",
                    <= 30 => "Distant",
                    _ => "Far"
                };

                return $"{strength} strike ({distance} km)";
            }
        }
    }
}
