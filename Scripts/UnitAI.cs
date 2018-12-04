using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Units performs actions automatically based on conditions
/// These is the interface for defining those conditions that the AI
/// uses when determining if an action should be triggered
/// </summary>
abstract class UnitAI : MonoBehaviour
{
    /// <summary>
    /// A collection of all the cool downs to process
    /// </summary>
    List<Cooldown> m_cooldowns;

    /// <summary>
    /// Returns the cooldown for triggering attacks
    /// </summary>
    /// <returns></returns>
    protected abstract Cooldown GetAttackCooldown();

    /// <summary>
    /// Returns the cooldown for triggering moving
    /// </summary>
    /// <returns></returns>
    protected abstract Cooldown GetMovementCooldown();

    /// <summary>
    /// Returns the cooldown for triggering attacks
    /// </summary>
    /// <returns></returns>
    protected abstract Cooldown GetHealingCooldown();

    /// <summary>
    /// Builds the list of cooldowns for this unit
    /// </summary>
    void SetCoolDowns()
    {
        m_cooldowns = new List<Cooldown>() {
            GetAttackCooldown(),
            GetMovementCooldown(),
            GetHealingCooldown(),
        };
    }


    /// <summary>
    /// Handles the rotuine for when the unit is idled
    /// </summary>
    /// <returns></returns>
    IEnumerator IdleRoutine()
    {
        yield return null;
    }
}

/// <summary>
/// A cooldown has a counter and a target to reach which can then be used to trigger an action
/// </summary>
public struct Cooldown
{
    /// <summary>
    /// Where the cooldown is currently at
    /// </summary>
    public float counter;

    /// <summary>
    /// The target value the counter needs to reach to trigger the action
    /// </summary>
    public float target;
}