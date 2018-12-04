using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is the UI representation of a cooldown timer that displays its progress
/// </summary>
public class CooldownProgressBar : ProgressionBar
{
    /// <summary>
    /// Shows progression
    /// </summary>
    void Update()
    {
        UpdateBar();
    }

    /// <summary>
    /// Triggered by the timer to notify of current progress
    /// </summary>
    /// <param name="progress"></param>
    public void OnTimerIncremented(float progress)
    {
        Progress = progress;
    }

    /// <summary>
    /// Gets and sets the text shows the current progress
    /// </summary>
    public void SetBarText(string text)
    {
        UpdateText(text);
    }
}
