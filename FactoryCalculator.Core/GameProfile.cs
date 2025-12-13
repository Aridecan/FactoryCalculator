using System.Collections.Generic;

namespace FactoryCalculator.Core
{
    public sealed record GameProfile
    {
        public string GameName { get; init; } = "New Game";
        public List<IngredientType> IngredientTypes { get; init; } = new() { IngredientType.Solid, IngredientType.Liquid, IngredientType.Gas };
        public UnitRate UnitRate { get; init; } = UnitRate.PerMinute;
        public List<Models.Item> Items { get; init; } = new();
        public List<Models.Machine> Machines { get; init; } = new();
        public List<Models.Recipe> Recipes { get; init; } = new();
        public List<Transport> Transports { get; init; } = new();
        public List<DesiredAmount> DesiredOutputs { get; init; } = new();
        public string Version { get; init; } = "1.0";
    }

    public sealed record Transport
    {
        public string Id { get; init; } = System.Guid.NewGuid().ToString("D");
        public string Name { get; init; } = string.Empty;
        public IngredientType ItemType { get; init; } = IngredientType.Solid;
        public decimal Speed { get; init; } // units per configured time
    }

    public sealed record DesiredAmount
    {
        public string ItemId { get; init; } = string.Empty;
        public decimal Quantity { get; init; } // in configured UnitRate
    }
}