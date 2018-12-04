using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Handles reporting the progress of a unit's health
/// </summary>
public class HealthProgressHandler : MonoBehaviour, IProgressHandler 
{
    /// <summary>
    /// A reference to the progress bar that represents this health progress
    /// </summary>
    [SerializeField]
    ProgressBar m_bar;

    /// <summary>
    /// Initialize
    /// </summary>
    void Start()
    {
        if(m_bar != null) {
            m_bar.Handler = this;
        }
    }

    /// <summary>
    /// Unit's current HP
    /// </summary>
    [SerializeField]
    public float CurrentHP { set; get; }

    /// <summary>
    /// Unit's maximum HP
    /// </summary>
    public float MaxHP { set; get; }

    /// <summary>
    /// Returns the current healt perentage
    /// </summary>
    public float Progress
    {
        get {
            float progress = 0f;

            // Avoid division by 0
            if (CurrentHP > 0) {
                progress = CurrentHP / MaxHP;
            }

            return progress;
        }
    }

    /// <summary>
    /// Returns current and max health summary
    /// </summary>
    public string Summary
    {
        get {
            string summary = CurrentHP.ToString();
            return summary;
        }
    }
}
