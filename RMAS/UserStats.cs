
using Newtonsoft.Json;
using Ut = Utility;

/// <summary>
/// record for storing stats of the user
/// </summary>
record UserStats {

    // --- VARIABLES and METHODS ---

    // tokens
    [JsonProperty("tokens")]
    public TokensRecord Tokens { get; } = new();
    public record TokensRecord {
        [JsonProperty("earned")]
        public AddOnlyInt Earned { get; } = new();

        public SpentRecord Spent { get; } = new();
        public record SpentRecord {
            [JsonIgnore]
            public ImmutableAddOnlyInt Total {
                get => new ImmutableAddOnlyInt((int)Begging + BathroomBreak);
            }

            [JsonProperty("begging")]
            public AddOnlyInt Begging { get; } = new();

            [JsonProperty("bathroom")]
            public AddOnlyInt BathroomBreak { get; } = new();
        }

        [JsonProperty("lost")]
        public AddOnlyInt Lost { get; } = new();
    }

    public double GetTokensSpentPercent() => Ut.Divide(Tokens.Spent.Total, Tokens.Earned);
    public double GetTokensSpentBeggingPercent() => Ut.Divide(Tokens.Spent.Begging, Tokens.Spent.Total);
    public double GetTokensSpentBathroombreakPercent() => Ut.Divide(Tokens.Spent.BathroomBreak, Tokens.Spent.Total);
    public double GetTokensLostPercent() => Ut.Divide(Tokens.Spent.Total, Tokens.Lost);

    // photos
    [JsonProperty("photos")]
    public PhallicPhotosRecord PhallicPhotos { get; } = new();
    public record PhallicPhotosRecord {
        [JsonProperty("earned")]
        public AddOnlyInt Earned { get; } = new();

        [JsonProperty("spent")]
        public AddOnlyInt Spent { get; } = new();
    }

    public double GetPhallicPhotosSpentPercent() => Ut.Divide(PhallicPhotos.Spent, PhallicPhotos.Earned);

    // refusals
    [JsonProperty("refusals")]
    public RefusalsRecord Refusals { get; } = new();
    public record RefusalsRecord {
        [JsonProperty("earned")]
        public AddOnlyInt Earned { get; } = new();

        [JsonIgnore]
        public ImmutableAddOnlyInt Spent {
            get => Attempted;
        }

        [JsonIgnore] 
        public ImmutableAddOnlyInt Attempted {
            get => new ImmutableAddOnlyInt((int)Successfully + Unsuccessfully);
        }

        [JsonProperty("successfully")]
        public AddOnlyInt Successfully { get; } = new();

        [JsonProperty("unsuccessfully")]
        public AddOnlyInt Unsuccessfully { get; } = new();
    }

    public double GetRefusalsSpentPercent() => Ut.Divide(Refusals.Spent, Refusals.Earned);
    public double GetSuccessfulRefusalPercent() => Ut.Divide(Refusals.Successfully, Refusals.Attempted);

    // time
    [JsonProperty("time")]
    public TimeRecord Time { get; } = new();
    public record TimeRecord {
        [JsonIgnore]
        public ImmutableAddOnlyTimeSpan Total { 
            get => new ImmutableAddOnlyTimeSpan((TimeSpan)Purchased + Commanded);
        }

        [JsonProperty("purchased")]
        public AddOnlyTimeSpan Purchased { get; } = new();

        [JsonProperty("commanded")]
        public AddOnlyTimeSpan Commanded { get; } = new();

        [JsonProperty("max_difficulty_successful")]
        public AddOnlyTimeSpan MaxDifficultySuccessful { get; } = new();
    }
    
    public double GetTimePurchasedPercent() => Ut.Divide(Time.Purchased, Time.Total);
    public double GetTimeCommandedPercent() => Ut.Divide(Time.Commanded, Time.Total);

    [JsonProperty("time_failed")]
    public TimeFailedRecord TimeFailed { get; } = new();
    public record TimeFailedRecord {
        [JsonIgnore]
        public ImmutableAddOnlyTimeSpan Total {
            get => new ImmutableAddOnlyTimeSpan((TimeSpan)Purchased + Commanded);
        }

        [JsonProperty("purchased")]
        public AddOnlyTimeSpan Purchased { get; } = new();

        [JsonProperty("commanded")] 
        public AddOnlyTimeSpan Commanded { get; } = new();
    }

    public double GetTimePurchasedFailedPercent() => Ut.Divide(TimeFailed.Purchased, Time.Purchased);
    public double GetTimeCommandedFailedPercent() => Ut.Divide(TimeFailed.Commanded, Time.Commanded);
    public double GetTimeTotalFailedPercent() => Ut.Divide(TimeFailed.Total, Time.Total);

    // time refused
    [JsonProperty("time_refused")]
    public TimeRefusedRecord TimeRefused { get; } = new(); // create instance of record for this UserStats
    public record TimeRefusedRecord {
        [JsonIgnore]
        public ImmutableAddOnlyTimeSpan Attempted { 
            get => new ImmutableAddOnlyTimeSpan((TimeSpan)Successfully + Unsuccessfully);
        }

        [JsonProperty("successfully")]
        public AddOnlyTimeSpan Successfully { get; } = new();

        [JsonProperty("unsuccessfully")]
        public AddOnlyTimeSpan Unsuccessfully { get; } = new();
    }

    public double GetTimeAttemptedRefusedPercent() => Ut.Divide(TimeRefused.Attempted, Time.Total);
    public double GetTimeSuccessfullyRefusedPercent() => Ut.Divide(TimeRefused.Successfully, Time.Total);
    public TimeSpan GetTimeAttemptedRefusedWeight() => new TimeSpan(0, 0, minutes:(int)Ut.Divide(TimeRefused.Attempted, Refusals.Attempted), 0);
    public TimeSpan GetTimeSuccessfullyRefusedWeight() => new TimeSpan(0, 0, minutes:(int)Ut.Divide(TimeRefused.Successfully, Refusals.Successfully), 0);
    public double GetTimeFailedOrRefusedSuccessfullyPercent() => Ut.Divide((double)TimeFailed.Total + TimeRefused.Successfully, Time.Total);
    public double GetTimeFailedOrRefusedAttemptedPercent() => Ut.Divide((double)TimeFailed.Total + TimeRefused.Attempted, Time.Total);

    // cycle
    [JsonProperty("cycles_completed")]
    public AddOnlyInt CyclesCompleted { 
        get => Refusals.Earned;
    }

    // turns
    [JsonProperty("turns")]
    public TurnsRecord Turns { get; } = new();
    public record TurnsRecord {
        [JsonIgnore]
        public ImmutableAddOnlyInt Attempted {
            get => new ImmutableAddOnlyInt((int)Successfully + Unsuccessfully);
        }

        [JsonProperty("successfully")]
        public AddOnlyInt Successfully { get; } = new();

        [JsonProperty("unsuccessfully")]
        public AddOnlyInt Unsuccessfully { get; } = new();
    }

    public double GetCycleTurnPercent() => Ut.Divide(CyclesCompleted, Turns.Successfully);
    public double GetTurnsFailedPercent() => Ut.Divide(Turns.Unsuccessfully, Turns.Attempted);
    public double GetTurnsFailedOrRefusedSuccessfullyPercent() => Ut.Divide((double)Turns.Unsuccessfully + Refusals.Successfully, Turns.Attempted);
    public double GetTurnsFailedOrRefusedAttemptedPercent() => Ut.Divide((double)Turns.Unsuccessfully + Refusals.Attempted, Turns.Attempted);
}