using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WeatherCockpit
{
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var window = new MainWindow();
            window.Activate();
        }
    }
}
