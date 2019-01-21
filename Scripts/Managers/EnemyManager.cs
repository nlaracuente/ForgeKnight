using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages all active enemies
/// </summary>
public class EnemyManager : MonoBehaviour
{
    /// <summary>
    /// Singleton
    /// </summary>
    public static EnemyManager instance;

    /// <summary>
    /// A collection of all active units
    /// </summary>
    List<EnemyUnit> m_units;
    List<EnemyUnit> Units
    {
        get {
            if (m_units == null) {
                m_units = new List<EnemyUnit>();
            }
            return m_units;
        }
        set {
            m_units = value;
        }
    }

    /// <summary>
    /// Set references
    /// </summary>
    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Adds the given unit to the list of active units
    /// </summary>
    /// <param name="unit"></param>
    public void AddUnit(EnemyUnit unit)
    {
        if(!Units.Contains(unit)) {
            Units.Add(unit);
            unit.IsActive = true;
        }
    }

    /// <summary>
    /// Removes the given unit to the list of active units
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveUnit(EnemyUnit unit)
    {
        if (Units.Contains(unit)) {
            Units.Remove(unit);
            unit.IsActive = false;
        }
    }

    /// <summary>
    /// Rather than using unity's Update() we use this one so that it can be controlled
    /// By the game manager and prevent updates from running when they are not needed
    /// </summary>
    public void RunUpdate()
    {
        Units.ForEach(unit => {
            if (unit.IsActive) {
                AIManager.instance.EnemyMoveLogic(unit);
                AIManager.instance.AttackLogic(unit);
            }
        });
    }

    /// <summary>
    /// Marks enemies as disables
    /// </summary>
    public void DisableEnemies()
    {
        Units.ForEach(unit => {
            unit.IsActive = false;
            unit.IsMoving = false;
        });
    }
}
