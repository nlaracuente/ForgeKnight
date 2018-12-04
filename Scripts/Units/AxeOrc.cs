using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeOrc : EnemyUnit
{
    /// <summary>
    /// Spawns an arrow to attack enemies with
    /// There's a bug with the triggers getting fired too quickly
    /// Turning this enemy into melee for the time being
    /// </summary>
    //protected override void Attack()
    //{
    //    Debug.Log("Axe Orc attack");
    //    // Throw an axe
    //    if (IsMoving) {
    //        StartCoroutine(FireProjectileRoutine(Vector3.left, typeof(PlayerUnit)));
    //    } else {
    //        MeeleAttack();
    //    }
    //}
}
