using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

public class PunchScale : MonoBehaviour
{
    public float m_PunchStrength = 1.5f;
    public float m_PunchLength = 0.2f;
    public int m_PunchVibrato = 10;
    public float m_PunchElasticity = 1f;

    [Button("Test")]
    public void DoPunch()
    {
        transform.DOPunchScale(transform.localScale * m_PunchStrength, m_PunchLength, m_PunchVibrato, m_PunchElasticity);
        //transform.DOShakeScale(m_PunchLength, m_PunchStrength, 10, 50f, true).SetEase(Ease.InOutSine);
        //transform.Dopunch
    }
}
