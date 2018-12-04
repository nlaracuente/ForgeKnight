using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// These are the numbers that appear on the unit whose HP was changed 
/// either by taking damage or being healed
/// </summary>
public class HPChangeDisplay : VisualEffect
{
    /// <summary>
    /// A reference to the Text component
    /// </summary>
    [SerializeField]
    Text m_text;

    /// <summary>
    /// Changes the text value
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string text)
    {
        if (m_text != null) {
            m_text.text = text;
        }
    }

    /// <summary>
    /// Changes the text color
    /// </summary>
    /// <param name="color"></param>
    public void SetColor(Color color)
    {
        if (m_text != null) {
            m_text.color = color;
        }
    }
}
