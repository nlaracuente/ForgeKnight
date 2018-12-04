using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Alchemist plays the role of support healing the Knight and providing mid-range low
/// level attacks with splash damage
/// </summary>
public class Mage : PlayerUnit
{
    /// <summary>
    /// A collection of all the places where to spawn the mage's attack
    /// The attacks all move in the same direction therefore the points
    /// should reflect that but we don't check for that
    /// </summary>
    [SerializeField]
    Transform[] m_spawnPoints;

    /// <summary>
    /// Triggers the special timer to start
    /// </summary>
    public override void Init()
    {
        base.Init();
        AIManager.instance.ResumeTimer(m_specialTimer);
    }

    /// <summary>
    /// Triggers customer fire projectile routine  
    /// </summary>
    protected override void Attack()
    {
        StartCoroutine(FireProjectileRoutine(Vector3.down, typeof(EnemyUnit)));
    }

    /// <summary>
    /// Overrides as we want to spawn a projectile for each spawnpoint we have
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    protected override IEnumerator FireProjectileRoutine(Vector3 direction, Type targetType)
    {
        // Wait to spawn/fire projectile
        m_fireProjectile = false;
        SetAnimatorTrigger("Attack");
        while (!m_fireProjectile) {
            yield return new WaitForEndOfFrame();
        }

        foreach (Transform spawnPoint in m_spawnPoints) {
            GameObject go = GameManager.instance.SpawnProjectile(m_projectile, spawnPoint);

            if (go != null && go.GetComponent<Projectile>() != null) {
                Projectile projectile = go.GetComponent<Projectile>();
                projectile.Fire(direction, Stats.attackPower, targetType);
            }
        }

        ResetTimer(m_attackTimer);
    }

    /// <summary>
    /// Triggers a party heal
    /// Resets the timer so that healing can continue
    /// </summary>
    protected override void Special()
    {
        SetAnimatorTrigger("Special");
        PartyManager.instance.HealParty(Stats.specialPower);
        ResetTimer(m_specialTimer);
        AIManager.instance.ResumeTimer(m_specialTimer);
    }
}
