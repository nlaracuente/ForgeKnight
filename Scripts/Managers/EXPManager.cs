using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the current accumulated experience point, the required experience to level up,
/// and the ui to show experience points and option to sacrafice them
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
    /// A reference to the text component that displays how much EXP is earned by clicking
    /// </summary>
    [SerializeField]
    Text m_clickToEarnText;

    /// <summary>
    /// Minimum required experience for the next level
    /// </summary>
    [SerializeField, Tooltip("Minimum required EXP for next level")]
    int m_baseExp = 30;

    /// <summary>
    /// The exponent to determine next level experience cost
    /// </summary>
    [SerializeField, Tooltip("Next level experience cost multiplier")]
    float m_exponent = 1.5f;

    /// <summary>
    /// Required experience point sacrifice to increase total EXP received
    /// </summary>
    [SerializeField]
    int m_sacrificeCost = 50;

    /// <summary>    
    /// How much EXP is rewarded when the player clicks to earn EXP
    /// </summary>
    [SerializeField, Tooltip("How much EXP to reward on click")]
    int m_clickEXP = 1;
    public int ClickExp { get{ return m_clickEXP; } }

    /// <summary>
    /// Used to help determine the cost required to further increase
    /// the exp per clicks next level cost
    /// </summary>
    int m_clickExpLvl = 1;

    /// <summary>
    /// How much EXP each enemy will reward when defeated
    /// </summary>
    [SerializeField, Tooltip("Total EXP per defeated enemy")]
    int m_enemyEXP = 1;

    /// <summary>
    /// EXP per clicks
    /// </summary>
    [SerializeField]
    float m_expPerCliksMultiplier = 1.25f;

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
    /// The message that lets player know how much sacrifice costs and what it provides
    /// </summary>
    [SerializeField]
    string m_messageFormat = "Sacrifice {0} EXP to get {1} more exp from defeated enemy units.";

    /// <summary>
    /// The message that lets player know how much experience per click they earn
    /// </summary>
    [SerializeField]
    string m_clickToEarnFormat = "Click on screen to earn {0} Exp";

    /// <summary>
    /// Sets up the singleton
    /// </summary>
    void Awake ()
    {
        instance = this;
        m_sacrificeCost = NextLevelEXP(m_clickExpLvl++);
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
            m_sacrificeText.text = string.Format(m_messageFormat, m_expPerCliksMultiplier);
        }

        if (m_requiredEXPText != null) {
            m_requiredEXPText.text = m_sacrificeCost.ToString();
        }

        if (m_clickToEarnText != null) {
            m_clickToEarnText.text = string.Format(m_clickToEarnFormat, m_clickEXP.ToString());
        }

        // Enables/Disables the sacrifice button
        // base on the current total exp
        if (m_sacrificeButton != null) {
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
    /// Given the wave number updates the exp rewarded by defeated enemies
    /// </summary>
    /// <param name="wave"></param>
    public void UpdateEnemyExpForWave(int wave)
    {
        //m_enemyEXP = NextSacrificeExp(wave);
        m_enemyEXP = wave;
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
            m_clickEXP = Mathf.CeilToInt(m_clickEXP * m_expPerCliksMultiplier);
            m_sacrificeCost = NextLevelEXP(m_clickExpLvl++);
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
    /// Triggers the given unit to level up when the current experience points 
    /// equals to or is greater than the unit's required experience to level up
    /// </summary>
    /// <param name="unit"></param>
    public void PlayerUnitLevelUp(PlayerUnit unit)
    {
        int exp = unit.Stats.NextLevelExp;

        if (ConsumeEXP(exp)) {
            unit.TriggerLevelUp();
        }
    }

    /// <summary>
    /// Returns the required experience to level up to the given level
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int NextLevelEXP(int level, int baseExp = 10)
    {
        float exp = (float)Math.Floor(baseExp * Math.Pow(level, m_exponent));
        return Mathf.RoundToInt(exp);
    }

    /// <summary>
    /// Pokemon style experience
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int NextSacrificeExp(int level)
    {
        float exp = (float)Math.Round(4 * Math.Pow(level, m_exponent)) / 5;
        return Mathf.RoundToInt(exp);
    }
}
