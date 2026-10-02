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
        //  EXISTING VIEWMODEL PROPERTIES
        // -----------------------------

        public string LiveLabel { get; private set; } = "Live";
        public string TempDisplay { get; private set; } = "";
        public string HumidityDisplay { get; private set; } = "";
        public string PressureDisplay { get; private set; } = "";

        public string StormLevel { get; private set; } = "";
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

        // -----------------------------
        //  STATUS SYSTEM (CLEAN VERSION)
        // -----------------------------

        // Parsed tokens (used by MainWindow.RenderTokens)
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

        // Clean final text (bound to XAML)
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
            TempDisplay = "-- °C";
            HumidityDisplay = "-- %";
            PressureDisplay = "-- hPa";
            StormLevel = "Loading...";

            // Initial status
            SetStatus("OK!");
        }

        // -----------------------------
        //  STATUS UPDATE ENTRY POINT
        // -----------------------------

        public void SetStatus(string raw)
        {
            // Parse control codes → tokens
            StatusTokens = MessageParser.Parse(raw);
            Console.WriteLine(StatusTokens.ToString());
            // Strip codes → clean text for binding
            //StatusMessage = MessageParser.StripCodes(raw);
        }

        // -----------------------------
        //  WEATHER UPDATE
        // -----------------------------

        public void Update(WeatherData d)
        {
            int stormScore = StormEngine.ComputeStormScore(d);
            var sig = StormEngine.BuildSignature(d, stormScore);
            var forecast = ForecastEngine.BuildForecastSummary(d, stormScore);
            var mode = ForecastEngine.GetModeLabel(stormScore, d.Rain);

            TempDisplay = $"{d.Temp:F1} °C";
            HumidityDisplay = $"{d.Humidity:F1} %";
            PressureDisplay = $"{d.Pressure:F1} hPa";

            StormLevel = sig.Level;
            PressureDriver = sig.PressureDriver;
            HumidityDriver = sig.HumidityDriver;
            WindDriver = sig.WindDriver;
            InstabilityDriver = sig.InstabilityDriver;
            DpdDriver = sig.DpdDriver;

            ModeLabel = mode;
            ForecastSummary = forecast;

            StripTemp = d.Temp.ToString("F1");
            StripHum = d.Humidity.ToString("F1");
            StripPress = d.Pressure.ToString("F1");
            StripWind = $"{d.WindDirText} {d.WindSpeed:F1}";
            StripRain = d.Rain.ToString("F1");

            LiveLabel = $"Live · {d.LastRefresh:ddd dd MMM yyyy, h:mm tt}";

            MoonPhaseText = d.MoonPhase;
            MoonRise = d.MoonRise;
            MoonSet = d.MoonSet;
            LocalForecast = forecast;

            StormLightningDistance = d.StormLightningDistance;
            StormLightningIntensity = d.StormLightningDistance;

            var phaseKey = d.MoonPhase.ToLower().Replace(" ", "_");
            MoonPhaseImage = $"ms-appx:///Assets/Images/moon/moon_{phaseKey}.png";

            OnPropertyChanged(null);
        }

        public string LightningActivityDisplay
        {
            get
            {
                int distance = ((int)StormLightningDistance);   // km
                int intensity = ((int)StormLightningIntensity); // 0–255

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
