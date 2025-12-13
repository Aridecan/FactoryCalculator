using System;
using System.IO;
using System.Threading.Tasks;
using FactoryCalculator.Core;
using FactoryCalculator.Core.Models;
using Xunit;

namespace FactoryCalculator.Core.Tests
{
    public class ConfigStoreTests : IDisposable
    {
        private readonly string _tempFile;

        public ConfigStoreTests()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), $"gameprofile_{Guid.NewGuid():N}.json");
        }

        public void Dispose()
        {
            if (File.Exists(_tempFile))
                File.Delete(_tempFile);
        }

        [Fact]
        public async Task SaveAndLoad_PreservesProfile()
        {
            var profile = new GameProfile
            {
                GameName = "Test Game",
                UnitRate = UnitRate.PerMinute
            };

            // Items is a mutable List<T> instance on the profile; add an item to verify round-trip
            profile.Items.Add(new Item { Name = "Iron Ore" });

            await ConfigStore.SaveAsync(profile, _tempFile);

            var loaded = await ConfigStore.LoadAsync(_tempFile);

            Assert.NotNull(loaded);
            Assert.Equal(profile.GameName, loaded!.GameName);
            Assert.Equal(profile.UnitRate, loaded.UnitRate);
            Assert.Single(loaded.Items);
            Assert.Equal("Iron Ore", loaded.Items[0].Name);
        }
    }
}
