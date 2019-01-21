using System;
using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Contains and manages unit stats
/// </summary>
[Serializable]
public class StatData
{
    /// <summary>
    /// A collection of stats growths
    /// These are transformed into m_statsGrowths
    /// </summary>
    [SerializeField]
    List<StatGrowthRate> m_growths = new List<StatGrowthRate>();

    /// <summary>
    /// A collection of frequency rates
    /// These are transformed into m_rates
    /// </summary>
    [SerializeField]
    List<FrequencyRate> m_frequencies = new List<FrequencyRate>();

    /// <summary>
    /// A container for stats => dice growth pair 
    /// </summary>
    public Dictionary<StatsId, Dice> Growths { get { return m_statsGrowths; } }
    Dictionary<StatsId, Dice> m_statsGrowths;    

    /// <summary>
    /// The rates at which bars fill up and movement happens
    /// </summary>
    public Dictionary<RateId, float> GrowthRates { get { return m_rates; } }
    Dictionary<RateId, float> m_rates = new Dictionary<RateId, float>()
    {
        { RateId.Attack, 0 },
        { RateId.Special, 0 },
        { RateId.Movement, 0 },
    };

    /// <summary>
    /// A container for the base values for each stats 
    /// </summary>
    Dictionary<StatsId, int> m_stats = new Dictionary<StatsId, int>()
    {
        { StatsId.HP_Cur, 0 },
        { StatsId.HP_Max, 0 },
        { StatsId.Attack, 0 },
        { StatsId.Special, 0 },
        { StatsId.Speed, 0 },
        { StatsId.Experience, 0 },
    };

    /// <summary>
    /// A container for all active stats modifiers
    /// </summary>
    List<StatsModifier> m_modifiers = new List<StatsModifier>();

    /// <summary>
    /// The unit's current level
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// The unit's current experience
    /// </summary>
    public int Exp { get; set; }

    /// <summary>
    /// The experience required for the next level
    /// </summary>
    public int NextLevelExp { get; set; }

    /// <summary>
    /// Gets/Sets the given stat id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int this[StatsId id]
    {
        get {
            int stat = 0;

            if (m_stats.ContainsKey(id)) {
                stat = m_stats[id];
            }

            return stat;
        }
        set {
            m_stats[id] = value;
        }
    }

    /// <summary>
    /// Gets/Sets the given rate id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public float this[RateId id]
    {
        get {
            float rate = 0f;

            if (m_rates.ContainsKey(id)) {
                rate = m_rates[id];
            }

            return rate;
        }

        set {
            m_rates[id] = value;
        }
    }

    /// <summary>
    /// True after the Init() method was called
    /// </summary>
    public bool IsInitialized { get; set; }

    /// <summary>
    /// Resets level to 0, experience to 0, and Next Level EXP to 0
    /// Initializes the stats growths and base stats
    /// </summary>
    public void Init()
    {
        IsInitialized = true;

        Level = 0;
        Exp = 0;
        NextLevelExp = 0;

        ResetGrowths();
        BuildRates();
        ResetStats();
    }

    /// <summary>
    /// Recreates the dice for the stats growth to resets their seeds
    /// </summary>
    void ResetGrowths()
    {
        m_statsGrowths = new Dictionary<StatsId, Dice>();

        foreach (StatGrowthRate growth in m_growths) {
            m_statsGrowths[growth.ID] = growth.CreateDice();
        }
    }

    /// <summary>
    /// Converts the frenquencies list into rates
    /// </summary>
    void BuildRates()
    {
        m_rates = new Dictionary<RateId, float>();

        foreach (FrequencyRate frequency in m_frequencies) {
            m_rates[frequency.ID] = frequency.Rate;
        }
    }

    /// <summary>
    /// Resets all the base stats to zero or to a growth roll if it has one
    /// </summary>
    void ResetStats()
    {
        Dictionary<StatsId, int> stats = new Dictionary<StatsId, int>();

        foreach (KeyValuePair<StatsId, int> item in m_stats) {
            StatsId id = item.Key;
            int value = item.Value;

            if (m_statsGrowths.ContainsKey(id)) {
                stats[id] = m_statsGrowths[id].Roll();
            } else {
                stats[id] = 0;
            }
        }

        // Reset current health to max health
        stats[StatsId.HP_Cur] = stats[StatsId.HP_Max];
        m_stats = stats;
    }

    /// <summary>
    /// Forces all current stats to be a zero value
    /// </summary>
    public void SetStatsToZero()
    {
        Dictionary<StatsId, int> stats = new Dictionary<StatsId, int>();

        foreach (KeyValuePair<StatsId, int> item in m_stats) {
            StatsId id = item.Key;
            int value = item.Value;
            stats[id] = 0;
        }

        m_stats = stats;
    }

    /// <summary>
    /// Allows stats to be iterable
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetEnumerator()
    {
        return m_stats.GetEnumerator();
    }

    /// <summary>
    /// Returns the value of the given stats with its modifiers applied
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int GetStat(StatsId id)
    {
        int stats = this[id];
        int multiplier = 0;

        foreach (StatsModifier modifier in m_modifiers) {

            StatData incrementalStats = modifier[ModifierType.Incremental];
            StatData multiplierStats = modifier[ModifierType.Multiplier];

            int incremental = incrementalStats[id];
            int multiply = multiplierStats[id];

            stats += incremental;
            multiplier += multiply;
        }

        int total = stats + (stats * multiplier);
        return total;
    }

    /// <summary>
    /// Returns a string with all the current stats id : value
    /// </summary>
    public string GetStatsPrintOut()
    {
        StringBuilder builder = new StringBuilder();
        string format = "{0}: {1}, ";

        foreach (KeyValuePair<StatsId, int> stats in m_stats) {
            builder.AppendFormat(format, stats.Key, stats.Value);
        }

        return builder.ToString();
    }

    /// <summary> 
    /// Adds the given modifier to the list of active modifiers
    /// </summary>
    /// <param name="modifier"></param>
    public void AddModifier(params StatsModifier[] modifiers)
    {
        foreach (StatsModifier modifier in modifiers) {
            if (!m_modifiers.Contains(modifier)) {
                m_modifiers.Add(modifier);
            }
        }
    }

    /// <summary>
    /// Removes the given modifier from the active modifiers
    /// </summary>
    /// <param name="modifier"></param>
    public void RemoveModifier(params StatsModifier[] modifiers)
    {
        foreach (StatsModifier modifier in modifiers) {
            if (m_modifiers.Contains(modifier)) {
                m_modifiers.Remove(modifier);
            }
        }
    }

    /// <summary>
    /// Removes all active modifiers
    /// </summary>
    public void ClearModifiers()
    {
        m_modifiers.Clear();
    }

    /// <summary>
    /// Returns a copy of this stats class
    /// Because the content that we want to copy is part of the class itself
    /// a shallow copy won't work therefore we create a new class instead
    /// </summary>
    /// <returns></returns>
    public StatData Copy()
    {
        StatData stats = new StatData();

        foreach (KeyValuePair<StatsId, int> item in m_stats) {
            stats[item.Key] = item.Value;
        }

        return stats;
    }
}
