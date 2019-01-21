using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for all enemy units including bosses
/// </summary>
public abstract class EnemyUnit : Unit
{
    /// <summary>
    /// A reference to the health progress handler
    /// </summary>
    [SerializeField]
    public HealthProgressHandler m_healthBar;

    /// <summary>
    /// True while the unit is moving
    /// </summary>
    protected bool m_isMoving = false;

    /// <summary>
    /// For testing only
    /// Stores the enemy's original location to respawn them there
    /// </summary>
    Vector3 m_origin;

    /// <summary>
    /// True when the death animation has finished
    /// </summary>
    bool m_deathAnimationDone = false;

    /// <summary>
    /// Update the animation to display unit moving or not moving
    /// </summary>
    public bool IsMoving
    {
        get { return m_isMoving; }
        set {
            m_isMoving = value;
            m_animator.SetBool("IsMoving", value);
        }
    }

    /// <summary>
    /// Initializes the attack timer
    /// </summary>
    public override void Init()
    {
        m_origin = transform.position;
        m_targetLayer = 1 << LayerMask.NameToLayer("PlayerUnit");
    }

    /// <summary>
    /// Updates the UI
    /// </summary>
    protected override void OnStatsChanged()
    {
        if (m_healthBar != null) {
            m_healthBar.CurrentHP = Stats[StatsId.HP_Cur];
            m_healthBar.MaxHP = Stats[StatsId.HP_Max];
        }
    }

    /// <summary>
    /// Enemy grabs the first unit in sight and attacks it
    /// </summary>
    protected override void Attack()
    {
        SetAnimatorTrigger("Attack");
    }

    /// <summary>
    /// Peforms a normal short range attack
    /// This is so that enemies with projectiles can switch to non-projectile attacks
    /// </summary>
    protected void MeeleAttack()
    {
        List<Unit> targets = AIManager.instance.GetTargetsInRange(this);
        if (targets.Count > 0) {
            Unit target = targets[0];
            target.HurtAction(Damage);
        }

        ResetTimer(m_attackTimer);
    }

    /// <summary>
    /// Enemies do not have specials
    /// </summary>
    protected override void Special() { }

    /// <summary>
    /// Triggers death for testing purposes
    /// </summary>
    /// <param name="damage"></param>
    public override void HurtAction(int damage)
    {
        base.HurtAction(damage);
        
        // Defeated
        if (Stats[StatsId.HP_Cur] <= 0) {
            TriggerDeath();
        }
    }

    /// <summary>
    /// Disables all timers and triggers so that this unti does not attack
    /// Sets the unit as inactive so that it cannot be interacted with
    /// Rewards the player with EXP
    /// Trigger the death animation
    /// Waits until the death animation is done
    /// </summary>
    protected void TriggerDeath()
    {
        IsActive = false;

        // Stops all timers
        foreach (Collider2D collider in GetComponentsInChildren<Collider2D>()) {
            collider.enabled = false;
        }

        // Reward the player with EXP
        EXPManager.instance.EnemyDefeated();

        StartCoroutine(DeathRoutine());
    }

    /// <summary>
    /// Triggers death animation 
    /// Waits until its done and destroys this enemy
    /// Notifies the wave manager to remove it from the active enemy list
    /// </summary>
    /// <returns></returns>
    protected IEnumerator DeathRoutine()
    {
        if (m_animator != null) {
            m_deathAnimationDone = false;
            SetAnimatorTrigger("Death");
        }

        while (!m_deathAnimationDone && !GameManager.instance.GameOver) {
            yield return null;
        }
        
        WaveManager.instance.EnemyDefeated(this);
        Destroy(gameObject);
    }

    /// <summary>
    /// Called by the animator to indicate death animation is done
    /// </summary>
    public void DeathAnimationCompleted()
    {
        m_deathAnimationDone = true;
    }

    public void DestroyNow()
    {
        IsActive = false;
        Destroy(gameObject);
    }
}
