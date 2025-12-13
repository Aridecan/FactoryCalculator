namespace FactoryCalculator.Core.Models
{
    public sealed record RecipeIngredient
    {
        public string ItemId { get; init; } = string.Empty;
        /// <summary>
        /// Quantity expressed in the GameProfile.UnitRate (conversion done by engine)
        /// </summary>
        public decimal Quantity { get; init; }
    }
}