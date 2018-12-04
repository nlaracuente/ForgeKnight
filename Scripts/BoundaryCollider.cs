using UnityEngine;
using System.Collections;

/// <summary>
/// Checks for collisions with projectiles to destroy them
/// This is so that projectiles that miss enemies get recycled
/// </summary>
public class BoundaryCollider : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Projectile projectile = other.GetComponent<Projectile>();
        if(projectile != null) {
            projectile.BoundaryCollision();
        }
    }
}
