using UnityEngine;

/// <summary>
/// Handles user click to earn experience
/// </summary>
public class ClickToEarnExp : MonoBehaviour
{
    /// <summary>
    /// The visual effect for display hp gained or lost
    /// </summary>
    [SerializeField]
    GameObject m_prefab;

    /// <summary>
    /// The text color
    /// </summary>
    [SerializeField]
    Color m_color = Color.white;

    /// <summary>
    /// Spawns the text that shows total exp earned when clicked
    /// </summary>
    /// <param name="exp"></param>
    /// <param name="color"></param>
    public void OnClick()
    {
        EXPManager.instance.OnClickToEarn();

        if (m_prefab != null) {
            string text = string.Format("+{0} exp", EXPManager.instance.ClickExp);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            GameObject go = Instantiate(m_prefab, transform, true);
            go.transform.position = mousePos;

            HPChangeDisplay display = go.GetComponentInChildren<HPChangeDisplay>();
            display.SetText(text.ToString());
            display.SetColor(m_color);
        }
    }
}
