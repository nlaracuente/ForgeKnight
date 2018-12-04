using UnityEngine;
using System.Collections;

/// <summary>
/// Handles processing progress from current to maximum value
/// </summary>
public interface IProgressHandler
{
    /// <summary>
    /// Returns a number between 0f and 1f that represents the current progress
    /// </summary>
    float Progress { get; }

    /// <summary>
    /// Returns a text representation of the current progress
    /// </summary>
    string Summary { get; }
}
