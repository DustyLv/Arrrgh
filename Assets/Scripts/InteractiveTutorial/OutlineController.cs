using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class OutlineController : MonoBehaviour
{
    private XRGrabInteractable m_XRGrabInteractable;
    private Outline m_Outline;

    void Start()
    {
        m_XRGrabInteractable = gameObject.GetComponent<XRGrabInteractable>();
        m_Outline = gameObject.GetComponent<Outline>();

        // This listener assignment is needed, because XRTK uses their own kind of UnityEvent system, and the events can not be assigned in Edit mode through AddPersistentListener.
        m_XRGrabInteractable.onHoverEntered.AddListener(DoEnable);
        m_XRGrabInteractable.onHoverExited.AddListener(DoDisable);
        m_Outline.enabled = false;
    }

    public void DoEnable(XRBaseInteractor interactor)
    {
        m_Outline.enabled = true;
    }

    public void DoDisable(XRBaseInteractor interactor)
    {
        m_Outline.enabled = false;
    }


}
