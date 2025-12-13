using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace FactoryCalculator.Core
{
    public static class ConfigStore
    {
        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public static async Task SaveAsync(GameProfile profile, string path)
        {
            var json = JsonSerializer.Serialize(profile, Options);
            await File.WriteAllTextAsync(path, json).ConfigureAwait(false);
        }

        public static async Task<GameProfile?> LoadAsync(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var json = await File.ReadAllTextAsync(path).ConfigureAwait(false);
            return JsonSerializer.Deserialize<GameProfile>(json, Options);
        }
    }
}