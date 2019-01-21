using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Knight is the party's "tank" as he can both deal and take
/// large amount of damage and is the key component to winning the game.
/// If the Kight dies, the round is over.
/// </summary>
[RequireComponent(typeof(CoolDownTimer))]
public class Knight : PlayerUnit
{
    /// <summary>
    /// Total enemies it can attack at once
    /// </summary>
    [SerializeField]
    int m_totalUnitsToAttack = 5;

    /// <summary>
    /// Trigger game over when the knight is no longer alive
    /// </summary>
    void Update()
    {
        if(IsActive && Stats[StatsId.HP_Cur] <= 0) {
            GameManager.instance.TriggerGameOver();
        }
    }

    /// <summary>
    /// Attacks
    /// </summary>
    protected override void Attack()
    {
        List<Unit> targets = AIManager.instance.GetTargetsInRange(this);
        if (targets.Count > 0) {
            SetAnimatorTrigger("Attack");

            int total = Math.Min(m_totalUnitsToAttack, targets.Count - 1);

            foreach (Unit unit in targets.GetRange(0, total)) {
                if(unit != null && unit.IsActive) {
                    unit.HurtAction(Damage);
                }
            }
        }

        ResetTimer(m_attackTimer);
    }

    /// <summary>
    /// No special
    /// </summary>
    protected override void Special()
    {
        
    }
}
