sealed class Utility {

    /// <summary>
    /// Divides two values and handles 0 errors
    /// </summary>
    /// <remarks>
    /// Handles the following errors: (by setting to 0)
    /// NaN,
    /// PositiveInfinity,
    /// NegativeInfinity,
    /// <param name="numerator"> The numerator (dividend) of the division </param>
    /// <param name="denominator"> The denominator (divisor) of the division </param>
    /// <returns> Divided double value </returns>
    public static double Divide(double numerator, double denominator) {
        double result = numerator / denominator;
        switch (result) { // if result is any of the following, set it to 0
            case double.NaN:
            case double.PositiveInfinity:
            case double.NegativeInfinity:
                result = 0;
                break;
        }
        return result;
    }
}