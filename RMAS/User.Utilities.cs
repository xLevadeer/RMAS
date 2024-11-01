using Newtonsoft.Json;

partial class User {

    // --- VARIABLES ---

    [JsonProperty("randoms")] 
    public Cycle Randoms {get; private set;} = new();

    [JsonProperty("stats")]
    public UserStats Stats { get; private set;} = new();

    // --- METHODS ---

    /// <summary>
    /// Rewards the user with the chosen rewards
    /// </summary>
    /// <param name="reward_refusal"> If refusals should be rewarded. Refusals are always rewarded despite if reward_user is true </param>
    /// <param name="reward_user"> If the user should be rewarded at all. Must be true for this function to do anything </param>
    private void Reward(bool reward_refusal, bool reward_user=true) {
        const double coin_bonus_from_total_cycle = 10 * 0.5; 
            // 10 is the value I want
            // (0.25 is the chance for the game to increase the difficulty by 1) multilpied by 2 to account for chances where it goes up by 2
                // we need this because the coin bonus is supposed to increase at the same rate as the difficulty
        const double refusal_reward_percentage = 1.0 / 7.0; // 1 refusal every 7 turns average
        double curr_time_difficulty_percent = GetTimeDifficultyPercent(Randoms.CompletedIndex.Get<int>() - 1); // gets the time difficulty as a range between 0 and 1
        curr_time_difficulty_percent = Math.Pow(curr_time_difficulty_percent, 2) + (0.5 * curr_time_difficulty_percent) + 0.5; // polynomial equation to give the following values:
            // 0 = 0.5
            // 0.5 = 1
            // 1 = 2

        // reward refusals
        if (reward_refusal) {
            // alters the refusal amount by the current time difficulty
            int refuasls_amount = (int)Math.Round((refusal_reward_percentage * curr_time_difficulty_percent) * Randoms.Difficulty);
            if (refuasls_amount == 0) refuasls_amount = 1;
            Refusals.Add(refuasls_amount);
            Stats.Refusals.Earned.Add(refuasls_amount);

            Console.Write("You completed a Cycle!" + 
            $"\n+ You've been rewarded {refuasls_amount} refusals!" +
            "\n");
        }

        // check if further rewards should be given
        if (!reward_user) { Console.WriteLine(); return; } // writes a line for spacing when a refusal is rewarded

        // tokens
        int coin_reward = GetRandomPrice(0.5f, Randoms.CompletedIndex.Get<int>() - 1); // half gauranteed
        coin_reward = ApplyCoinBonus(coin_reward); // apply the coin bonus
        Tokens.Add(coin_reward);
        Stats.Tokens.Earned.Add(coin_reward);
        Console.Write($"+ You earned {coin_reward} tokens!\n");

        // phallic photo
        if (Chance.Of(curr_time_difficulty_percent, 4)) { // 1/8th to 1/2 chance to get a phallic photo 
            PhallicPhotos.Increment();
            Stats.PhallicPhotos.Earned.Add(1);
            Console.Write($"+ You earned a phallic photo!\n");
        }

        // coin bonus & crowns
        if (!Randoms.IsMaxDifficulty()) { // only increase the coin bonus if it's not max difficulty or greater
            if (!(CoinBonus.Get<double>() >= CoinBonusMax)) { // only adds to the coin bonus if it's not above the max
                double coin_bonus_amount = coin_bonus_from_total_cycle / (double)Randoms.Difficulty;
                CoinBonus.Add(coin_bonus_amount);
                // will not print for floored 0. This means that there is a silent (to the user) increase happening
                if ((int)Math.Floor(coin_bonus_amount) != 0) Console.Write($"+ Your coin bonus has increased by {coin_bonus_amount:F0}%!\n");
            }
        } else { // crowns section
            Stats.Time.MaxDifficultySuccessful.Add(CagingPeriod); // time is handled down here because we only want successful and not refused
            Crowns.Increment();
        }

        Console.WriteLine(); // adds space
    }

    /// <summary>
    /// Generates a new randoms Cycle and increments difficulty based on respective chances
    /// </summary>
    private void GenerateNewRandoms() {
        int difficulty = Randoms.Difficulty;
        if (!Randoms.IsMaxDifficulty()) { // only tries to increase the difficulty if it's not the max
            // get (new) difficulty (from old difficulty)
            difficulty += Chance.Of(1, 4) ? (Chance.Of(1, 3) ? 2 : 1) : 0;
            // lets break down the chances
                // 75% chance to be 0
                // 16%~ chance to be 1
                // 8%~ chance to be 2
        }

        // num min and max
            // shifts the value by the percentage upwards (level 2 is a 10 shift upwards)
        int num_increase = (difficulty - Cycle.DefaultValues.CycleLength) * 10; // each level of difficulty (after 1) is worth a 10% num increase (as undivided int 10);
        int num_max = Cycle.DefaultValues.NumMinMax.max + num_increase;
        int num_min = Cycle.DefaultValues.NumMinMax.min + num_increase;

        // nums min and max
        int nums_max = Randoms.GetNumsMax(difficulty);
        int nums_min = Randoms.GetNumsMin(difficulty);

        // set/generate the new randoms
        Randoms = new Cycle(difficulty, (num_min, num_max), (nums_min, nums_max));
    }
}