using Newtonsoft.Json;

partial class User {

    // --- VARIABLES ---

    [JsonProperty("photos")] 
    private Count PhallicPhotos {get; set;} = new();

    // --- METHODS ---

    /// <summary>
    /// Gets the Phallic Phots Amount as an int
    /// </summary>
    /// <returns> Integer </returns>
    public int GetPhallicPhotos() {
        return PhallicPhotos.Get<int>();
    }

    /// <summary>
    /// Prints a success/failiure message after purchasing a phallic photo
    /// </summary>
    public void BuyPhallicPhoto() {
        if (PhallicPhotos.Purchase(1)) {
            Console.WriteLine("Successfully redeemed Phallic Photo!\n");
            Stats.PhallicPhotos.Spent.Add(1);
        } else {
            Console.WriteLine("You do not have enough Phallic Photos to redeem this currently!\n");
        }
    }
}