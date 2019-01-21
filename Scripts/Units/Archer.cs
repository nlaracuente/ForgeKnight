using System.Collections;
using UnityEngine;

/// <summary>
/// Archer has a long range attacks aginst a single unit and 
/// a medium range special that can attack multiple units
/// </summary>
public class Archer : PlayerUnit
{
    /// <summary>
    /// The projectile to use for specile attacks
    /// </summary>
    [SerializeField]
    Projectile m_specialProjectile;

    /// <summary>
    /// Increaes the attack of the normal projectile for SPECIAL attacks 
    /// </summary>
    [SerializeField]
    float m_specialProjectileMultiplier = 1.5f;

    /// <summary>
    /// True when the special attack animation is at the point to fire off the projectile
    /// </summary>
    bool m_fireSpecialProjectile = false;

    /// <summary>
    /// Spawns an arrow to attack enemies with
    /// </summary>
    protected override void Attack()
    {
        StartCoroutine(FireProjectileRoutine(Vector3.right, typeof(EnemyUnit)));
    }

    /// <summary>
    /// Triggers the special move and resets the timer as specials
    /// do not require units to be close enough
    /// </summary>
    protected override void Special()
    {
        StartCoroutine(SpecialProjectileRoutine(Vector3.right));
    }

    /// <summary>
    /// Spawns and fires the unit's projectile when the animation tells it to
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    protected IEnumerator SpecialProjectileRoutine(Vector3 direction)
    {
        // Wait to spawn/fire projectile
        m_fireSpecialProjectile = false;

        // Play and wait for the animation
        SetAnimatorTrigger("Special");
        while (!m_fireSpecialProjectile) {
            yield return new WaitForEndOfFrame();
        }

        GameObject go = GameManager.instance.SpawnProjectile(m_specialProjectile, m_projectileSpawnPoint);

        if (go != null && go.GetComponent<Projectile>() != null) {
            int power = (int)(Stats.GetStat(StatsId.Attack) * m_specialProjectileMultiplier);
            Projectile projectile = go.GetComponent<Projectile>();
            projectile.Fire(direction, power, typeof(EnemyUnit));
        }

        ResetTimer(m_specialTimer);
    }

    /// <summary>
    /// Called by the animator to trigger the projectile to spawn/move
    /// </summary>
    public void FireSpecialProjectile()
    {
        m_fireSpecialProjectile = true;
    }
}
