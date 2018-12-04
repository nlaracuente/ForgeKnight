using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles AI behavior for all units
/// AI in this game is simple as the unit can either do or not do an action
/// Hence why a centralized location to manage both player and enemy units
/// 
/// [Player Units]
/// Resume attack timers
/// 
/// [Enemy Units]
/// Move toward player
/// Resume attack timers
/// </summary>
public class AIManager : MonoBehaviour
{
    /// <summary>
    /// Singleton
    /// </summary>
    public static AIManager instance;

    /// <summary>
    /// Contains the point where the enemy needs to walk towards
    /// </summary>
    [SerializeField]
    Transform m_enemyDestination;

    /// <summary>
    /// How close to the destination before snapping to position
    /// </summary>
    [SerializeField]
    float m_proximity = 0.01f;

    /// <summary>
    /// Raycast distance for short range weapons
    /// </summary>
    [SerializeField]
    float m_shortRange = 1f;

    /// <summary>
    /// Raycast distance for medium range weapons
    /// </summary>
    [SerializeField]
    float m_mediumRange = 2f;

    /// <summary>
    /// Raycast distance for long range weapons
    /// </summary>
    [SerializeField]
    float m_longRange = 3f;

    /// <summary>
    /// Sets the reference
    /// </summary>
    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Handles the movement logic for an enemy unit
    /// </summary>
    /// <param name="unit"></param>
    public void EnemyMoveLogic(EnemyUnit unit)
    {
        Vector3 destination = new Vector3(
            m_enemyDestination.position.x,
            unit.transform.position.y,
            unit.transform.position.z
        );

        // Already there - no need to proceed
        if(!IsAtDestination(unit, destination)) {
            MoveEnemyUnit(unit, destination);
        }
    }

    /// <summary>
    /// Returns true if the unit's current position is at or close to the given position
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    bool IsAtDestination(Unit unit, Vector3 destination)
    {
        float distance = Vector3.Distance(unit.transform.position, destination);
        return distance <= m_proximity;
    }

    /// <summary>
    /// Moves the unit closer to the destination snapping into position once it is close enough
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="destination"></param>
    void MoveEnemyUnit(EnemyUnit unit, Vector3 destination)
    {
        Vector3 position = Vector3.MoveTowards(unit.transform.position, destination, unit.Stats.movementSpeed * Time.deltaTime);
        unit.transform.position = position;
        unit.IsMoving = true;

        // Made it
        if (IsAtDestination(unit, destination)) {
            unit.transform.position = destination;
            unit.IsMoving = false;
        }
    }

    /// <summary>
    /// Handles the attack logic for all types of units
    /// If a unit's target is within their weapon's range
    /// then it triggers their attack timers to either start or resume
    /// </summary>
    /// <param name="unit"></param>
    public void AttackLogic(Unit unit)
    {
        List<Unit> targets = GetTargetsInRange(unit);

        if (targets.Count > 0) {
            ResumeTimer(unit.AttackTimer);
        } else {
            PauseTimer(unit.AttackTimer);
        }
    }

    /// <summary>
    /// Returns a list of all the targets available within the unit's range
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public List<Unit> GetTargetsInRange(Unit unit)
    {
        float distance = 0f;
        List<Unit> targets = new List<Unit>();        

        switch (unit.Range) {
            case WeaponRange.SHORT:
                distance = m_shortRange;
                break;
            case WeaponRange.MEDIUM:
                distance = m_mediumRange;
                break;
            case WeaponRange.LONG:
                distance = m_longRange;
                break;
        }

        // Defaults to the direction an enemy faces
        Vector3 direction = Vector3.left;

        // Updates to a plyer unit's direction
        if (unit.GetType().IsSubclassOf(typeof(PlayerUnit))) {
            direction = Vector3.right;
        }

        // Unit could become inactive before this point
        // therefore we will not try to access its properties
        if (unit.IsActive) {
            // Raises up the position of the raycast
            Vector3 position = new Vector3(
                unit.transform.position.x,
                unit.transform.position.y + .25f,
                unit.transform.position.z
            );

            Debug.DrawRay(position, direction * distance, Color.red);
            RaycastHit2D[] hits = Physics2D.RaycastAll(position, direction, distance, unit.TargetLayer);

            foreach (RaycastHit2D hit in hits) {
                Unit target = hit.collider.GetComponent<Unit>();
                if (target != null) {
                    targets.Add(target);
                }
            }
        }

        return targets;
    }

    /// <summary>
    /// Ensures the timer routine is stopped, resets it, and restarts it
    /// </summary>
    /// <param name="timer"></param>
    public void ResumeTimer(CoolDownTimer timer)
    {
        // Restart timer where it was 
        if (timer != null && !timer.IsRunning) {
            StartCoroutine(timer.TimerRoutine());
        }        
    }

    /// <summary>
    /// Stops the coroutine for the timer so that it no longer increases
    /// However, it does keep its current state so that it can be resumed
    /// </summary>
    /// <param name="timer"></param>
    public void PauseTimer(CoolDownTimer timer)
    {
        if (timer != null && timer.IsRunning) {
            timer.StopTimer();
            StopCoroutine(timer.TimerRoutine());
        }
    }
}
