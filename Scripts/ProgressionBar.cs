using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A simple class to uppdate the xscale of a progression bar
/// </summary>
public abstract class ProgressionBar : MonoBehaviour
{
    /// <summary>
    /// A reference to the sprite that represents the cooldown bar
    /// This is the one that moves to show progress
    /// </summary>
    [SerializeField]
    GameObject m_bar;

    /// <summary>
    /// The text component for this progression bar
    /// </summary>
    [SerializeField]
    protected Text m_barText;

    /// <summary>
    /// Returns the current progress in percent not exceeding 100%
    /// Otherwise the bar would go beyond its borders
    /// </summary>
    /// <returns></returns>
    float m_progress;
    protected float Progress
    {
        get { return Mathf.Clamp01(m_progress); }
        set { m_progress = value; }
    }

    /// <summary>
    /// Updates the local scale of the progression bar to show progress
    /// </summary>
    protected void UpdateBar()
    {
        if(m_bar != null) {
            Vector3 localScale = m_bar.transform.localScale;
            m_bar.transform.localScale = new Vector3(
                Progress,
                localScale.y,
                localScale.z
            );
        }
    }

    /// <summary>
    /// Sets the bar text to the given value
    /// </summary>
    /// <param name="text"></param>
    protected void UpdateText(string text)
    {
        if(m_barText != null) {
            m_barText.text = text;
        }
    }
}
