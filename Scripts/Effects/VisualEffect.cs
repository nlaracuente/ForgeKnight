using UnityEngine;
using System.Collections;

/// <summary>
/// A visual effects plays out an animation and once the animation is completed
/// It removes itself
/// </summary>
public class VisualEffect : MonoBehaviour
{
    /// <summary>
    /// True when the animation is done
    /// </summary>
    bool m_isDone = false;

    /// <summary>
    /// Initialize routine
    /// </summary>
    void Start()
    {
        StartCoroutine(PlayEffectRoutine());        
    }

    /// <summary>
    /// Waits for the animation to done before destroying itself
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayEffectRoutine()
    {
        while(!m_isDone) {
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Triggered by the animator to indicate animation is done
    /// </summary>
    public void IsDone()
    {
        m_isDone = true;
    }
}
