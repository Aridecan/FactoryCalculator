using FactoryCalculator.Core;

namespace FactoryCalculator.Core.Models
{
    public sealed record Item
    {
        public string Id { get; init; } = System.Guid.NewGuid().ToString("D");
        public string Name { get; init; } = string.Empty;
        public IngredientType Type { get; init; } = IngredientType.Solid;
        public decimal? Cost { get; init; }
    }
}
