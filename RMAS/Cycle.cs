using System.Collections.ObjectModel;
using Newtonsoft.Json;

/// <summary>
/// <c>Cycle</c> class to store information about randoms and managing them
/// </summary>
class Cycle {
    // --- VARIABLES ---
    
    [JsonProperty("overall")] 
    public int Num {get; private set;}

    [JsonProperty("pool")] 
    public ReadOnlyCollection<int> Nums {get; private set;}

    [JsonIgnore] 
    public int Difficulty {
        get {
            return Nums.Count;
        }
    }

    [JsonProperty("completed_index")] 
    public Count CompletedIndex {get; private set;}

    public record DefaultValues {
        [JsonIgnore]
        public static int CycleLength {get;} = 1;

        [JsonIgnore]
        public static (int min, int max) NumMinMax {get;} = (5, 105); // 5 gives it a 5% starting offset

        [JsonIgnore]
        public static (int min, int max) NumsMinMax {get;} = (1, 4);

        [JsonIgnore]
        public static int MaxDifficulty = 16;
    }

    // --- CONSTRUCTOR ---

    /// <summary>
    /// Generates a new <c>Cycle</c>. Creating a new cycle with no arguments will use the default values.
    /// </summary>
    /// <param name="cycle_length"> Integer amount of nums in the cycle </param>
    /// <param name="num_min_max"> Length 2 tuple describing the min and max values of the main cycle random int (both inclusive) </param>
    /// <param name="nums_min_max"> Length 2 tuple describing the min and max values of the generated nums (both inclusive) </param>
    public Cycle(int? cycle_length=null, (int min, int max)? num_min_max=null, (int min, int max)? nums_min_max=null) {
        Random r = new Random();
        
        // set values if null
        cycle_length ??= DefaultValues.CycleLength;
        num_min_max ??= DefaultValues.NumMinMax;
        nums_min_max ??= DefaultValues.NumsMinMax;

        // check if the function can run
        if (Math.Abs(nums_min_max.Value.min - nums_min_max.Value.max) < 1) { // checks if the difference between numbers is less than 1; the function can only generate randoms if the numbers are different than each other
            throw new ArgumentException($"Cannot run random number generation for a min and max with a difference of 0: min {nums_min_max.Value.min}, max {nums_min_max.Value.max} (in \"nums_min_max\")");
        }

        // IMPORTANT: you may notice there is a keyword ".Value" being used in accessing. This is checking to make sure that there is value (not null) and is required since the tuple is nullable

        // create new cycle random
        Num = r.Next(num_min_max.Value.min, num_min_max.Value.max + 1);

        // create new randoms list
        var nums = new List<int>();
        int? last_random = null; // last random is initially set to null so it cannot equal any number
        for (int i = 0; i < cycle_length; i++) {
            int curr_random = GetDifferentRandom(last_random, nums_min_max.Value); // inputs non-nullable tuple by calling ".Value"; gets a different random
            last_random = curr_random;
            nums.Add(curr_random);
        }
        Nums = new ReadOnlyCollection<int>(nums); // convert list to readonly collection
        
        // set the completed index to a new 0
        CompletedIndex = new Count();
    }

    // --- METHODS ---

    /// <summary>
    /// Function which gets a different random than the last random
    /// </summary>
    /// <param name="last_random"> The last random that was used </param>
    /// <param name="min_max"> Length 2 tuple describing the min and max values of the new number (both inclusive) </param>
    /// <returns> Integer </returns>
    private static int GetDifferentRandom(int? last_random, (int min, int max) min_max) {
        Random r = new Random();

        // makes sure the random is different than the last
        int curr_random = r.Next(min_max.min, min_max.max + 1);
        if (curr_random == last_random) { // the randoms are the same, generate a new random from the pool of all other possibilities
            curr_random = r.Next(min_max.min, min_max.max); // new random
            if ((last_random - curr_random) <= 0) { // check if the new random is bigger, add 1
                curr_random += 1;
            }
        }
        return curr_random;
    }

    /// <summary>
    /// Replaces the random value at the input index with a different random
    /// </summary>
    /// <param name="index"> Index position of the replacement. If null then it will use the CompletionIndex </param>
    public void ReplaceRandom(int? index=null) {
        int index_not_null = index ?? CompletedIndex.Get<int>(); // equals index unless it's null where it will equal the completed index

        var nums = new List<int>(Nums); // convert readonly to a normal list
        nums[index_not_null] = GetDifferentRandom(nums[index_not_null], (GetNumsMin(), GetNumsMax())); // gets a different random replacement
        Nums = new ReadOnlyCollection<int>(nums); // wrapping so that the readonly collection can be replaced with the value replacement
    }

    /// <summary>
    /// Gets the minimum nums number based on the difficulty
    /// </summary>
    /// <param name="difficulty"> The difficulty input </param>
    /// <returns> Integer </returns>
    public int GetNumsMax(int? difficulty_input=null) {
        int difficulty = difficulty_input ?? Difficulty;
        return DefaultValues.NumsMinMax.max + (difficulty - DefaultValues.CycleLength); // each level of difficulty (after 1) is worth 15 minutes of time (as unmultiplied int 1)
    }

    /// <summary>
    /// Gets the maximum nums number based on the difficulty
    /// </summary>
    /// <param name="difficulty"> The difficulty input </param>
    /// <returns> Integer </returns>
    public int GetNumsMin(int? difficulty_input=null) {
        int difficulty = difficulty_input ?? Difficulty;
        return DefaultValues.NumsMinMax.min + (int)Math.Round((difficulty - DefaultValues.CycleLength) / 4.0); // each level of difficulty is 1/4 of a min time aka every 4 difficulty is 1 min increase
    }

    /// <summary>
    /// Checks if it's currently max difficulty
    /// </summary>
    /// <returns> Boolean true if it's max difficulty </returns>
    public bool IsMaxDifficulty() {
        return Difficulty >= DefaultValues.MaxDifficulty;
    }
}
