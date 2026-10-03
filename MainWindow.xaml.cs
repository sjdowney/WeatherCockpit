using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;

namespace WeatherCockpit
{
    public sealed partial class MainWindow : Window
    {
        public CockpitViewModel ViewModel { get; } = new();

        public MainWindow()
        {
            InitializeComponent();

            ScrollText.Opacity = 0;
            StatusBox.Loaded += StatusBox_Loaded;

            MaximizeWindow();
            Root.DataContext = ViewModel;
            Title = "Weather Cockpit";

            _ = RefreshAsync();

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(3)
            };
            timer.Tick += async (_, __) => await RefreshAsync();
            timer.Start();
        }

        private void MaximizeWindow()
        {
            var presenter = AppWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
            presenter?.Maximize();
        }

        private void StatusBox_Loaded(object sender, RoutedEventArgs e)
        {
            var visual = ElementCompositionPreview.GetElementVisual(StatusBox);
            visual.Clip = visual.Compositor.CreateInsetClip(0, 0, 0, 0);

            ScrollText.SizeChanged += (_, __) =>
            {
                ScrollCanvas.Width = ScrollText.ActualWidth;
                ScrollCanvas.UpdateLayout();
                StartScroll();
                ScrollText.Opacity = 1;
            };
        }

        private void StartScroll()
        {
            var canvasVisual = ElementCompositionPreview.GetElementVisual(ScrollCanvas);
            var compositor = canvasVisual.Compositor;

            float maskWidth = (float)StatusBox.ActualWidth;
            float textWidth = (float)ScrollText.ActualWidth;

            var animation = compositor.CreateScalarKeyFrameAnimation();
            var linear = compositor.CreateLinearEasingFunction();

            animation.InsertKeyFrame(0f, maskWidth, linear);
            animation.InsertKeyFrame(1f, -textWidth - 20, linear);
            animation.Duration = TimeSpan.FromSeconds(10);
            animation.IterationBehavior = Microsoft.UI.Composition.AnimationIterationBehavior.Forever;

            canvasVisual.StartAnimation("Offset.X", animation);
        }

        private async Task RefreshAsync()
        {
            var data = await WeatherService.GetLiveAsync(ViewModel);
            await TideService.GetLiveAsync(ViewModel);

            RenderTokens(ViewModel.StatusTokens);
            ViewModel.Update(data);

            RotateNeedle(data.WindDirectionDegrees);
        }

        private void RotateNeedle(double angle)
        {
            var visual = ElementCompositionPreview.GetElementVisual(CompassNeedle);
            var compositor = visual.Compositor;

            visual.CenterPoint = new System.Numerics.Vector3(
                (float)CompassNeedle.Width / 2,
                (float)CompassNeedle.Height / 2,
                0f);

            var animation = compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(1.0f, (float)angle);
            animation.Duration = TimeSpan.FromMilliseconds(500);

            visual.StartAnimation("RotationAngleInDegrees", animation);
        }

        private void RenderTokens(List<MessageToken> tokens)
        {
            ScrollText.Inlines.Clear();

            bool bold = false;
            bool italic = false;
            Brush currentBrush = new SolidColorBrush(Colors.Cyan);

            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.BoldOn:
                        bold = true;
                        break;

                    case TokenType.BoldOff:
                        bold = false;
                        break;

                    case TokenType.ItalicOn:
                        italic = true;
                        break;

                    case TokenType.ItalicOff:
                        italic = false;
                        break;

                    case TokenType.Color:
                        currentBrush = new SolidColorBrush(ParseColor(token.Value));
                        break;

                    case TokenType.Text:
                        var run = new Run
                        {
                            Text = token.Value,
                            Foreground = currentBrush,
                            FontWeight = bold ? FontWeights.Bold : FontWeights.Normal,
                            FontStyle = italic ? FontStyle.Italic : FontStyle.Normal
                        };
                        ScrollText.Inlines.Add(run);
                        break;
                }
            }

            ScrollText.UpdateLayout();
            ScrollCanvas.Width = ScrollText.ActualWidth;
            ScrollCanvas.UpdateLayout();
        }

        private Color ParseColor(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Colors.Cyan;

            if (value.StartsWith("#") && value.Length == 7)
            {
                byte r = Convert.ToByte(value.Substring(1, 2), 16);
                byte g = Convert.ToByte(value.Substring(3, 2), 16);
                byte b = Convert.ToByte(value.Substring(5, 2), 16);
                return Color.FromArgb(255, r, g, b);
            }

            return Colors.Cyan;
        }
    }
}
