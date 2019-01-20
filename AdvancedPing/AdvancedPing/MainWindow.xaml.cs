using System;
using System.Diagnostics;
using System.Reflection;
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
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            Start.IsEnabled = true;
            Stop.IsEnabled = false;
            viewModel = new MainWindowViewModel();
            DataContext = viewModel;
        }

        private async void Start_OnClick(object sender, RoutedEventArgs e)
        {
            Start.IsEnabled = false;

            var address = viewModel.Address;
            if (string.IsNullOrWhiteSpace(address) ||
                viewModel.Delay < 1 || viewModel.Count < 1)
            {
                StatusText.Text = "Error, please check your inputs";
                Start.IsEnabled = true;
                return;
            }

            _currentPingManager = new PingManager(viewModel.Count, viewModel.Delay, address);

            //Attach OnPing event
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

            //Remove OnPing event to make sure garbage collector works correctly
            _currentPingManager.PingReceived -= PingManager_OnPing;

            Stop.IsEnabled = false;
            Start.IsEnabled = true;
        }

        private void AppendOutput(string newLine)
        {
            viewModel.ConsoleOutput.Add(newLine);
            ScrollViewer.ScrollToEnd();
        }

        private void PingManager_OnPing(object sender, PingEventArgs args)
        {
            AppendOutput($"Reply from {args.PingReply.Address} took {args.PingReply.RoundtripTime}ms");
            viewModel.UpdateFromResults(args.PingResults);
        }

        private void Stop_OnClick(object sender, RoutedEventArgs e)
        {
            if (_currentPingManager == null)
            {
                return;
            }

            _pingCancellationTokenSource.Cancel();
            _currentPingManager.CancelPings();
        }

        private void About_OnClick(object sender, RoutedEventArgs e)
        {
            var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            MessageBox.Show($"Advanced Ping v{version}.\nCreated by Andrew Bennett", "About", MessageBoxButton.OK);
        }
    }
}
