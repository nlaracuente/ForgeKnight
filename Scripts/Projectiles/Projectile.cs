using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Projectiles are attacks initiated by a unit that move in a given direction
/// until collision with a target unit type. Projectiles have endurance which 
/// dictates how many units it can hit and ony hits once per unit.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    /// <summary>
    /// The movement speed of this projectile
    /// </summary>
    [SerializeField, Range(0f, Mathf.Infinity), Tooltip("How fast the projectile travels")]
    protected float m_speed = 1f;

    /// <summary>
    /// Keeps track of the units already hit
    /// </summary>
    protected List<Unit> m_hits;

    /// <summary>
    /// How much damage to deal on collision
    /// </summary>
    protected int m_power = 1;

    /// <summary>
    /// How many targets it can hit before breaking
    /// </summary>
    [SerializeField]
    protected int m_endurance = 1;

    /// <summary>
    /// The target unit type to detect collision with
    /// </summary>
    protected Type m_targetType;

    /// <summary>
    /// True when the projectile can no longer endure hits
    /// </summary>
    public bool IsBroken { get; set; }

    /// <summary>
    /// True after the initial fire is triggerd
    /// </summary>
    bool Fired { get; set; }

    /// <summary>
    /// True once it hits a boundary
    /// </summary>
    protected bool m_collidedWithBoundary = false;

    /// <summary>
    /// Initialize
    /// </summary>
    void Start()
    {
        IsBroken = false;
        Fired = false;
        m_hits = new List<Unit>();
    }

    /// <summary>
    /// Checks if it collided with the target and trigger a hit
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        // Already broken, don't bother
        if (IsBroken) {
            return;
        }

        Unit unit = other.gameObject.GetComponent<Unit>();
        
        if (unit != null) {
            bool isTarget = unit.GetType().IsSubclassOf(m_targetType);
            if (isTarget) {
                HitTarget(unit);
            }
        }
    }

    /// <summary>
    /// Triggers the unit's hurt state so long as it has not been hit
    /// by this projectile already
    /// Triggers broken state if this is the last hit it can endure
    /// </summary>
    /// <param name="unit"></param>
    void HitTarget(Unit unit)
    {
        if (!m_hits.Contains(unit)) {
            m_hits.Add(unit);
            unit.HurtAction(m_power);
        }

        if (m_hits.Count >= m_endurance) {
            IsBroken = true;
        }
    }

    /// <summary>
    /// Fires this projectile once which will cotinue to move until broken
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="power"></param>
    /// <param name="targetType"></param>
	public void Fire(Vector3 direction, int power, Type targetType)
    {
        if (!Fired) {
            Fired = true;
            m_power = power;
            m_targetType = targetType;
            StartCoroutine(MoveRoutine(direction));
        }
    }

    /// <summary>
    /// Moves the projectile in the given direction each frame
    /// while the <see cref="IsBroken"/> flag is true.
    /// One the flag is not true then the projectile is destroyed
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    protected virtual IEnumerator MoveRoutine(Vector3 direction)
    {
        while (!IsBroken && !m_collidedWithBoundary) {
            transform.position = Vector3.MoveTowards(
                transform.position,
                transform.position + direction,
                m_speed * Time.deltaTime
            );
            yield return new WaitForEndOfFrame();
        }

        Break();
    }

    /// <summary>
    /// Trigger by the boundary triggers
    /// </summary>
    public void BoundaryCollision()
    {
        m_collidedWithBoundary = true;
    }

    /// <summary>
    /// Destroyes the projectile
    /// </summary>
    protected virtual void Break()
    {
        Destroy(gameObject);
    }
}
