using System.Collections.Generic;

namespace FactoryCalculator.Core.Models
{
    public sealed record Machine
    {
        public string Id { get; init; } = System.Guid.NewGuid().ToString("D");
        public string Name { get; init; } = string.Empty;
        public List<RecipeIngredient> Inputs { get; init; } = new();
        public List<RecipeIngredient> Outputs { get; init; } = new();
        public List<string> FunctionRequirements { get; init; } = new(); // e.g. "150 kW/s"
    }
}