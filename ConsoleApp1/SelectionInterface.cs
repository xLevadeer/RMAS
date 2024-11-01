using Ut = Utility;

public class InputException : Exception
{
    private const string Prefix = "The input was not a valid input";

    // Constructor that takes nothing
    public InputException() : base(Prefix) {} // pass default message

    // Constructor that takes a custom message
    public InputException(string message) : base($"{Prefix}; {message}") {} // Pass the message to the base class constructor

    // Constructor that takes a custom message and an inner exception
    public InputException(string message, Exception innerException) : base($"{Prefix}; {message}", innerException) {} // Pass both to the base class constructor
}

sealed class SelectionInterface {

    /// <summary>
    /// Asks a question to the user and gets their answer
    /// </summary>
    /// <param name="question"> The question to be asked </param>
    /// <returns> String </returns>
    private static string? Question(string question) {
        Console.WriteLine($"{question}");
        return Console.ReadLine();
    }

    /// <summary>
    /// Create a selection between the given options and question
    /// </summary>
    /// <param name="question"> The question to be asked </param>
    /// <param name="options"> The options that are answers to the question </param>
    /// <returns> Integer </returns>
    /// <exception cref="InputException"></exception>
    private static int Selection(string question, List<string> options) {
        // ask question
        Console.WriteLine($"{question}");

        options.Add("               Exit"); // add the exit option
        // list options
        int i = 0;
        foreach (string option in options) {
            Console.WriteLine($"{i}) {option}");
            i += 1;
        }

        // get selection
        string? selection_string = Console.ReadLine();
        Console.WriteLine(); // adds a space

        // test selection is correct
        int selection_int;
        if (string.IsNullOrWhiteSpace(selection_string)) {
            throw new InputException("There was no selection");
        }
        if (!int.TryParse(selection_string, out selection_int)) { // extract a number from the string to selection_int
            throw new InputException("The selection is not an Integer");
        }
        if ((selection_int < 0) || (selection_int >= options.Count)) { // check if the selection_int isn't in the bound of the selection options
            throw new InputException("The selection is not within the bounds of the selectable options");
        }

        // check if it's the exit option
        if (selection_int == (options.Count - 1)) {
            return -1; // exit signal
        }
        return selection_int;
    }

    /// <summary>
    /// Exits the program if the selection is -1
    /// </summary>
    /// <param name="selection"> The user's selection </param>
    /// <param name="user"> The current user is needed so they can be saved before exiting </param>
    private static void SelectionExit(int selection, User user) {
        if (selection == -1) { // exit case
            user.Store();
            Console.WriteLine("Exiting...");
            Environment.Exit(0);
        }
    }

    /// <summary>
    /// Gets the user's stats as a string
    /// </summary>
    /// <param name="user"> The current user </param>
    /// <param name="print_cage_stats"> Determines whether to print cage stats or not <param>
    /// <returns> String </returns>
    private static string GetUserStats(User user, bool print_cage_stats) {
        int completed = user.Randoms.CompletedIndex.Get<int>();
        int total = user.Randoms.Difficulty;

        const int padding = 40;
        string base_stats = $"--- {user.Username}'s Stats ---" +
        "\nTokens: ".PadRight(padding) + $"{user.GetTokens()}" +
        "\nPhallic Photos: ".PadRight(padding) + $"{user.GetPhallicPhotos()}" +
        "\nRefusals: ".PadRight(padding) + $"{user.GetRefusals()}" +
        "\nCoin Bonus: ".PadRight(padding) + $"{user.GetCoinBonus():F0}%" + // the F0 is how many decimal places to include, which is 0 in this case
        "\nCurrent Cost of Caging: ".PadRight(padding) + $"{user.GetCagingCost()} tokens" +
        "\nCurrent Cost of Bathroom Break: ".PadRight(padding) + $"{user.GetBathroomBreakCost()} tokens" +
        "\nCurrent Cycle Completion: ".PadRight(padding) + $"{completed} of {total} portions (aka {(100 * ((double)completed / total)).ToString("F0")}%)" +
        "\n";

        string cage_stats = "User caged at: ".PadRight(padding - 1) + $"{user.GetCagingStarted()}" + // not sure what the padding issue is here, but I fix it with -1 LOL
        "\nCurrent user caging period length: ".PadRight(padding) + $"{user.GetCagingPeriod()}" +
        "\nEstimated user uncaging at: ".PadRight(padding) + $"{user.GetCagingEndEstimated()}" +
        "\n";

        // crowns is set to an empty string if it's not the max difficulty
        string crowns = user.Randoms.IsMaxDifficulty() ? "Crowns: ".PadRight(padding) + $"{user.GetCrowns()}" + "\n" : "";

        return print_cage_stats ? $"{base_stats}{cage_stats}{crowns}" : base_stats;
    }

    /// <summary>
    /// Gets the user's detailed stats.
    /// </summary>
    /// <remarks>
    /// Doesn't print the user's regular stats because they are already printed at the beginning of each selection.
    /// <param name="user"> The current user </param>
    /// <returns> String </returns>
    private static string GetDetailedUserStats(User user) {
        // function to get average time difficulty
        TimeSpan get_average_time_difficulty() {
            // gets the worst case (max) nums value based on the difficulty
            int worst_case_nums_int = Cycle.DefaultValues.NumsMinMax.max + (user.Randoms.Difficulty - Cycle.DefaultValues.CycleLength);
            // gets the average difficulty as a time (this portion rounds to minutes, but it wont matter because none of my outputs can display seconds)
            int average_time_difficulty_double = (int)Math.Round(15 * (worst_case_nums_int / 2.0));
            // converts to TimeSpan
            return new TimeSpan(0, minutes:average_time_difficulty_double, 0);
        }
        
        // function to get refusal weight against
            // will output a value as a double for percentage structuring
        double get_average_refusal_weight_over_average_time_difficulty(bool use_attempted) {
            TimeSpan average_time_difficulty = get_average_time_difficulty();
            // refusal weight
            TimeSpan average_refusal_time_weight = use_attempted ? user.Stats.GetTimeAttemptedRefusedWeight() : user.Stats.GetTimeSuccessfullyRefusedWeight();
            return average_refusal_time_weight / average_time_difficulty;
        }
        
        // function for easy line generation
            // navs stands for Name and Value String
        const int padding = 60;
        string navs(string name, string value) {
            return $"{name.PadRight(padding)}{value}"; 
        }

        // function for formatting percentages
            // p stands for percentage
        const int max_amount_of_digits = 2;
        string p(double value) {
            value *= 100; // sets value to a percent digit
            string output_value = ""; // building the decimal part of a string
            bool found_value = false; // checks if a value has been found from the end moving up (to include 0's before it)
            for (int i = max_amount_of_digits; i >= 1; i--) { // run for the amount of included digits from the end to the front
                // the line below works by: 
                    // taking the constant input value and multiplying it by 10 to the power of i
                        // this gets the current place as an integer with extra stuff on the left side
                        // for example, the value from 0.0123 (with a max digits of 3) would be 12.3
                    // next, it takes modulus 10, which will always result in only the first value before the decimals
                        // here, 12.3 would become 2
                int curr_value = (int)(Math.Truncate(value * Math.Pow(10, i)) % 10); // gets an int value equal to the current index's decimal place
                if ((curr_value != 0) | (found_value == true)) { // if the current integer value is 0
                    output_value = $"{curr_value}{output_value}"; // prepend to the curr_value
                    found_value = true; // found is now true
                }
            }
            int value_with_no_decimals = (int)Math.Truncate(value);
            output_value = $"{value_with_no_decimals}{(found_value ? "." : "")}{output_value}%";
            return output_value.ToString();
        }

        // function for formatting TimeSpans
            // t stands for time
        string t(TimeSpan time) {
            string result = "";

            // adds to the result based on the input    
                // join_statement in the value that joins this value and the last one
                // time_value is an int of time input
                // following_statement is the statement that always goes after the time_value
            void add_to_result_for(string join_statement, int time_value, string following_statement) {
                // function to all the value and following statement to the result
                void add_value() {
                    if (!string.IsNullOrEmpty(result)) result += join_statement; // if the string is not empty then add a join statment
                    result += $"{time_value} {following_statement}"; // regular add statement
                }

                if (time_value > 1) { // if the time is more than one (and requires an s)
                    add_value();
                    result += "s";
                } else if (time_value > 0) { // if there is time to include at all
                    add_value(); 
                }
            }

            // add different values to the result
            add_to_result_for("", time.Days, "day");
            add_to_result_for(", ", time.Hours, "hour");
            add_to_result_for(" and ", time.Minutes, "minute");
            
            // check if time is 0
            if (time == TimeSpan.Zero) result += "None";

            return result;
        }

        List<string> detailed_stats = [
            $"--- {user.Username}'s Detailed Stats ---",
            "-- Token Stats --",
            navs("Tokens Earned: ", user.Stats.Tokens.Earned),
            navs("Token Spent: ", user.Stats.Tokens.Spent.Total),
            navs("Tokens Spent %: ", p(user.Stats.GetTokensSpentPercent())),
            navs("Tokens Spent Begging: ", user.Stats.Tokens.Spent.Begging),
            navs("Tokens Spent Begging %: ", p(user.Stats.GetTokensSpentBeggingPercent())),
            navs("Tokens Spent Bathroom Break: ", user.Stats.Tokens.Spent.BathroomBreak),
            navs("Tokens Spent Bathroom Break %: ", p(user.Stats.GetTokensSpentBathroombreakPercent())),
            navs("Tokens Lost: ", user.Stats.Tokens.Lost),
            navs("Tokens Lost %: ", p(user.Stats.GetTokensLostPercent())),
            "-- Phallic Photo Stats ---",
            navs("Phallic Photos Earned: ", user.Stats.PhallicPhotos.Earned),
            navs("Phallic Photos Spent: ", user.Stats.PhallicPhotos.Spent),
            navs("Phallic Photos Spent %: ", p(user.Stats.GetPhallicPhotosSpentPercent())),
            "-- Refusal Stats --",
            navs("Refusals Earned: ", user.Stats.Refusals.Earned),
            navs("Refusals Spent: ", user.Stats.Refusals.Spent),
            navs("Refusals Spent %: ", p(user.Stats.GetRefusalsSpentPercent())),
            navs("Refusals Attempted: ", user.Stats.Refusals.Attempted),
            navs("Refusals Successful: ", user.Stats.Refusals.Successfully),
            navs("Refusals Unsuccessful: ", user.Stats.Refusals.Unsuccessfully),
            navs("Refusals Successful %: ", p(user.Stats.GetSuccessfulRefusalPercent())),
            "-- Time Total Stats --",
            navs("Assigned Caging Time Total: ", t(user.Stats.Time.Total)),
            navs("Assigned Caging Time Purchased: ", t(user.Stats.Time.Purchased)),
            navs("Assigned Caging time Purchased %: ", p(user.Stats.GetTimePurchasedPercent())),
            navs("Assigned Caging Time Commanded: ", t(user.Stats.Time.Commanded)),
            navs("Assigned Caging time Commanded %: ", p(user.Stats.GetTimeCommandedPercent())),
            "-- Time Failed Stats --",
            navs("Failed Caging Time Total: ", t(user.Stats.TimeFailed.Total)),
            navs("Failed Caging Time Total %: ", p(user.Stats.GetTimeTotalFailedPercent())),
            navs("Failed Caging Time Purchased: ", t(user.Stats.TimeFailed.Purchased)),
            navs("Failed Caging Time Purchased %: ", p(user.Stats.GetTimePurchasedFailedPercent())),
            navs("Failed Caging Time Commanded: ", t(user.Stats.TimeFailed.Commanded)),
            navs("Failed Caging Time Commanded %: ", p(user.Stats.GetTimeCommandedFailedPercent())),
            "-- Time Refused Stats --",
            navs("Refused Caging Time Attempted: ", t(user.Stats.TimeRefused.Attempted)),
            navs("Refused Caging Time Attempted %: ", p(user.Stats.GetTimeAttemptedRefusedPercent())),
            navs("Refused Caging Time Attempted Weight %", t(user.Stats.GetTimeAttemptedRefusedWeight())),
            "   The average time that one refusal is worth (based on all attempted refusals)",
            navs("Refused Caging Time Successful: ", t(user.Stats.TimeRefused.Successfully)),
            navs("Refused Caging Time Successful %: ", p(user.Stats.GetTimeSuccessfullyRefusedPercent())),
            navs("Refused Caging Time Successful Weight %: ", t(user.Stats.GetTimeSuccessfullyRefusedWeight())),
            "   The average time that one refusal is worth (based on successful refusals)",
            navs("Refused Caging Time Unsuccessful: ", t(user.Stats.TimeRefused.Unsuccessfully)),
            "-- Turn and Cycle Stats --",
            navs("Cycles Completed: ", user.Stats.CyclesCompleted),
            navs("Turns Attempted: ", user.Stats.Turns.Attempted),
            navs("Turns Successful: ", user.Stats.Turns.Successfully), 
            navs("Turns Failed: ", user.Stats.Turns.Unsuccessfully),
            navs("Turns Failed %: ", p(user.Stats.GetTurnsFailedPercent())),
            navs("Cycle Completion Turns %: ", p(user.Stats.GetCycleTurnPercent())),
            "   The relative difficulty acceleration of your user",
            "-- Time Failed or Refused Stats --",
            navs("Turns Failed or Refused Attempted %: ", p(user.Stats.GetTurnsFailedOrRefusedAttemptedPercent())),
            navs("Time Failed or Refused Attempted %: ", p(user.Stats.GetTimeFailedOrRefusedAttemptedPercent())),
            navs("Turns Failed or Refused Successfully %:", p(user.Stats.GetTurnsFailedOrRefusedSuccessfullyPercent())),
            navs("Time Failed or Refused Succesfully %: ", p(user.Stats.GetTimeFailedOrRefusedSuccessfullyPercent())),
            "-- Difficulty Stats --",
            navs("Current Difficulty: ", user.Randoms.Difficulty.ToString()),
            navs("Current Minimum Difficulty: ", user.Randoms.GetNumsMin().ToString()),
            navs("Average Time Difficulty: ", t(get_average_time_difficulty())),
            navs("Current Portion of Max Difficulty %: ", p(Ut.Divide(user.Randoms.Difficulty, Cycle.DefaultValues.MaxDifficulty))),
            navs("Refusal Weight Attempted to Average Time Difficulty: %", p(get_average_refusal_weight_over_average_time_difficulty(true))),
            navs("Refusal Weight Successful to Average Time Difficulty: %", p(get_average_refusal_weight_over_average_time_difficulty(false))),
            "   How does your refusal weight compare to the current average time difficulty? (100% would be average)",
        ];

        string messsage_joined = $"{String.Join("\n", detailed_stats)}\n\n";
        messsage_joined += user.Randoms.IsMaxDifficulty() ? $"\n{navs("Max Difficulty Time Succesfully Completed: ", t(user.Stats.Time.MaxDifficultySuccessful))}\n" : ""; // joins the crowns in if neccessary

        return messsage_joined;
    }

    private static string GetInfo() {
        List<string> info_list = [
            "--- Info ---",
            "-- What are Turns? --",
            "Turns are composed of two disctinct periods:",
            "- The Uncaged Period -",
            "During the uncaged period you can chill out. There is no task to complete and you can simply exist.",
            "During this period you can still do basic actions like view stats and redeem phallic photos.",
            "The important thing is that during this period you have the ability to purchase caging [Beg] if you so please.",
            "Purchasing caging [Beg] will move you into the 2nd phase.", 
            "You can also be commanded [Be Commanded] into the second phase by your master.",
            "- The Caged Period -",
            "During the caged period you must be in your cage.",
            "You will be given a time period that you must be in your cage for. You cannot fall below or above this limit by more than 3 minutes.",
            "If you are you will fail [Be Punished] else you will be successful [Be Prasied].",
            "You can also choose to try to refuse completing you caging [Grovel] or get a bathroom break [Relief]. There will be info about this later",
            "When caging for any of the reasons above you will move back into the uncaged period.",
            "When a turn is completed in different ways you will either recieve or lose items like refusals, tokens and phallic photos.",
            "",
            "-- What are Cycles? --",
            "Cycles are composed of a number of turns based on the difficulty.",
            "As you play, your difficulty will slowly increase and make each cycle harder by putting more turns in the cycle.",
            "There are specific rewards that you can only get when you complete a cycle",
            "",
            "-- What are Tokens? --",
            "Tokens are the main game currency. You are rewarded tokens when you complete caging succesffully [Be Praised].",
            "You lose tokens when you purchase caging [Beg] or a bathroom break [Relief].",
            "You will also lose a number of tokens when you fail [Be Punished].",
            "",
            "-- What are Phallic Photos? --",
            "Phallic photos are a redeemable currency. You have a chance to recieve phallic photos when you complete a turn.",
            "Redeeming a phallic photo means you will be sent a dick pic.",
            "",
            "-- What are Refusals? --",
            "Refusals are used to attempt to refuse to complete caging [Grovel]",
            "When you successfully refuse you will be moved out of the caged period without losing anything (except the refusal you spent).",
            "Additionally, whe you refuse successfully you will complete the turn, moving you further into the current cycle or completing it.",
            "There is a 25% chance, that when you try to refuse, you will fail and remain in the caging period.",
            "Refusals can only be rewarded when a cycle is completed.",
            "You will recieve more when the difficulty is harder.",
            "",
            "-- What is the Coin Bonus? --",
            "The coin bonus is a multiplier for how many more tokens you get when you complete a turn.",
            "The tokens you would normally get is multiplied by your coin bonus at the end of each turn.",
            "Your coin bonus will increase whenever you succesfully complete caging.",
            $"Your coin bonus cannot increase higher than {User.CoinBonusMax}%.",
            "",
            "-- What happens when you Get Commanded? --",
            "When you are commanded [Be Commanded] you will be moved into the caging period.",
            "You do not have control of when you are commanded [Be Commanded]. Your master does.",
            "",
            "-- What happens when you Beg? --",
            "When you beg you [Beg] you will be moved into the caging period.",
            "This difference between begging [Beg] and commanding [Be Commanded] is that you will pay the current token caging cost when you beg [Beg].",
            $"The default caging price is {User.DefaultCagingPurchaseAmount}, but this value is almost always different because it's applied to a random amount.",
            "",
            "-- What happens when you Get Praised? --",
            "When you are praised [Be Praised] you will successfully complete the caged period and move into the uncaged period.",
            "You will be given reward depending on the stats of the turn you have completed.",
            "",
            "-- What happens when you Get Punished? --",
            "When you get punished [Be Punished] you will lose different items depending on what is available in your inventory.",
            "It's important to remember the exact order of how this happens because YOUR ACCOUNT CAN BE DELETED ENTIRELY if you have items available.",
            "First, you will lose a random amount of tokens if you have enough. If you do not have enough you will lose all your tokens.",
            "If you have 0 tokens, you will instead lose 2 refusals if you have enough.",
            "If you have less than 2 refusals you will lose all your phallic photos.",
            "If you have 0 phallic photos your account will be ERASED.",
            "",
            "-- What happens when you Grovel? --",
            "When you refuse [Grovel] you will spend 1 refusal for an attempt to fail the turn without losing anything.",
            "This means that when you successfully refuse [Grovel] you will move out of the caged period and back into the uncaged period.",
            "When you successfully refuse [Grovel] you will lose nothing and also still complete the turn (move forward in the cycle).",
            "There is a 25% chance to fail to refuse [Grovel] and you will remain in the caged period with no repercussions aside from losing 1 refusal.",
            "",
            "-- What happens when you Relieve Yourself? --",
            "When you relieve yourself [Relief] you will pay the current bathroom break cost.",
            "Nothing changes about the turn, you are just allowed to take a bathroom break as long as your master is willing to permit.",
            $"The default bathroom break price is {User.DefaultBathroomBreakPurchaseAmount}, but this value is almost always different because it's applied to a random amount.",
            "",
            "-- What happens when you Admire? --",
            "When you admire you will spend 1 phallic photo to be send a dick pic from your master.",
            "Your master should communicate the exact terms of this to you.",
            "",
            "-- Subjectivity --",
            "The whole game is subjective specifically to your master.",
            "You must obey the commands, but your master can subjectively makes decisions like whether or not they mark your caging as complete.",
            "Pets should use this to their advantage to beg and convince their master to do things",
        ];

        return $"{String.Join("\n", info_list)}\n\n";
    }

    /// <summary>
    /// The set of selections that can be made when caged
    /// </summary>
    /// <param name="user"> The current user </param>
    private static void CagedSelection(User user) {
        Console.Write($"{GetUserStats(user, true)}\n"); // print user stats
        // get selection
        List<string> options = [
            "[Be Praised]   Mark Caging as Complete",
            "[Be Punished]  Mark Caging as Failed",
            "[Grovel]       Try to Refuse Caging",
            "[Relief]       Purchase a Bathroom Break",
            "[Admire]       Redeem Phallic Photo",
            "               Show Stats",
            "               Show Info",
        ];
        int selection = Selection("Choose an option", options);
        SelectionExit(selection, user);

        // handle selection
        switch (selection) {
            case 0:
                user.Uncage(true);
                break;
            case 1:
                user.Uncage(false);
                break;
            case 2:
                user.RefuseCaging();
                break;
            case 3:
                user.BuyBathroomBreak();
                break;
            case 4:
                user.BuyPhallicPhoto();
                break;
            case 5:
                Console.Write(GetDetailedUserStats(user));
                break;
            case 6:
                Console.Write(GetInfo());
                break;
        }
    }

    /// <summary>
    /// The set of selections that can be made when uncaged
    /// </summary>
    /// /// <param name="user"> The current user </param>
    private static void UncagedSelection(User user) {
        Console.Write($"{GetUserStats(user, false)}\n"); // print user stats
        // get selection
        List<string> options = [
            "[Be Commanded] Start Caging (free)",
            "[Beg]          Start Caging (purchase)",
            "[Admire]       Redeem Phallic Photo",
            "               Show Stats",
            "               Show Info",
        ];
        int selection = Selection("Choose an option", options);
        SelectionExit(selection, user);

        // handle selection
        switch (selection) {
            case 0:
                user.CommandCaging();
                break;
            case 1:
                user.BuyCaging();
                break;
            case 2:
                user.BuyPhallicPhoto();
                break;
            case 3:
                Console.Write(GetDetailedUserStats(user));
                break;
            case 4: 
                Console.Write(GetInfo());
                break;
        }
    }

    /// <summary>
    /// Main function that will handle all selections and complexities of selections
    /// </summary>
    public static void Initiate() {
        // get Username
        string? username = Question("Please enter a user by Username");
        if (string.IsNullOrWhiteSpace(username)) { // check if input is invalid
            throw new InputException("Username input was not valid");
        }
        // read/create user
        User user = new User(username);

        // loop the selection possibilities
        while (true) {
            if (!user.IsCaged()) { // if the user IS NOT caged
                UncagedSelection(user);
            } else { // if the user IS caged
                CagedSelection(user);
            }

            // saves the user after every main interaction completes
            user.Store();
        }
    }
}