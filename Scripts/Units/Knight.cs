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
    /// Trigger game over when the knight is no longer alive
    /// </summary>
    void Update()
    {
        if(Stats.hp <= 0) {
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
            Unit target = targets[0];
            target.HurtAction(Damage);
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
