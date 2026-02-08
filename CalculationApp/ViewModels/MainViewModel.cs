using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CalculationApp.Models;

namespace CalculationApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _newName = string.Empty;
        private string _newPrice = string.Empty;
        private string _selectedCategory = "動画配信";
        private Subscription? _selectedSubscription;

        public ObservableCollection<Subscription> Subscriptions { get; } = new();

        public List<string> Categories { get; } = new()
        {
            "動画配信",
            "音楽配信",
            "ゲーム",
            "クラウド",
            "その他"
        };

        public string NewName
        {
            get => _newName;
            set { _newName = value; OnPropertyChanged(); }
        }

        public string NewPrice
        {
            get => _newPrice;
            set { _newPrice = value; OnPropertyChanged(); }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); }
        }

        public Subscription? SelectedSubscription
        {
            get => _selectedSubscription;
            set { _selectedSubscription = value; OnPropertyChanged(); }
        }

        public int MonthlyTotal => Subscriptions.Sum(s => s.MonthlyPrice);
        public int YearlyTotal => MonthlyTotal * 12;
        public string DailyAmount => $"{MonthlyTotal * 12 / 365.0:F1}";

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }

        public MainViewModel()
        {
            AddCommand = new RelayCommand(ExecuteAdd, CanExecuteAdd);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteDelete);
        }

        private bool CanExecuteAdd(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(NewName)
                && int.TryParse(NewPrice, out int price)
                && price > 0;
        }

        private void ExecuteAdd(object? parameter)
        {
            var subscription = new Subscription
            {
                Name = NewName.Trim(),
                MonthlyPrice = int.Parse(NewPrice),
                Category = SelectedCategory
            };

            Subscriptions.Add(subscription);

            NewName = string.Empty;
            NewPrice = string.Empty;
            SelectedCategory = "動画配信";

            UpdateTotals();
        }

        private bool CanExecuteDelete(object? parameter)
        {
            return SelectedSubscription != null;
        }

        private void ExecuteDelete(object? parameter)
        {
            if (SelectedSubscription != null)
            {
                Subscriptions.Remove(SelectedSubscription);
                SelectedSubscription = null;
                UpdateTotals();
            }
        }

        private void UpdateTotals()
        {
            OnPropertyChanged(nameof(MonthlyTotal));
            OnPropertyChanged(nameof(YearlyTotal));
            OnPropertyChanged(nameof(DailyAmount));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
