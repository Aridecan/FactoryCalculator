using System.Text.Json.Serialization;

namespace FactoryCalculator.Core
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum IngredientType
    {
        Solid,
        Liquid,
        Gas
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UnitRate
    {
        PerSecond,
        PerMinute,
        PerHour
    }
}
