
/// <summary>
/// Handles chances for numbers to be selected
/// </summary>
static class Chance {
    // --- METHODS ---

    /// <summary>
    /// Runs a chance and returns the respective value based on failure or success
    /// </summary>
    /// <param name="chance"> The chance of something occuring (numerator) </param>
    /// <param name="outOf"> The value the chance occurs out of (denominator) </param>
    /// <returns> Boolean </returns>
    public static bool Of(double chance, double outOf) {
        // check to make sure the function can run
        if ((chance < 0) || (outOf < 0)) {
            // values are negative, can't run
            throw new ArgumentException($"Could not run Chance.Of; the chance and/or outOf is negative: chance {chance}, outOf {outOf}");
        }
        if (chance >= outOf) {
            // could technically run as just a 100% chance, but if this is the case them something is wrong
            throw new ArgumentException($"Could not run Chance.Of; the chance is equal to or greater than the outOf amount: chance {chance}, outOf {outOf}");
        }

        Random r = new Random();
        return (r.NextDouble() * outOf) <= chance; // generates a random number and checks where the number lands in terms of the chance
    }
}