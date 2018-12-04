using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for player units
/// </summary>
[RequireComponent(typeof(PlayerStatsUIManager))]
abstract public class PlayerUnit : Unit
{
    /// <summary>
    /// A reference to the player UI stats manager
    /// </summary>
    [SerializeField]
    protected PlayerStatsUIManager m_statsUIManager; 

    /// <summary>
    /// Handles initialization
    /// Sets the current experience require to level up
    /// </summary>
    public override void Init()
    {
        if (m_statsUIManager == null) {
            m_statsUIManager.GetComponent<PlayerStatsUIManager>();
        }

        m_targetLayer = 1 << LayerMask.NameToLayer("EnemyUnit");

        m_statsUIManager.Init(Stats);
        BindUIEvents();

        IsActive = true;
        AIManager.instance.ResumeTimer(m_attackTimer);
        AIManager.instance.ResumeTimer(m_specialTimer);
    }

    /// <summary>
    /// Notifies the UI Manager that stats have changed
    /// </summary>
    override protected void OnStatsChanged()
    {
        // Handles updating stats display
        if (m_statsUIManager != null) {
            m_statsUIManager.OnStatsChange(Stats);
        }

        // Attack and Power cooldown frequency updates
        if (m_attackTimer != null) {
            m_attackTimer.Frequency = Stats.attackFrequency;
        }

        if (m_specialTimer != null) {
            m_specialTimer.Frequency = Stats.specialFrequency;
        }
    }

    /// <summary>
    /// Wires the cooldown timers with the ui elements to represent progress
    /// </summary>
    protected virtual void BindUIEvents()
    {
        // Attacks
        if (m_statsUIManager.m_attackBar != null) {
            m_attackTimer.OnTimerIncremented = m_statsUIManager.m_attackBar.OnTimerIncremented;
        }

        // Specials
        if (m_statsUIManager.m_specialBar != null) {
            m_specialTimer.OnTimerIncremented = m_statsUIManager.m_specialBar.OnTimerIncremented;
        }
    }
}
