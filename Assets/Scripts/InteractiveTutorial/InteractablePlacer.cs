using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractablePlacer : MonoBehaviour
{
    public InteractableObject m_InteractableObject;
    public XRBaseInteractable m_XRInteractable;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetInteractableFromPickup(XRBaseInteractable interactable)
    {
        //print($"current picked up interactable is: {interactable.gameObject.name}");
        m_InteractableObject = interactable.gameObject.GetComponent<InteractableObject>();
        m_XRInteractable = interactable;
    }


    public void PlaceInteractableInSocket()
    {
        if(m_InteractableObject != null)
        {
            m_InteractableObject.m_IntersectingSocket.PlaceInSocket(m_InteractableObject);
            m_InteractableObject = null;
            
        }
    }

    
}
