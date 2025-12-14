using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;

namespace FactoryCalculator.ViewModels
{
    public sealed class GameSetupViewModel : INotifyPropertyChanged
    {
        private static readonly JsonSerializerOptions s_jsonOptions = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = true
        };

        private string _configPath = string.Empty;
        private string _gameName = "New Game";
        private FactoryCalculator.Core.UnitRate _unitRate = FactoryCalculator.Core.UnitRate.PerMinute;
        private string _serializedJson = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

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

        public FactoryCalculator.Core.UnitRate UnitRate
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

        public GameSetupViewModel()
        {
            UpdateSerializedJson();
        }

        public async Task LoadAsync()
        {
            if (string.IsNullOrWhiteSpace(ConfigPath))
            {
                throw new InvalidOperationException("ConfigPath must be set before loading.");
            }

            // Capture the UI dispatcher while we're still on the UI thread so we can marshal updates back.
            var uiDispatcher = DispatcherQueue.GetForCurrentThread();

            var profile = await FactoryCalculator.Core.ConfigStore.LoadAsync(ConfigPath).ConfigureAwait(false);
            if (profile is null)
            {
                var defaultProfile = new FactoryCalculator.Core.GameProfile
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

            // Apply the loaded values on the UI thread to avoid raising PropertyChanged across threads.
            void ApplyProfile()
            {
                GameName = profile.GameName;
                UnitRate = profile.UnitRate;
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

            // Capture UI dispatcher in case we resume on a background thread.
            var uiDispatcher = DispatcherQueue.GetForCurrentThread();

            var profile = new FactoryCalculator.Core.GameProfile
            {
                GameName = GameName,
                UnitRate = UnitRate
            };

            await FactoryCalculator.Core.ConfigStore.SaveAsync(profile, ConfigPath).ConfigureAwait(false);

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

        private void UpdateSerializedJson()
        {
            var profile = new FactoryCalculator.Core.GameProfile
            {
                GameName = GameName,
                UnitRate = UnitRate
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
            if (propertyName == nameof(GameName) || propertyName == nameof(UnitRate))
            {
                UpdateSerializedJson();
            }
        }
    }
}

