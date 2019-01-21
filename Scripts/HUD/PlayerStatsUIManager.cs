using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Handles binding player unit stats updates aginst the UI System
/// So that when a stat is changed the UI is notified
/// </summary>
public class PlayerStatsUIManager : MonoBehaviour
{
    /// <summary>
    /// A reference to the health progress handler
    /// </summary>
    [SerializeField]
    public HealthProgressHandler m_healthBar;

    /// <summary>
    /// A reference to the progress bar for displaying attack cooldowns
    /// </summary>
    [SerializeField]
    public CooldownProgressBar m_attackBar;

    /// <summary>
    /// A reference to the progress bar for displaying special cooldowns
    /// </summary>
    [SerializeField]
    public CooldownProgressBar m_specialBar;

    /// <summary>
    /// A reference to the level text that dislays the current level
    /// </summary>
    [SerializeField]
    public Text m_currentLevel;

    /// <summary>
    /// A reference to the level text that dislays the required experiences to level up
    /// </summary>
    [SerializeField]
    public Text m_experienceRequired;

    /// <summary>
    /// Triggers a stat change to setup the UI
    /// </summary>
    public void Init(StatData stats)
    {
        OnStatsChange(stats);
    }

    /// <summary>
    /// Triggered on any unit stats change
    /// </summary>
    /// <param name="stats"></param>
    public void OnStatsChange(StatData stats)
    {
        if(m_healthBar != null) {
            m_healthBar.CurrentHP = stats.GetStat(StatsId.HP_Cur);
            m_healthBar.MaxHP = stats.GetStat(StatsId.HP_Max);
        }

        if(m_attackBar != null) {
            m_attackBar.SetBarText(stats.GetStat(StatsId.Attack).ToString());
        }

        if(m_specialBar != null) {
            m_specialBar.SetBarText(stats.GetStat(StatsId.Special).ToString());
        }

        if(m_currentLevel != null) {
            m_currentLevel.text = stats.Level.ToString();
        }

        if (m_experienceRequired != null) {
            m_experienceRequired.text = stats.NextLevelExp.ToString();
        }
    }
}
