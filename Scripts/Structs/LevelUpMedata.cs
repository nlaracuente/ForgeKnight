
/// <summary>
/// Represents the change in value for a party member's current experience,
/// how many levels they are gaining, and by how much their stats are changing.
/// This allows the data to be displayed to the player before applying it to the party member
/// </summary>
public struct LevelUpMetada
{
    /// <summary>
    /// How much the current level will be increased by
    /// </summary>
    public int level;

    /// <summary>
    /// By how much the current stats will be increased
    /// </summary>
    public StatData stats;

    /// <summary>
    /// Sets the level up data
    /// </summary>
    /// <param name="exp"></param>
    /// <param name="_level"></param>
    /// <param name="_stats"></param>
    public LevelUpMetada(int _level, StatData _stats)
    {
        level = _level;
        stats = _stats;
    }
}