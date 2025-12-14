using System;
using System.Linq;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using FactoryCalculator.ViewModels;

namespace FactoryCalculator.Views
{
    public sealed partial class GameSetupPage : Page
    {
        private readonly GameSetupViewModel _viewModel;

        public GameSetupPage()
        {
            InitializeComponent();
            _viewModel = new GameSetupViewModel();
            DataContext = _viewModel;

            // Populate ComboBox with enum values so SelectedItem binds to UnitRate (enum)
            UnitRateComboBox.ItemsSource = Enum.GetValues(typeof(FactoryCalculator.Core.UnitRate)).Cast<FactoryCalculator.Core.UnitRate>();
        }

        private async void OnLoadClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                var pathBefore = _viewModel.ConfigPath;
                System.Diagnostics.Debug.WriteLine($"[GameSetup] Loading config from: {pathBefore}");

                await _viewModel.LoadAsync();

                DispatcherQueue.TryEnqueue(() => { /* no-op - bindings update via INotifyPropertyChanged */ });
            }
            catch (Exception ex)
            {
                var details = $"{ex.GetType().FullName} HResult=0x{ex.HResult:X8}\n{ex}";
                System.Diagnostics.Debug.WriteLine($"[GameSetup] Load failed: {details}");
                DispatcherQueue.TryEnqueue(async () => await ShowMessageAsync($"Load failed:\n{details}"));
            }
        }

        private async void OnSaveClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                await _viewModel.SaveAsync().ConfigureAwait(false);
                DispatcherQueue.TryEnqueue(async () => await ShowMessageAsync("Saved successfully."));
            }
            catch (Exception ex)
            {
                var details = $"{ex.GetType().FullName} HResult=0x{ex.HResult:X8}\n{ex}";
                System.Diagnostics.Debug.WriteLine($"[GameSetup] Save failed: {details}");
                DispatcherQueue.TryEnqueue(async () => await ShowMessageAsync($"Save failed:\n{details}"));
            }
        }

        private async Task ShowMessageAsync(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "FactoryCalculator",
                Content = message,
                CloseButtonText = "OK"
            };

            dialog.XamlRoot = this.XamlRoot;
            await dialog.ShowAsync();
        }
    }
}

