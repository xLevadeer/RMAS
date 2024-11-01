using Newtonsoft.Json;

partial class User {

    // --- VARIABLES ---

    [JsonProperty("crowns")]
    private Count Crowns { get; set; } = new(); 

    // --- METHODS ---

    /// <summary>
    /// Gets the Crowns as an int
    /// </summary>
    /// <returns></returns>
    public int GetCrowns() {
        return Crowns.Get<int>();
    }
}