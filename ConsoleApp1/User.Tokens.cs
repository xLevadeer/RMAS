using Newtonsoft.Json;

partial class User {

    // --- VARIABLES ---

    [JsonProperty("tokens")] 
    private Count Tokens {get; set;} = new();

    [JsonIgnore] 
    public const int DefaultCagingPurchaseAmount = 15;

    [JsonIgnore]
    public const int DefaultBathroomBreakPurchaseAmount = 8;

    // --- METHODS ---

    /// <summary>
    /// Gets the Token Amount as an int
    /// </summary>
    /// <returns> Integer </returns>
    public int GetTokens() {
        return Tokens.Get<int>();
    }

    /// <summary>
    /// Gets the current cost of caging (based on the current cycle)
    /// </summary>
    /// <returns> Integer </returns>
    public int GetCagingCost() {
        // multiplies the default amount by the Num as a percent value (double)
        return (int)Math.Round(DefaultCagingPurchaseAmount * (1 + (Randoms.Num / 100.0)));
    }

    /// <summary>
    /// Cages the user by attempting to buy with tokens and prints message
    /// </summary>
    public void BuyCaging() {
        int price = GetCagingCost();
        if (Tokens.Purchase(price)) { // if purchase went through
            Console.WriteLine("Successfully purchased caging!\n");
            Stats.Tokens.Spent.Begging.Add(price);
            LastCagingWasPurchased = true;
            Cage();
        } else { // if purchase not successful
            Console.WriteLine("You do not have enough Tokens to cage currently!\n");
        }
    }

    /// <summary>
    /// Gets the current cost of a bathroom break (based on the current cycle)
    /// </summary>
    /// <returns> Integer </returns>
    public int GetBathroomBreakCost() {
        // multiplies the default amount by the Num as a percent value (double)
        return (int)Math.Round(DefaultBathroomBreakPurchaseAmount * (1 + (Randoms.Num / 100.0)));
    }

    public void BuyBathroomBreak() {
        int price = GetBathroomBreakCost();
        if (Tokens.Purchase(price)) {
            Console.WriteLine("Successfully purchased bathroom break!");
            Stats.Tokens.Spent.BathroomBreak.Add(price);
            // nothing real actually happens
        } else {
            Console.WriteLine("You do not have enough Tokens to have a bathroom break currently!");
        }
    }

    /// <summary>
    /// Gets a random price based on various factors, namely including a portion of gauranteed coins
    /// </summary>
    /// <remarks>
    /// The main considerations this function takes into account are: 
    /// difficulty,
    /// caging period length
    /// </remarks>
    /// <param name="portion_guaranteed"> The portion of coins which is guaranteed </param>
    /// <param name="index_nullable"> The The index to get the time difficulty from. By default it will be the completed index. </index>
    /// <returns> Integer rounded price </returns>
    private int GetRandomPrice(float portion_guaranteed, int? index_nullable=null) {
        int index = index_nullable ?? Randoms.CompletedIndex.Get<int>(); // set index to defualt        

        Random r = new();

        // translate caging period
        double time_difficulty_alteration_percent = GetTimeDifficultyPercent(index) + 0.5; // add 0.5 the effect; make the min and max alteration 0.5 and 1.5

        // calculate coins
        int average_caging_cost = GetAverageCagingCost(); // get the total average caging cost
        const double amount_of_turns = 4.0; // describes the amount of turns it should take to get enough coins to afford a caging (must be a double so double divison happens)
        double average_coin_reward = average_caging_cost / amount_of_turns; // the average total amount of coin rewards the user should get
        double average_coin_reward_altered = average_coin_reward * time_difficulty_alteration_percent; // apply the time difficulty percent to the average coin reward (now altered)
        int guaranteed_coins = (int)Math.Floor(average_coin_reward_altered * portion_guaranteed * 2); // the amount of coins the user will get 100% of the time (this will always be half of the average coins)
        // the amount of random coins the user can get is the average coins, which will have an average value equal enough such that when it's added to the gauranteed value it will equal the average coin reward (altered)
        int random_max = (int)Math.Ceiling(average_coin_reward_altered * (1 - portion_guaranteed));
        // you'll notice the random is ceilinged and the guaranteed is floored. This creates a natural Round when getting the actual average coin reward.
            // rounding both would do the same thing, but this way the random chance is a slightly larger value than the guaranteed if neccesary.
        int coin_reward = guaranteed_coins + r.Next(0, random_max + 1); 
        // creates a random int value which will average to the round of average coin reward (adds 1 for inclusion)
            // this value can be off by a difference of 1 in each direction. this is because dividing and rounding to integers for getting portions changes how ranges (of values) are handled
        return coin_reward;
    }

    /// <summary>
    /// Applies the coin bonus to the initial_coins input
    /// </summary>
    /// <param name="initial_coins"> The initial amount of coins before the bonus has been applied </param>
    /// <returns> Integer coin amount </returns>
    private int ApplyCoinBonus(int initial_coins) => (int)(initial_coins * (1 + (CoinBonus.Get<double>() / 100.0)));
}