using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class IndicationArrow : MonoBehaviour
{
    public Transform m_ArrowGraphics;
    public bool m_AutoPlay = true;

    private Sequence m_Sequence;

    public static IndicationArrow i;

    private void Awake()
    {
        i = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Sequence = DOTween.Sequence();
        SetUpSequence();
        if (m_AutoPlay) Tween_Play();
    }

    private void SetUpSequence()
    {
        m_Sequence.Append(m_ArrowGraphics.DOLocalMoveY(0.05f, 0.5f));
        m_Sequence.Append(m_ArrowGraphics.DOLocalMoveY(0f, 0.15f));
        m_Sequence.SetLoops(-1);
    }

    public void Tween_Play()
    {
        m_Sequence.Play();
    }

    public void Tween_Pause()
    {
        m_Sequence.Pause();
    }

    public void Move(Transform target)
    {
        transform.DOMove(target.position, 0.1f);
        
    }

}
