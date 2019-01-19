using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AdvancedPing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PingManager _currentPingManager;
        private CancellationTokenSource _pingCancellationTokenSource;
        private StringBuilder _consoleOutput = new StringBuilder();

        public MainWindow()
        {
            InitializeComponent();
            Start.IsEnabled = true;
            Stop.IsEnabled = false;
        }

        private async void Start_OnClick(object sender, RoutedEventArgs e)
        {
            Start.IsEnabled = false;

            var address = Address.Text;

            if (!int.TryParse(Delay.Text, out var delay) ||
                !int.TryParse(Count.Text, out var count) ||
                string.IsNullOrWhiteSpace(address) ||
                delay < 1 || count < 1)
            {
                StatusText.Text = "Error, please check your inputs";
                Start.IsEnabled = true;
                return;
            }

            _currentPingManager = new PingManager(count, delay, address);
            _currentPingManager.PingReceived += PingManager_OnPing;
            AppendOutput("Beginning ping...");

            Stop.IsEnabled = true;

            try
            {
                _pingCancellationTokenSource = new CancellationTokenSource();
                await _currentPingManager.StartPing(_pingCancellationTokenSource.Token);
                AppendOutput("Finished!");
            }
            catch (TaskCanceledException)
            {
                AppendOutput("Cancelled!");
            }

            _currentPingManager.PingReceived -= PingManager_OnPing;
            Stop.IsEnabled = false;
            Start.IsEnabled = true;
        }

        private void AppendOutput(string newLine)
        {
            _consoleOutput.AppendLine(newLine);
            Output.Text = _consoleOutput.ToString();
            ScrollViewer.ScrollToEnd();
        }

        private void PingManager_OnPing(object sender, PingEventArgs args)
        {
            AppendOutput($"Reply from {args.PingReply.Address} took {args.PingReply.RoundtripTime}ms");
            UpdateResults(args.PingResults);
        }

        private void UpdateResults(PingResults results)
        {
            Max.Text = results.Max.ToString();
            Min.Text = results.Min.ToString();
            Completed.Text = results.Count.ToString();
            Average.Text = results.Average.ToString("0.0");
            MaxJitter.Text = results.MaxJitter.ToString();
            AverageJitter.Text = results.AverageJitter.ToString("0.0");
            PacketLoss.Text = results.PacketsLost.ToString();
            PacketLossPercent.Text = results.PacketsLostPercent.ToString("0.0");
        }

        private void Stop_OnClick(object sender, RoutedEventArgs e)
        {
            if (_currentPingManager != null)
            {
                _pingCancellationTokenSource.Cancel();
                _currentPingManager.CancelPings();
            }
        }

        private void About_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
