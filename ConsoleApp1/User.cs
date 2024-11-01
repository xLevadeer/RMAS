using System.Collections.Immutable;
using System.Reflection;
using Newtonsoft.Json;

/// <summary>
/// <c>User</c> class handles interactions with user-specific data and information. Anything pertaining to a user is handled here or as part of it.
/// </summary>
partial class User {
    // --- VARIABLES ---        

    [JsonIgnore] 
    public static ImmutableList<string> Datapath {get;} = ["Data", "Users"];

    [JsonIgnore] 
    public string Username {get;}

    // --- CONSTRUCTOR ---

    /// <summary>
    /// Creates a new user if one cannot be loaded from the username
    /// </summary>
    /// <param name="username"> The primary identitifer for Users </param>
    /// <param name="tokens"> A double amount of Tokens </param>
    /// <param name="phallic_photos"> An int amount of PhallicPhotos </param>
    /// <param name="coin_bonus"> A double amounts of CoinBonus </param>
    /// <param name="refusals"> An int amount of Refusals </param>
    /// <param name="is_caged"> Determines if the user is currently caged </param>
    public User(
        string username, 
        double tokens=0, 
        int phallic_photos=0, 
        double coin_bonus=0, 
        int refusals=0,
        int crowns=0,
        DateTime caging_started=default, 
        TimeSpan caging_period=default, 
        bool last_caging_was_purchased=false,
        Cycle? randoms=null,
        UserStats? stats=null) {
        // set username
        Username = StringFormat.Name(username);

        // attempt to load
        try {
            Load();

            Console.WriteLine($"User ({Username}) was found and loaded");
        } catch (FileNotFoundException) { // will create a user if cannot be loaded
            Console.WriteLine($"User could not be found. Creating new user: \"{Username}\"");

            // assign class variables
            Tokens = new Count(tokens);
            PhallicPhotos = new Count(phallic_photos);
            CoinBonus = new Count(coin_bonus);
            Refusals = new Count(refusals);
            Crowns = new Count(crowns);
            CagingStarted = caging_started;
            CagingPeriod = caging_period;
            LastCagingWasPurchased = last_caging_was_purchased;
            // the code below is the same as "Randoms = (randoms != null) ? random : new Cycle()" but it has been simplified
            Randoms = randoms ?? new Cycle(); // assigns random or default cyle value if null (this effectively acts the same as just using a default value) (implicitly generates randoms)
            Stats = stats ?? new UserStats();

            // save this new user
            Store();
        }
    }

    /// <summary>
    /// Empty <c>User</c> class. Should only be used internally. (is required for Json reading)
    /// </summary>
    public User() {
        Username = "None";
    }

    // --- METHODS ---

    /// <summary>
    /// Reads a json file based on the username and tries to load the read values to the current User
    /// </summary>
    /// <exception cref="FileNotFoundException"> Throws exception if the user file could not be found </exception>
    private void Load() {
        // try to read json class
        User loaded_user;
        try {
            loaded_user = Json.Read<User>(Datapath.ToList().Append($"{Username}.json").ToList());
        } catch (FileNotFoundException) { // if file not found, then do not continue trying to load
            throw;
        }

        // copy class variables to this class
        Tokens = loaded_user.Tokens;
        PhallicPhotos = loaded_user.PhallicPhotos;
        CoinBonus = loaded_user.CoinBonus;
        Refusals = loaded_user.Refusals;
        Crowns = loaded_user.Crowns;
        CagingStarted = loaded_user.CagingStarted;
        CagingPeriod = loaded_user.CagingPeriod;
        LastCagingWasPurchased = loaded_user.LastCagingWasPurchased;
        Randoms = loaded_user.Randoms;
        Stats = loaded_user.Stats;
    }

    /// <summary>
    /// Stores the current user to the respective Userpath profile file
    /// </summary>
    public void Store() {
        Json.Write<User>(Datapath.ToList().Append($"{Username}.json").ToList(), this);
    }

    /// <summary>
    /// Deletes this user and their user data
    /// </summary>
    public void Remove() {
        Json.Delete(Datapath.ToList().Append($"{Username}.json").ToList());
    }

    // --- OVERRIDES ---

    public override string ToString() {
        return StringFormat.ToString(this, BindingFlags.Static);
    }
}