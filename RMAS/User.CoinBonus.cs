using Newtonsoft.Json;

partial class User {

    // --- VARIABLES ---

    [JsonProperty("bonus")] 
    private Count CoinBonus {get; set;} = new();

    [JsonIgnore]
    public const double CoinBonusMax = 200.0;

    // --- METHODS ---

    /// <summary>
    /// Gets the Coin Bonus as a double
    /// </summary>
    /// <returns> Double </returns>
    public double GetCoinBonus() {
        return CoinBonus.Get<double>();
    }
}