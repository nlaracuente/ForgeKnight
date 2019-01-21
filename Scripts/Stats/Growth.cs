
/// <summary>
/// Defines the rolls that determine how fast a stats grow
/// </summary>
public static class Growth
{
    /// <summary>
    /// Returns the dice that represents the given growth rate
    /// The dice is created each time so that the random seed is changed each time
    /// </summary>
    /// <param name="rate"></param>
    /// <returns></returns>
    static public Dice GetDiceForRate(GrowthRate rate)
    {
        // Defaults to always return 1
        string notation = "1d1";

        switch (rate) {
            case GrowthRate.SLOW:
                notation = "1d3";
                break;
            case GrowthRate.MEDIUM:
                notation = "2d3";
                break;
            case GrowthRate.FAST:
                notation = "3d2";
                break;
            case GrowthRate.FASTEST:
                notation = "4d3";
                break;
        }

        Dice dice = new Dice(notation);

        return dice;
    }
}

/// <summary>
/// The growth speed rate
/// </summary>
public enum GrowthRate
{
    SLOW,
    MEDIUM,
    FAST,
    FASTEST,
}
