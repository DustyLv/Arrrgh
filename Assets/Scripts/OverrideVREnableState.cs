using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class OverrideVREnableState : MonoBehaviour
{
    public bool m_State = true;

    //XRDisplaySubsystem xRDisplaySubsystem;
    void Start()
    {
        XRSettings.enabled = m_State;
    }


}
