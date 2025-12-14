using FactoryCalculator.Core;

namespace FactoryCalculator.Core.Models
{
    public sealed record Item
    {
        public string Id { get; init; } = System.Guid.NewGuid().ToString("D");
        public string Name { get; init; } = string.Empty;

        // Changed to string so item types can be defined per-game (matches GameProfile.IngredientTypes)
        public string Type { get; init; } = "Solid";

        public decimal? Cost { get; init; }
    }
}
