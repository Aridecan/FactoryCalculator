using System;
using System.Linq;
using System.Threading.Tasks;
using FactoryCalculator.ViewModels;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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

        private async void OnAddIngredientClicked(object sender, RoutedEventArgs e)
        {
            _viewModel.AddIngredientType();
        }

        private async void OnRemoveIngredientClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.Tag is string tag)
            {
                _viewModel.RemoveIngredientType(tag);
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

