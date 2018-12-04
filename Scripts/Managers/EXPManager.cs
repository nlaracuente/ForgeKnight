using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the player's EXP currency 
/// </summary>
public class EXPManager : MonoBehaviour
{
    /// <summary>
    /// A singleton reference
    /// </summary>
    public static EXPManager instance;

    /// <summary>
    /// A reference to the text component that displays the current EXP
    /// </summary>
    [SerializeField]
    Text m_currentEXPText;

    /// <summary>
    /// A reference to the text component that display the message for sacrificing exp
    /// </summary>
    [SerializeField]
    Text m_sacrificeText;

    /// <summary>
    /// A reference to the text component that displays the required EXP to do a sacrifice
    /// </summary>
    [SerializeField]
    Text m_requiredEXPText;

    /// <summary>
    /// Minimum required experience for the next level
    /// </summary>
    [SerializeField, Tooltip("Minimum required EXP for next level")]
    int m_baseEXP = 30;

    /// <summary>
    /// The exponent to determine next level experience cost
    /// </summary>
    [SerializeField, Tooltip("Next level experience cost multiplier")]
    float m_EXPMultiplier = 1.5f;

    /// <summary>
    /// The message that lets player know how much sacrifice costs and what it provides
    /// </summary>
    [SerializeField, Tooltip("Keep the {0} and {1} so that they can be changed into values")]
    string m_messageFormat = "Sacrifice {0} EXP to get {1}% more exp from defeated enemy units.";

    /// <summary>
    /// How many EXPs must be sacrificed to earn more
    /// </summary>
    [SerializeField]
    int m_sacrificeCost = 50;

    /// <summary>
    /// How much to increase the sacrifice each time it is consumed
    /// </summary>
    [SerializeField, Tooltip("Sacrifice cost increase in percentage")]
    float m_costMultiplier = 1.25f;

    /// <summary>
    /// How much EXP is rewarded when the player clicks to earn EXP
    /// </summary>
    [SerializeField, Tooltip("How much EXP to reward on click")]
    int m_clickEXP = 1;

    /// <summary>
    /// How much EXP each enemy will reward when defeated
    /// </summary>
    [SerializeField, Tooltip("Total EXP per defeated enemy")]
    int m_enemyEXP = 1;

    /// <summary>
    /// Enemy EXP reward multiplier
    /// </summary>
    [SerializeField, Range(.25f, 100f), Tooltip("How much each sacrifice increases enemy EXP in percentage")]
    float m_enemyEXPMultiplier = 2f;

    /// <summary>
    /// A reference to the sacrifice button that the player clicks to trigger a sacrifice
    /// </summary>
    [SerializeField]
    Button m_sacrificeButton;

    /// <summary>
    /// Stores the player's current EXP
    /// </summary>
    [SerializeField]
    int m_exp = 0;
    public int EXP { get { return m_exp; } }

	/// <summary>
    /// Sets up the singleton
    /// </summary>
	void Awake ()
    {
        instance = this;
	}

    /// <summary>
    /// Updates the experience ui display
    /// </summary>
    void LateUpdate()
    {
        if (m_currentEXPText != null) {
            m_currentEXPText.text = m_exp.ToString();
        }

        if (m_sacrificeText != null) {
            m_sacrificeText.text = string.Format(m_messageFormat, m_enemyEXPMultiplier);
        }

        if (m_requiredEXPText != null) {
            m_requiredEXPText.text = m_sacrificeCost.ToString();
        }

        // Enables/Disables the sacrifice button
        // base on the current total exp
        if(m_sacrificeButton != null) {
           m_sacrificeButton.interactable = CanConsumeEXP(m_sacrificeCost);
        }
    }

    /// <summary>
    /// True when the current EXP is greater or equal to the desired EXP
    /// </summary>
    /// <param name="exp"></param>
    /// <returns></returns>
    public bool CanConsumeEXP(int exp)
    {
        return exp <= m_exp;
    }

    /// <summary>
    /// Increases the total EXP by the given exp
    /// </summary>
    /// <param name="exp"></param>
    public void AddEXP(int exp)
    {
        m_exp += Mathf.Abs(exp);
    }

    /// <summary>
    /// As long as it can, it consumes the given EXP
    /// returning true when it was successful
    /// </summary>
    /// <param name="exp"></param>
    public bool ConsumeEXP(int exp)
    {
        bool consumed = false;
        exp = Mathf.Abs(exp);

        if (CanConsumeEXP(exp)) {
            consumed = true;
            m_exp -= exp;
        }

        return consumed;
    }

    /// <summary>
    /// Triggered when an enemy is defeated to increase experience
    /// </summary>
    public void EnemyDefeated()
    {
        AddEXP(m_enemyEXP);
    }

    /// <summary>
    /// Player made a sacrifice to earn more EXP
    /// increase the total EXP received per enemey defeated
    /// decreases total exp by sacrifice amount
    /// increases the costs for sacrifices
    /// </summary>
    public void SacrificeMade()
    {
        if (ConsumeEXP(m_sacrificeCost)) {
            m_sacrificeCost = (int)Mathf.Round(m_sacrificeCost * m_costMultiplier);
            m_enemyEXP = (int)Mathf.Ceil(m_enemyEXP * m_enemyEXPMultiplier);            
        }
    }

    /// <summary>
    /// Triggered by clicking on the button to earn more EXP
    /// </summary>
    public void OnClickToEarn()
    {
        AddEXP(m_clickEXP);
    }

    /// <summary>
    /// Triggers the given unit to level up when the unit's next level exp can be consumed
    /// </summary>
    /// <param name="unit"></param>
    public void LevelUp(PlayerUnit unit)
    {
        int exp = unit.Stats.nextLevelExp;        

        if (ConsumeEXP(exp)) {
            int level = unit.Stats.level + 1;
            int nextEXP = GetEXPForLevel(level + 1);
            
            unit.SetStatsToLevel(level, nextEXP);
        }
    }

    /// <summary>
    /// Returns the required experience to level up to the given level
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int GetEXPForLevel(int level)
    {
        return Mathf.FloorToInt(m_baseEXP * Mathf.Pow(level, m_EXPMultiplier));
    }
}
