using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineOnMouseOver : MonoBehaviour
{
    private Outline m_Outline;

    // Start is called before the first frame update
    void Start()
    {
        m_Outline = GetComponent<Outline>();
        m_Outline.enabled = false;
    }

    private void OnMouseOver()
    {
        if (m_Outline.enabled == false)
        {
            m_Outline.enabled = true;
        }
    }

    private void OnMouseExit()
    {
        m_Outline.enabled = false;
    }
}
