using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenRotate : MonoBehaviour
{
    public float m_RotationSpeed = 1f;
    public Vector3 m_Axis = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, m_Axis, m_RotationSpeed);
    }
}
