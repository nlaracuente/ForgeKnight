using System.Collections.Generic;

/// <summary>
/// A stats modifier is an object that affects (modifiers) one or more stats
/// Each modifier has an ID so that it can be distinguished from other modifiers
/// Modifiers can either increment (+/-) and/or multiply (+/-) a sets of stats
/// </summary>
public abstract class StatsModifier
{
    /// <summary>
    /// Functions as an auto increment id so that as stats modifiers are created
    /// multiple of the same type can be and quickly identified
    /// </summary>
    private static int m_id = 0;

    /// <summary>
    /// ID that identifies this stats modifier from the rest
    /// </summary>
    public int ID { get; protected set; }

    /// <summary>
    /// A container type of modification and stats this modifier affects
    /// </summary>
    public Dictionary<ModifierType, StatData> Modifiers { get; protected set; }

    /// <summary>
    /// Returns the stats associated with the given modifier type
    /// Defaults to stats with 0 values when modifier does not exist
    /// </summary>
    /// <param name="modifier"></param>
    /// <returns></returns>
    public StatData this[ModifierType modifier]
    {
        get {
            StatData stats = new StatData();

            if (Modifiers.ContainsKey(modifier)) {
                stats = Modifiers[modifier];
            }

            return stats;
        }
    }

    /// <summary>
    /// Sets and increments the ID
    /// </summary>
    public StatsModifier()
    {
        ID = m_id++;
    }

    /// <summary>
    /// Initializes the stats modifier
    /// </summary>
    abstract protected void Init();
}