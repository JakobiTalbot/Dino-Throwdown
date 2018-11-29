using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// highlights toggles
public class ToggleHighlighted : MonoBehaviour
{
    public GameObject m_highlight;

    public void OnSelect()
    {
        m_highlight.SetActive(true);
    }
    public void OnDeselect()
    {
        m_highlight.SetActive(false);
    }
}