using System.Collections.Generic;

namespace FactoryCalculator.Core.Models
{
    public sealed record Recipe
    {
        public string Id { get; init; } = System.Guid.NewGuid().ToString("D");
        public string Name { get; init; } = string.Empty;
        public List<RecipeIngredient> Inputs { get; init; } = new();
        public List<RecipeIngredient> Outputs { get; init; } = new();
        public string? Technology { get; init; }
        public string? MachineId { get; init; }
    }
}
