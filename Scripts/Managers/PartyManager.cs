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
        m_party.ForEach(x => {
            UnitStats stats = x.Stats;
            int newHP = Mathf.Min(stats.hp + amount, stats.maxHP);
            stats.hp = newHP;

            // Triggers the UI change
            x.Stats = stats;
            x.DisplayHealing(amount);
        });
    }
}
