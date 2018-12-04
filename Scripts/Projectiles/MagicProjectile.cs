using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicProjectile : Projectile
{
    /// <summary>
    /// The explosion visual effect to play when the projectile hits the ground
    /// </summary>
    [SerializeField]
    VisualEffect m_explosionEffect;

    /// <summary>
    /// Override the routine so tha it does not stop moving until a boundary
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    protected override IEnumerator MoveRoutine(Vector3 direction)
    {
        while (!m_collidedWithBoundary) {
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
    /// Triggers the break logic
    /// </summary>
    protected override void Break()
    {
        StartCoroutine(BreakRoutine());
    }

    /// <summary>
    /// Waits until it hits boundary to trigger the visual effect
    /// Disables the sprite renderer so that it is not visible
    /// Spawns the effect and waits for it to be destroyed
    /// Then destroys itself
    /// This is so that it can cause "splash" damage while the visual effect is playing
    /// </summary>
    /// <returns></returns>
    IEnumerator BreakRoutine()
    {
        while(!m_collidedWithBoundary) {
            yield return new WaitForEndOfFrame();
        }

        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        if (renderer != null) {
            renderer.enabled = false;
        }

        GameObject go = Instantiate(m_explosionEffect.gameObject, transform, true);
        go.transform.position = transform.position;

        // GO destroyes itself once it is done
        while(go != null) {
            yield return new WaitForEndOfFrame();
        }
        
        Destroy(gameObject);
    }
}
