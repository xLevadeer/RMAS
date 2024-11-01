using Newtonsoft.Json;

partial class User {

    // --- VARIABLES ---

    [JsonProperty("started")] 
    private DateTime CagingStarted {get; set;} = new();

    [JsonProperty("period")] 
    private TimeSpan CagingPeriod {get; set;} = new();

    [JsonIgnore] 
    private const string DateTimeDisplay = "hh:mm tt (on M/d/yyyy)";

    [JsonProperty("last_caging_was_purchased")]
    private bool LastCagingWasPurchased {get; set;} = false;

    // --- METHODS ---

    /// <summary>
    /// Gets the average caging cost based on the current difficulty
    /// </summary>
    /// <returns> Integer rounded cost </returns>
    public int GetAverageCagingCost() {
        // (Cycle.DefaultValues.NumMinMax.max - Cycle.DefaultValues.NumMinMax.min) gives us the range of possible Nums
        // / 2 gives us the average output of the range
        // / 100.0 turns the integer into a (double) percent value
        // + 1 makes sure the value accounts for always being higher than the original (like how buying caging with tokens does)
        // * the default purchase amount turns the percentage value into an estimate of the average cost of buying a caging
        return (int)Math.Round(DefaultCagingPurchaseAmount * (1 + ((Cycle.DefaultValues.NumMinMax.max - Cycle.DefaultValues.NumMinMax.min) / 2 / 100.0)));
    }

    /// <summary>
    /// Gets the current time difficulty as a percent (double between 0 and 1)
    /// </summary>
    /// <param name="index_nullable"> The index to get the time difficulty from. By default it will be the completed index. </param>
    /// <returns> Double between 0 and 1 </returns>
    private double GetTimeDifficultyPercent(int? index_nullable=null) {
        int index = index_nullable ?? Randoms.CompletedIndex.Get<int>(); // set index to defualt

        int curr_num = Randoms.Nums[index]; // get the current random number
        // has to be - 1 from the index here because 
        int range_of_difficulties = Randoms.GetNumsMax() - Randoms.GetNumsMin(); // get the range of possible time difficulties in the current cycle
        double curr_time_difficulty_percent = (double)curr_num / range_of_difficulties; // get the relative time difficulty based on the (technically max) relative range of possible difficulties
        return curr_time_difficulty_percent;
    }

    /// <summary>
    /// Uncages the user and handles either success or failiure
    /// </summary>
    /// <param name="successful"> If the user was successful or failed. If this is false the reward_user parameter doesn't matter </param>
    /// <param name="reward_user"> If the user should be rewarded at all. Refusals will always be rewarded no matter what. </param>
    /// <param name="time_was_purchased"> If the was purchased or commanded </param>
    public void Uncage(bool successful, bool reward_user=true) {
        // stats total time
        (LastCagingWasPurchased ? Stats.Time.Purchased : Stats.Time.Commanded).Add(CagingPeriod); // sets based on purchased or commanded

        // success and failiure block
        if (successful) {
            Console.WriteLine("Caging completed successfully!");
            Randoms.CompletedIndex.Increment();
            // stats success turn
            Stats.Turns.Successfully.Add(1);
            if (Randoms.CompletedIndex.Get<int>() >= Randoms.Nums.Count) { // if the completed index is out of bounds of the Nums list (cycle complete)
                // rewards are given first to make sure they are of the completed index, not the last one
                Reward(true, reward_user); // give refusal for completing a cycle
                GenerateNewRandoms();
            } else { // cycle isn't complete
                Reward(false, reward_user); // give regular rewards
            }
        } else { // unsuccessful
            // get a replacement number
            Console.WriteLine("Caging FAILED!");
            
            // failiure loses logic
            int token_price = GetRandomPrice(0.25f); // no gauranteed portion
            token_price = ApplyCoinBonus(token_price); // apply coin bonus
            if (Tokens.Purchase(token_price)) { // tokens substraction by price
                Console.WriteLine($"- You lost {token_price} tokens!\n");
            } else if (Tokens.Get<int>() != 0) { // tokens subtraction if not 0 (and smaller than price)
                Console.WriteLine($"- You lost {Tokens.Get<int>()} tokens!\n");
                Tokens.Clear();
            } else if (Refusals.Purchase(2)) { // refusal subtraction
                Console.WriteLine("- You lost 2 refusals!");
            } else if (PhallicPhotos.IsPositive()) { // phallic photo subtraction
                Console.WriteLine($"- You lost all ({PhallicPhotos.Get<int>()}) your phallic photos!\n");
                PhallicPhotos.Clear();
            } else { // account deletion
                Console.WriteLine("\nYou lost! Your profile has been deleted\n");
                Remove();
                Environment.Exit(0);
            }

            // replaces random AFTER doing all current calculations
            Randoms.ReplaceRandom(); // no reward, give a new time for trying again
            
            // stats failed turn and failed time
            Stats.Turns.Unsuccessfully.Add(1);
            (LastCagingWasPurchased ? Stats.TimeFailed.Purchased : Stats.TimeFailed.Commanded).Add(CagingPeriod); // sets based on purchased or commanded
        }

        // set iscaging values to default
        CagingPeriod = default;
        CagingStarted = default;
    }

    /// <summary>
    /// Handling for caging the user by command
    /// </summary>
    public void CommandCaging() {
        LastCagingWasPurchased = false;
        Cage();
    }

    /// <summary>
    /// Cage the user
    /// </summary>
    private void Cage() {
        // set the caging period
        int time_in_minutes = 15 * Randoms.Nums[Randoms.CompletedIndex.Get<int>()]; // get the time from the current completed index
        CagingPeriod = TimeSpan.FromMinutes(time_in_minutes);

        // sets the caging started time rounded to minutes
        DateTime current_time = DateTime.Now;
        if (current_time.Second >= 30) current_time.AddMinutes(1);
        CagingStarted = new DateTime(current_time.Year, current_time.Month, current_time.Day, current_time.Hour, current_time.Minute, 0); // sets all the same and seconds to 0

        // print the caging period
        Console.Write("You must cage..." +
        $"\n> for {GetCagingPeriod()}!" + 
        "\n\n");
    }

    /// <summary>
    /// Check if the user is caged
    /// </summary>
    /// <returns> Boolean </returns>
    public bool IsCaged() {
        return CagingStarted != new DateTime();
    }

    /// <summary>
    /// Gets the time the user started caging
    /// </summary>
    /// <returns> String </returns>
    public string GetCagingStarted() {
        return CagingStarted.ToString(DateTimeDisplay);
    }

    /// <summary>
    /// Gets the period of time the user must cage for
    /// </summary>
    /// <returns> String </returns>
    public string GetCagingPeriod() {
        return (CagingPeriod.Hours > 0 ? $"{CagingPeriod.Hours} hours and " : "") + $"{CagingPeriod.Minutes} minutes"; // only display hours if there are more than 0
    }

    /// <summary>
    /// Gets the time the user is estimated to stop caging
    /// </summary>
    /// <returns> String </returns>
    public string GetCagingEndEstimated() {
        return (CagingStarted + CagingPeriod).ToString(DateTimeDisplay);
    }
}