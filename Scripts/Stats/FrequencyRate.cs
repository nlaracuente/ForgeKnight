using System;
using UnityEngine;

/// <summary>
/// A frequency rate refers to a fixed non-int stats such as 
/// attack, special, and movement speeds which determine how 
/// quickly these actions take place.
/// </summary>
[Serializable]
public struct FrequencyRate
{
    /// <summary>
    /// The stat ID this grows
    /// </summary>
    [SerializeField]
    RateId m_id;
    public RateId ID { get { return m_id; } }

    /// <summary>
    /// How fast, in seconds, the rate is
    /// </summary>
    [SerializeField]
    float m_rate;
    public float Rate { get { return m_rate; } }
}
