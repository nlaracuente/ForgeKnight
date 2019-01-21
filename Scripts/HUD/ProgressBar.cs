using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// A progress bar is a visual representation of some sort of progress, handled by a progress handler,
/// that is visually displayed in the UI system
/// </summary>
public class ProgressBar : MonoBehaviour
{
    /// <summary>
    /// A referene to the progress handler
    /// </summary>
    [SerializeField]
    IProgressHandler m_handler;
    public IProgressHandler Handler { set { m_handler = value; } }

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
    /// Triggers the progress bar visualization
    /// </summary>
    void Update()
    {
        UpdateBar();
        UpdateText();
    }

    /// <summary>
    /// Updates the local scale of the progression bar with the current progress
    /// </summary>
    protected void UpdateBar()
    {
        if (m_bar != null && m_handler != null) {
            Vector3 localScale = m_bar.transform.localScale;
            m_bar.transform.localScale = new Vector3(
                m_handler.Progress,
                localScale.y,
                localScale.z
            );
        }
    }

    /// <summary>
    /// Updates the bar's text with the current progress summary
    /// </summary>
    /// <param name="text"></param>
    protected void UpdateText()
    {
        if (m_barText != null && m_handler != null) {
            m_barText.text = m_handler.Summary;
        }
    }
}
