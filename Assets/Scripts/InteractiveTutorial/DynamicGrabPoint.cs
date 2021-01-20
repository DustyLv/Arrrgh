using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class DynamicGrabPoint : MonoBehaviour
{
    private Vector3 m_InteractorPosition = Vector3.zero;
    private Quaternion m_InteractorRotation = Quaternion.identity;

    private XRGrabInteractable m_XRGrabInteractable;

    // Start is called before the first frame update
    void Start()
    {
        m_XRGrabInteractable = GetComponent<XRGrabInteractable>();

        // This listener assignment is needed, because XRTK uses their own kind of UnityEvent system, and the events can not be assigned in Edit mode through AddPersistentListener.
        m_XRGrabInteractable.onSelectEntered.AddListener(OnSelectEnter);
        m_XRGrabInteractable.onSelectExited.AddListener(OnSelectExit);
    }



    public void OnSelectEnter(XRBaseInteractor interactor)
    {
        StoreInteractor(interactor);
        MatchAttachmentPoints(interactor);
    }

    public void OnSelectExit(XRBaseInteractor interactor)
    {
        ResetAttachmentPoints(interactor);
        ClearInteractor(interactor);
    }



    private void StoreInteractor(XRBaseInteractor interactor)
    {
        // Store Controller attachment point transform
        m_InteractorPosition = interactor.attachTransform.localPosition;
        m_InteractorRotation = interactor.attachTransform.localRotation;
    }

    // Sets the controllers attach point
    private void MatchAttachmentPoints(XRBaseInteractor interactor)
    {
        // Check if this Interactable has an attach point
        // If it does, then use that
        // But if it doesn't, then use this Interactables' transform
        bool hasAttach = m_XRGrabInteractable.attachTransform != null;
        interactor.attachTransform.position = hasAttach ? m_XRGrabInteractable.attachTransform.position : transform.position;
        interactor.attachTransform.rotation = hasAttach ? m_XRGrabInteractable.attachTransform.rotation : transform.rotation;
    }

    private void ResetAttachmentPoints(XRBaseInteractor interactor)
    {
        interactor.attachTransform.localPosition = m_InteractorPosition;
        interactor.attachTransform.localRotation = m_InteractorRotation;
    }

    private void ClearInteractor(XRBaseInteractor interactor)
    {
        m_InteractorPosition = Vector3.zero;
        m_InteractorRotation = Quaternion.identity;
    }
}
