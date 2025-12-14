using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using FactoryCalculator.Core;
using FactoryCalculator.Core.Models;
using Microsoft.UI.Dispatching;

namespace FactoryCalculator.ViewModels
{
    public sealed class RecipeIngredientVm
    {
        public string ItemId { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
    }

    public sealed class MachineItemVm : INotifyPropertyChanged
    {
        private string _id = Guid.NewGuid().ToString("D");
        private string _name = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public ObservableCollection<RecipeIngredientVm> Inputs { get; } = new();
        public ObservableCollection<RecipeIngredientVm> Outputs { get; } = new();
        public ObservableCollection<string> FunctionRequirements { get; } = new();

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Machine ToModel()
        {
            return new Machine
            {
                Id = Id,
                Name = Name,
                Inputs = Inputs.Select(i => new RecipeIngredient { ItemId = i.ItemId, Quantity = i.Quantity }).ToList(),
                Outputs = Outputs.Select(o => new RecipeIngredient { ItemId = o.ItemId, Quantity = o.Quantity }).ToList(),
                FunctionRequirements = FunctionRequirements.ToList()
            };
        }

        public static MachineItemVm FromModel(Machine m)
        {
            var vm = new MachineItemVm
            {
                Id = m.Id,
                Name = m.Name
            };

            if (m.Inputs is not null)
            {
                foreach (var i in m.Inputs)
                {
                    vm.Inputs.Add(new RecipeIngredientVm { ItemId = i.ItemId, Quantity = i.Quantity });
                }
            }

            if (m.Outputs is not null)
            {
                foreach (var o in m.Outputs)
                {
                    vm.Outputs.Add(new RecipeIngredientVm { ItemId = o.ItemId, Quantity = o.Quantity });
                }
            }

            if (m.FunctionRequirements is not null)
            {
                foreach (var f in m.FunctionRequirements)
                {
                    vm.FunctionRequirements.Add(f);
                }
            }

            return vm;
        }
    }

    public sealed class MachineViewModel : INotifyPropertyChanged
    {
        private string _configPath = string.Empty;
        private MachineItemVm? _selected;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<MachineItemVm> Machines { get; } = new();

        public MachineItemVm? SelectedMachine
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public string ConfigPath
        {
            get => _configPath;
            set => SetProperty(ref _configPath, value);
        }

        // Commands
        public ICommand LoadCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand AddMachineCommand { get; }
        public ICommand RemoveMachineCommand { get; }
        public ICommand AddInputCommand { get; }
        public ICommand RemoveInputCommand { get; }
        public ICommand AddOutputCommand { get; }
        public ICommand RemoveOutputCommand { get; }
        public ICommand AddFunctionReqCommand { get; }
        public ICommand RemoveFunctionReqCommand { get; }

        public MachineViewModel()
        {
            LoadCommand = new AsyncRelayCommand(_ => LoadAsync());
            SaveCommand = new AsyncRelayCommand(_ => SaveAsync());
            AddMachineCommand = new RelayCommand(p => AddMachine(p as string ?? string.Empty));
            RemoveMachineCommand = new RelayCommand(_ => RemoveSelectedMachine());
            AddInputCommand = new RelayCommand(p =>
            {
                if (p is Tuple<string, decimal> t) AddInput(t.Item1, t.Item2);
            });
            RemoveInputCommand = new RelayCommand(p =>
            {
                if (p is RecipeIngredientVm r) RemoveInput(r);
            });
            AddOutputCommand = new RelayCommand(p =>
            {
                if (p is Tuple<string, decimal> t) AddOutput(t.Item1, t.Item2);
            });
            RemoveOutputCommand = new RelayCommand(p =>
            {
                if (p is RecipeIngredientVm r) RemoveOutput(r);
            });
            AddFunctionReqCommand = new RelayCommand(p => { if (p is string s) AddFunctionRequirement(s); });
            RemoveFunctionReqCommand = new RelayCommand(p => { if (p is string s) RemoveFunctionRequirement(s); });
        }

        public async Task LoadAsync()
        {
            if (string.IsNullOrWhiteSpace(ConfigPath))
            {
                throw new InvalidOperationException("ConfigPath must be set before loading.");
            }

            var uiDispatcher = DispatcherQueue.GetForCurrentThread();
            var profile = await ConfigStore.LoadAsync(ConfigPath).ConfigureAwait(false);

            void Apply(GameProfile p)
            {
                Machines.Clear();
                if (p?.Machines is not null)
                {
                    foreach (var m in p.Machines)
                    {
                        Machines.Add(MachineItemVm.FromModel(m));
                    }
                }

                SelectedMachine = Machines.FirstOrDefault();
            }

            if (profile is null)
            {
                Apply(new GameProfile());
                return;
            }

            if (uiDispatcher is not null)
            {
                uiDispatcher.TryEnqueue(() => Apply(profile));
            }
            else
            {
                Apply(profile);
            }
        }

        public async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(ConfigPath))
            {
                throw new InvalidOperationException("ConfigPath must be set before saving.");
            }

            var profile = await ConfigStore.LoadAsync(ConfigPath).ConfigureAwait(false) ?? new GameProfile();

            profile = profile with
            {
                Machines = Machines.Select(m => m.ToModel()).ToList()
            };

            await ConfigStore.SaveAsync(profile, ConfigPath).ConfigureAwait(false);
        }

        public void AddMachine(string name)
        {
            var m = new MachineItemVm { Name = string.IsNullOrWhiteSpace(name) ? $"Machine {Machines.Count + 1}" : name.Trim() };
            Machines.Add(m);
            SelectedMachine = m;
        }

        public void RemoveSelectedMachine()
        {
            if (SelectedMachine is null)
            {
                return;
            }

            Machines.Remove(SelectedMachine);
            SelectedMachine = Machines.FirstOrDefault();
        }

        public void AddInput(string itemId, decimal quantity)
        {
            if (SelectedMachine is null)
            {
                return;
            }

            SelectedMachine.Inputs.Add(new RecipeIngredientVm { ItemId = itemId ?? string.Empty, Quantity = quantity });
        }

        public void RemoveInput(RecipeIngredientVm? ingredient)
        {
            if (SelectedMachine is null || ingredient is null)
            {
                return;
            }

            SelectedMachine.Inputs.Remove(ingredient);
        }

        public void AddOutput(string itemId, decimal quantity)
        {
            if (SelectedMachine is null)
            {
                return;
            }

            SelectedMachine.Outputs.Add(new RecipeIngredientVm { ItemId = itemId ?? string.Empty, Quantity = quantity });
        }

        public void RemoveOutput(RecipeIngredientVm? ingredient)
        {
            if (SelectedMachine is null || ingredient is null)
            {
                return;
            }

            SelectedMachine.Outputs.Remove(ingredient);
        }

        public void AddFunctionRequirement(string req)
        {
            if (SelectedMachine is null || string.IsNullOrWhiteSpace(req))
            {
                return;
            }

            SelectedMachine.FunctionRequirements.Add(req.Trim());
        }

        public void RemoveFunctionRequirement(string? req)
        {
            if (SelectedMachine is null || req is null)
            {
                return;
            }

            SelectedMachine.FunctionRequirements.Remove(req);
        }

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value))
            {
                return;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
