using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Associated with a unit that it can level up
/// Enables/Disables itself when there's enough EXP to level up the unit
/// Handles calling EXPManager when a request to level up has happened
/// </summary>
[RequireComponent(typeof(Button))]
public class LevelUpButton : MonoBehaviour, IPointerUpHandler
{
    /// <summary>
    /// The associated player unit
    /// </summary>
    [SerializeField]
    PlayerUnit m_unit;

    /// <summary>
    /// A reference to the button this is bound to
    /// </summary>
    [SerializeField]
    Button m_button;

    /// <summary>
    /// Initialize
    /// </summary>
    void Start()
    {
        if(m_button == null) {
            m_button = GetComponent<Button>();
        }

        // Default to disabled
        m_button.interactable = false;
    }

    /// <summary>
    /// Enables/Disables the button based on current EXP
    /// </summary>
    void Update()
    {
        if(m_button != null) {
            int exp = m_unit.Stats.NextLevelExp;
            m_button.interactable = EXPManager.instance.CanConsumeEXP(exp);
        }
    }

    /// <summary>
    /// Triggers a level up request
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (m_button != null) {
            EXPManager.instance.PlayerUnitLevelUp(m_unit);
        }
    }
}
