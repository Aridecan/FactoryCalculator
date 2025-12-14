using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using FactoryCalculator.Core;

namespace FactoryCalculator.ViewModels
{
    public sealed class UnitRateOption
    {
        public UnitRate Value { get; init; }
        public string Display { get; init; } = string.Empty;
    }

    public sealed class GameSetupViewModel : INotifyPropertyChanged
    {
        private static readonly JsonSerializerOptions s_jsonOptions = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = true
        };

        private string _configPath = string.Empty;
        private string _gameName = "New Game";
        private UnitRate _unitRate = UnitRate.PerMinute;
        private string _serializedJson = string.Empty;
        private string _newIngredientType = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<string> IngredientTypes { get; } = new();

        public ObservableCollection<UnitRateOption> UnitRateOptions { get; } = new()
        {
            new UnitRateOption { Value = UnitRate.PerSecond, Display = "per second" },
            new UnitRateOption { Value = UnitRate.PerMinute, Display = "per minute" },
            new UnitRateOption { Value = UnitRate.PerHour, Display = "per hour" }
        };

        // Commands
        public ICommand LoadCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand AddIngredientCommand { get; }
        public ICommand RemoveIngredientCommand { get; }

        public string ConfigPath
        {
            get => _configPath;
            set => SetProperty(ref _configPath, value);
        }

        public string GameName
        {
            get => _gameName;
            set => SetProperty(ref _gameName, value);
        }

        public UnitRate UnitRate
        {
            get => _unitRate;
            set => SetProperty(ref _unitRate, value);
        }

        /// <summary>
        /// A read-only JSON preview of the current profile.
        /// </summary>
        public string SerializedJson
        {
            get => _serializedJson;
            private set => SetProperty(ref _serializedJson, value);
        }

        public string NewIngredientType
        {
            get => _newIngredientType;
            set => SetProperty(ref _newIngredientType, value);
        }

        public GameSetupViewModel()
        {
            // populate defaults
            IngredientTypes.Add("Solid");
            IngredientTypes.Add("Liquid");
            IngredientTypes.Add("Gas");

            // wire commands
            LoadCommand = new AsyncRelayCommand(_ => LoadAsync());
            SaveCommand = new AsyncRelayCommand(_ => SaveAsync());
            AddIngredientCommand = new RelayCommand(_ => AddIngredientType());
            RemoveIngredientCommand = new RelayCommand(p => RemoveIngredientType(p as string));

            UpdateSerializedJson();
        }

        public async Task LoadAsync()
        {
            if (string.IsNullOrWhiteSpace(ConfigPath))
            {
                throw new InvalidOperationException("ConfigPath must be set before loading.");
            }

            var uiDispatcher = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

            var profile = await ConfigStore.LoadAsync(ConfigPath).ConfigureAwait(false);
            if (profile is null)
            {
                var defaultProfile = new GameProfile
                {
                    GameName = _gameName,
                    UnitRate = _unitRate
                };

                var json = JsonSerializer.Serialize(defaultProfile, s_jsonOptions);
                if (uiDispatcher is not null)
                {
                    uiDispatcher.TryEnqueue(() => SerializedJson = json);
                }
                else
                {
                    SerializedJson = json;
                }

                return;
            }

            void ApplyProfile()
            {
                GameName = profile.GameName;
                UnitRate = profile.UnitRate;

                IngredientTypes.Clear();
                if (profile.IngredientTypes is not null)
                {
                    foreach (var t in profile.IngredientTypes)
                    {
                        IngredientTypes.Add(t);
                    }
                }

                SerializedJson = JsonSerializer.Serialize(profile, s_jsonOptions);
            }

            if (uiDispatcher is not null)
            {
                uiDispatcher.TryEnqueue(ApplyProfile);
            }
            else
            {
                ApplyProfile();
            }
        }

        public async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(ConfigPath))
            {
                throw new InvalidOperationException("ConfigPath must be set before saving.");
            }

            var uiDispatcher = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

            var profile = new GameProfile
            {
                GameName = GameName,
                UnitRate = UnitRate,
                IngredientTypes = new System.Collections.Generic.List<string>(IngredientTypes)
            };

            await ConfigStore.SaveAsync(profile, ConfigPath).ConfigureAwait(false);

            var json = JsonSerializer.Serialize(profile, s_jsonOptions);
            if (uiDispatcher is not null)
            {
                uiDispatcher.TryEnqueue(() => SerializedJson = json);
            }
            else
            {
                SerializedJson = json;
            }
        }

        public void AddIngredientType()
        {
            var candidate = (NewIngredientType ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(candidate))
            {
                return;
            }

            if (!IngredientTypes.Any(t => string.Equals(t, candidate, StringComparison.OrdinalIgnoreCase)))
            {
                IngredientTypes.Add(candidate);
            }

            NewIngredientType = string.Empty;
        }

        public void RemoveIngredientType(string? type)
        {
            if (type is null)
            {
                return;
            }

            IngredientTypes.Remove(type);
        }

        private void UpdateSerializedJson()
        {
            var profile = new GameProfile
            {
                GameName = GameName,
                UnitRate = UnitRate,
                IngredientTypes = new System.Collections.Generic.List<string>(IngredientTypes)
            };

            SerializedJson = JsonSerializer.Serialize(profile, s_jsonOptions);
        }

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value))
            {
                return;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == nameof(GameName) || propertyName == nameof(UnitRate) || propertyName == nameof(IngredientTypes))
            {
                UpdateSerializedJson();
            }
        }
    }
}
