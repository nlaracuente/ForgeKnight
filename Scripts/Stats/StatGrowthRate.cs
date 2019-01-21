using System;
using UnityEngine;

/// <summary>
/// A container for a single stats and its growth rate
/// This is to create a serializable key => value pair for UnityEditor
/// </summary>
[Serializable]
public struct StatGrowthRate
{
    /// <summary>
    /// The stat ID this grows
    /// </summary>
    [SerializeField]
    StatsId m_id;
    public StatsId ID { get { return m_id; } }

    /// <summary>
    /// The rate at which the stat grows
    /// </summary>
    [SerializeField]
    GrowthRate m_rate;

    /// <summary>
    /// Dice notation to use optional to growth rates
    /// </summary>
    [SerializeField, Tooltip("(Optional) Dice notation used instead of the growth rate if given")]
    string m_notation;

    /// <summary>
    /// Creates and returns a new dice for this stat
    /// </summary>
    public Dice CreateDice()
    {
        Dice dice;

        if (!string.IsNullOrEmpty(m_notation)) {
            dice = new Dice(m_notation);
        } else {
            dice = Growth.GetDiceForRate(m_rate);
        }

        return dice;
    }
}
