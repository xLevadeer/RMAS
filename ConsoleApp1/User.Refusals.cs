using Newtonsoft.Json;

partial class User {
    
    // --- VARIABLES ---

    [JsonProperty("refusals")] 
    private Count Refusals {get; set;} = new();

    // --- METHODS ---

    /// <summary>
    /// Gets the Refusals as an int
    /// </summary>
    /// <returns> Integer </returns>
    public int GetRefusals() {
        return Refusals.Get<int>();
    }

    /// <summary>
    /// Attempts to refuse caging and prints message
    /// </summary>
    public void RefuseCaging() {
        // Use a refusal (refusals are still used even if they fail)
        if (!Refusals.Purchase(1)) { // tries to purchase refusal here
            Console.WriteLine("You do not have enough Refusals to do this currently!\n"); 
            return; 
        }

        // run a chance for refusal to fail
        Console.WriteLine(Chance.Of(1, 4));
        if (Chance.Of(1, 4)) { // 25% chance to fail
            Console.WriteLine("Failed to refuse! Try again or be forced to cage!\n");
            // stat failed refusal turn and failed time
            Stats.Refusals.Unsuccessfully.Add(1);
            Stats.TimeRefused.Unsuccessfully.Add(CagingPeriod);
            return;
        }
        // 75% chance to pass
        Console.WriteLine("Successfully refused caging!\n");
        // stat success refusal turn abd success time
        Stats.Refusals.Successfully.Add(1);
        Stats.TimeRefused.Successfully.Add(CagingPeriod);
        Uncage(successful:true, reward_user:false);
    }
}