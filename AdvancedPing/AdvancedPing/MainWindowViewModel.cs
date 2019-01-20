using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AdvancedPing.Annotations;

namespace AdvancedPing
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _address;
        private ObservableCollection<string> _consoleOutput;
        private int _count;
        private int _delay;
        private long _completed;
        private double _averagePing;
        private long _maxPing;
        private long _minPing;
        private long _packetsLost;
        private double _packetsLostPercentage;
        private long _maxJitter;
        private double _averageJitter;

        public ObservableCollection<string> ConsoleOutput
        {
            get => _consoleOutput;
            set
            {
                if (Equals(value, _consoleOutput)) return;
                _consoleOutput = value;
                OnPropertyChanged();
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                if (value == _address) return;
                _address = value;
                OnPropertyChanged();
            }
        }

        public int Count
        {
            get => _count;
            set
            {
                if (value == _count) return;
                _count = value;
                OnPropertyChanged();
            }
        }

        public int Delay
        {
            get => _delay;
            set
            {
                if (value == _delay) return;
                _delay = value;
                OnPropertyChanged();
            }
        }

        public long Completed
        {
            get => _completed;
            set
            {
                if (value == _completed) return;
                _completed = value;
                OnPropertyChanged();
            }
        }

        public double AveragePing
        {
            get => _averagePing;
            set
            {
                if (value.Equals(_averagePing)) return;
                _averagePing = value;
                OnPropertyChanged();
            }
        }

        public long MaxPing
        {
            get => _maxPing;
            set
            {
                if (value == _maxPing) return;
                _maxPing = value;
                OnPropertyChanged();
            }
        }

        public long MinPing
        {
            get => _minPing;
            set
            {
                if (value == _minPing) return;
                _minPing = value;
                OnPropertyChanged();
            }
        }

        public long PacketsLost
        {
            get => _packetsLost;
            set
            {
                if (value == _packetsLost) return;
                _packetsLost = value;
                OnPropertyChanged();
            }
        }

        public double PacketsLostPercentage
        {
            get => _packetsLostPercentage;
            set
            {
                if (value.Equals(_packetsLostPercentage)) return;
                _packetsLostPercentage = value;
                OnPropertyChanged();
            }
        }

        public long MaxJitter
        {
            get => _maxJitter;
            set
            {
                if (value == _maxJitter) return;
                _maxJitter = value;
                OnPropertyChanged();
            }
        }

        public double AverageJitter
        {
            get => _averageJitter;
            set
            {
                if (value.Equals(_averageJitter)) return;
                _averageJitter = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            Count = 100;
            Delay = 500;
            Address = "192.168.1.1";
            ConsoleOutput = new ObservableCollection<string>{"Ready"};
        }

        public void UpdateFromResults(PingResults results)
        {
            Completed = results.Count;
            AveragePing = results.Average;
            MaxPing = results.Max;
            MinPing = results.Min;
            PacketsLost = results.PacketsLost;
            PacketsLostPercentage = results.PacketsLostPercent;
            MaxJitter = results.MaxJitter;
            AverageJitter = results.AverageJitter;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
