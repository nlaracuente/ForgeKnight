using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Handles interaction between enemies and the party as well as other party memebers
/// Since we only have the Knight which can receive damage, most of the calls is to
/// interact with the Knight
/// </summary>
public class PartyManager : MonoBehaviour
{
    /// <summary>
    /// Singleton
    /// </summary>
    static public PartyManager instance;

    /// <summary>
    /// A list of all the player units (a.k.a party)
    /// Only the Knight should be here for now but if we do
    /// decide on adding others we could
    /// </summary>
    [SerializeField]
    List<PlayerUnit> m_party = new List<PlayerUnit>();
    List<PlayerUnit> Party
    {
        get {
            if (m_party == null) {
                m_party = new List<PlayerUnit>();
            }
            return m_party;
        }
        set { m_party = value; }
    }

    /// <summary>
    /// Returns the average party level
    /// </summary>
    public int AverageLevel
    {
        get {
            int mean = 1;

            if (m_party.Count > 0) {
                int total = 0;
                foreach (PlayerUnit unit in m_party) {
                    total += unit.Stats.Level;
                }

                float avg = total / m_party.Count;
                mean = Mathf.RoundToInt(avg);
            }

            return mean;
        }
    }

    /// <summary>
    /// Sets up the reference
    /// </summary>
    void Awake ()
    {
        instance = this;
    }
	
    /// <summary>
    /// Restores the health of the party by the giving amount 
    /// without going over the unit's max hp
    /// </summary>
    /// <param name="amount"></param>
	public void HealParty(int amount)
    {
        Party.ForEach(unit => {
            StatData stats = unit.Stats;
            int newHP = Mathf.Min(stats[StatsId.HP_Cur] + amount, stats[StatsId.HP_Max]);
            stats[StatsId.HP_Cur] = newHP;

            // Triggers the UI change
            unit.Stats = stats;
            unit.DisplayHealing(amount);
        });
    }

    /// <summary>
    /// Restores party's health back to 100%
    /// </summary>
    public void ReviveParty()
    {
        Party.ForEach(unit => {
            StatData stats = unit.Stats;
            stats[StatsId.HP_Cur] = stats[StatsId.HP_Max];

            // Triggers the UI change
            unit.Stats = stats;
            unit.ResetTimers();
            unit.DisplayHealing(stats[StatsId.HP_Max]);
            unit.ResetFlashingEffect();
            unit.IsActive = true;
        });
    }

    /// <summary>
    /// Triggers all party members to be marked as disabled
    /// </summary>
    public void DisableParty()
    {
        Party.ForEach(unit => {
            unit.IsActive = false;
        });
    }

    /// <summary>
    /// Triggels all party members to be marked as active
    /// </summary>
    public void ActivateParty()
    {
        Party.ForEach(unit => {
            unit.IsActive = true;
        });
    }
}
