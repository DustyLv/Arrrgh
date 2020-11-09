using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SliderToTextUI : MonoBehaviour
{
    public TextMeshProUGUI m_Text;

    void Start()
    {
        m_Text.text = "1";
    }

    public void UpdateTextFromSlider(float input)
    {
        m_Text.text = input.ToString("F2");
    }
}
