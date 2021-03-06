﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KnobRotate : MonoBehaviour
{
    public Vector3 currentRotation;

    public TextMeshProUGUI outputText;


    public float _sensitivity = 1f;

    private Vector3 _rotation;

    private float m_StartEndRotation = 150f;

    public RPMControlBox _rpmControlScript;

    void Start()
    {

        _rotation = new Vector3(0f, -m_StartEndRotation, 0f);
        transform.rotation = Quaternion.Euler( _rotation);

    }

    //void Update()
    //{
    //    currentRotation = new Vector3(WrapAngle(transform.localRotation.eulerAngles.x), WrapAngle(transform.localRotation.eulerAngles.y), WrapAngle(transform.localRotation.eulerAngles.z));
            
        
    //    if (_isRotating)
    //    {
    //        if(currentRotation.y > _startEndRotation)
    //        {
    //            currentRotation.y = _startEndRotation;
    //        }

    //        // offset
    //        _mouseOffset = (Input.mousePosition - _mouseReference);

    //        // apply rotation
    //        _rotation.y = -(_mouseOffset.x + _mouseOffset.y) * _sensitivity;

    //        // rotate
    //        transform.Rotate(_rotation);

    //        // store mouse
    //        _mouseReference = Input.mousePosition;

    //        if (_motorRotateScript.m_IsOn)
    //        {
    //            float rpm = currentRotation.y.Remap(-_startEndRotation, _startEndRotation, 0f, _motorRotateScript.m_MaxMotorRPM);
    //            outputText.text = rpm.ToString("F0");
    //            _motorRotateScript.SetRPM(rpm);
    //        }
    //    }
        
    //}

    //void OnMouseDown()
    //{
    //    // rotating flag
    //    _isRotating = true;

    //    // store mouse
    //    _mouseReference = Input.mousePosition;
    //}

    //void OnMouseUp()
    //{
    //    // rotating flag
    //    _isRotating = false;
    //}
    private float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }
    private float UnwrapAngle(float angle)
    {
        if (angle >= 0)
            return angle;

        angle = -angle % 360;

        return 360 - angle;
    }


    private void OnMouseDrag()
    {
        currentRotation = new Vector3(WrapAngle(transform.localRotation.eulerAngles.x), WrapAngle(transform.localRotation.eulerAngles.y), WrapAngle(transform.localRotation.eulerAngles.z));


        float XaxisRotation = Input.GetAxis("Mouse X") * _sensitivity;
        transform.Rotate(Vector3.down, XaxisRotation);

        if (currentRotation.y > m_StartEndRotation)
        {
            currentRotation.y = m_StartEndRotation;
            transform.localRotation = Quaternion.Euler(new Vector3(0f, UnwrapAngle(m_StartEndRotation), 0f));
        }

        if (currentRotation.y < -m_StartEndRotation)
        {
            currentRotation.y = -m_StartEndRotation;
            transform.localRotation = Quaternion.Euler(new Vector3(0f, UnwrapAngle(-m_StartEndRotation), 0f));
        }

        //if (_rpmControlScript.m_IsOn)
        //{
            float rpm = currentRotation.y.Remap(-m_StartEndRotation, m_StartEndRotation, 0f, _rpmControlScript.m_MaxMotorRPM);
            outputText.text = rpm.ToString("F0");
            _rpmControlScript.SetRPM(rpm);
        //}

    }

}
