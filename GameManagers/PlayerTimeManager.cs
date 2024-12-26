using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace BoardGameFrontend.Managers
{
    public class PlayerTimeManager : INotifyPropertyChanged
    {
        private DispatcherTimer _timer;
        private TimeSpan _totalElapsedTime;
        private DateTime _lastUpdateTime; // Store the last update time
        
        public TimeSpan TotalElapsedTime
        {
            get => _totalElapsedTime;
            set
            {
                if (_totalElapsedTime != value)
                {
                    _totalElapsedTime = value;
                    OnPropertyChanged(nameof(TotalElapsedTime));
                }
            }
        }

        private TimeSpan _currentSessionTime;
        public TimeSpan CurrentSessionTime
        {
            get => _currentSessionTime;
            set
            {
                if (_currentSessionTime != value)
                {
                    _currentSessionTime = value;
                    OnPropertyChanged(nameof(CurrentSessionTime));
                }
            }
        }

        private bool _isRunning;

        public PlayerTimeManager()
        {
            _totalElapsedTime = TimeSpan.Zero;
            _currentSessionTime = TimeSpan.Zero;
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // Can be set lower for better accuracy
            };
            _timer.Tick += TimerTick;
        }

        public void StartTimer()
        {
            if (!_isRunning)
            {
                _timer.Start();
                _isRunning = true;
                CurrentSessionTime = TimeSpan.Zero;
                _lastUpdateTime = DateTime.Now; // Capture the start time
            }
        }

        public void StopTimer()
        {
            if (_isRunning)
            {
                _timer.Stop();
                _isRunning = false;

                DateTime currentTime = DateTime.Now;
                TimeSpan elapsedSinceLastTick = currentTime - _lastUpdateTime;
                CurrentSessionTime = CurrentSessionTime.Add(elapsedSinceLastTick);
                TotalElapsedTime = TotalElapsedTime.Add(CurrentSessionTime);
                CurrentSessionTime = TimeSpan.Zero;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddTime(TimeSpan timeSpan)
        {
            TotalElapsedTime = TotalElapsedTime.Add(timeSpan);
        }

        public TimeSpan GetTotalTime()
        {
            return TotalElapsedTime;
        }

        public TimeSpan GetCurrentSessionTime()
        {
            return CurrentSessionTime;
        }

        public string FormatElapsedTime(TimeSpan timeSpan)
        {
            if (timeSpan.TotalMinutes < 1)
            {
                return $"{(int)timeSpan.TotalSeconds}s";
            }
            else
            {
                return $"{timeSpan.TotalMinutes:F1}m";
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (_isRunning)
            {
                DateTime currentTime = DateTime.Now;
                TimeSpan elapsedSinceLastTick = currentTime - _lastUpdateTime;
                CurrentSessionTime = CurrentSessionTime.Add(elapsedSinceLastTick);
                _lastUpdateTime = currentTime;
            }
        }
    }
}
