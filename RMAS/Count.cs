using Newtonsoft.Json;

/// <summary>
/// Count class. 
/// </summary>
[JsonConverter(typeof(CountJsonConverter))]
class Count {

    // --- VARIABLES ---

    [JsonProperty("value")] 
    private double Amount {get; set;}

    // --- CONSTRUCTOR ---

    /// <summary>
    /// Create a <c>Count</c> class
    /// </summary>
    /// <param name="amount"> Starting ammount for <c>Count</c> </param>
    public Count(double amount=0) {
        Amount = amount;
    }

    // --- METHODS ---

    /// <summary>
    /// Gets the count as either an integer or a double, depending on what is declared
    /// </summary>
    /// <returns> Integer </returns>
    public T Get<T>() where T : struct {
        if (typeof(T) == typeof(int)) {
            return (T)(object)(int)Math.Round(Amount); // Cast to object then to T
        }
        else if (typeof(T) == typeof(double)) {
            return (T)(object)(double)Amount; // Cast to object then to T
        }
        else {
            throw new InvalidOperationException("Unsupported type <T> when trying to get Count");
        }
    }

    /// <summary>
    /// Checks if the count amount is positive, 0 included
    /// </summary>
    /// <returns> Bool true if positive </returns>
    public bool IsPositive() {
        return Amount >= 0;
    }

    /// <summary>
    /// Checks if the count amount is negative, 0 included
    /// </summary>
    /// <returns> Bool true if negative </returns>
    public bool IsNegative() {
        return Amount <= 0;
    }

    /// <summary>
    /// Adds the num(ber) to the Ammount
    /// </summary>
    /// <param name="num"> Number value to add/subtract </param>
    public void Add(double num) {
        Amount += num;
    }

    /// <summary>
    /// Subtracts the num(ber) from the Ammount
    /// </summary>
    /// <param name="num"> Number value to add/subtract </param>
    [Obsolete("Don't use subtract for the RMAS program. Use SubtractAboveZero instead")]
    public void Subtract(double num) {
        Amount -= num;
    }

    /// <summary>
    /// Subtracts but will never subtract to below 0
    /// </summary>
    /// <param name="num"> Number value to add/subtract </param>
    public void SubtractAboveZero(double num) {
        // subtracts from the amount if the num isn't greater or equal to the amount
        Amount = (num >= Amount) ? 0 : Amount - num;
    }

    /// <summary>
    /// Iterates the current amount by 1.
    /// </summary>
    /// <param name="direction"> Changes the direction of iteration. True is positive. False is negative. </param>
    public void Increment(bool direction=true) {
        if (direction == true) {
            Amount += 1;
        } else {
            Amount -= 1;
        }
    }

    /// <summary>
    /// Checks if the value will become negative
    /// </summary>
    /// <param name="num"> The value attempting to subtract </param>
    /// <returns> Boolean, true if not negative </returns>
    private bool WontBecomeNegative(double num) {
        return (Amount - num) >= 0;
    }

    /// <summary>
    /// Subtracts the input from the Amount as long as it remains positive or 0
    /// </summary>
    /// <param name="num"> Number to subtract from Amount </param>
    /// <returns> True if successful; False if unsuccessful </returns>
    public bool Purchase(double num) {
        bool amount_is_positive = WontBecomeNegative(num);
        if (amount_is_positive) {
            Amount -= num;
        } 
        return amount_is_positive;
    }

    /// <summary>
    /// Clears the count, setting it to 0
    /// </summary>
    public void Clear() {
        Amount = 0;
    }

    // --- OVERRIDES ---
    public override string ToString()
    {
        return StringFormat.ToString(this);
    }
}

/// <summary>
/// Class for serializing Count.
/// </summary>
/// <remarks>
/// This class will serialize as just double rather than having the "Amount" string for every object
/// </remarks>
class CountJsonConverter : JsonConverter {
    
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        double? casted_value;
        if (value == null) { // sets to null if null
            casted_value = null;
        } else { // sets to the value if it's not null
            casted_value = ((Count)value).Get<double>(); // cast the value as a count and then get it's value as a double
        }
        writer.WriteValue(casted_value);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        Count? casted_value;
        if (reader.Value == null) { // sets to null if null
            casted_value = null;
        } else { // sets to the value if it's not null
            casted_value = new Count((double)reader.Value); // cast the value as a count and then get it's value as a double
        }
        return casted_value;
    }

    public override bool CanConvert(Type objectType)
    {
        // Specify the type this converter handles
        return objectType == typeof(Count);
    }
}