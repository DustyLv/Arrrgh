using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerInteractionController : XRDirectInteractor
{

    public InteractableObject m_Interactable;

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        
    }

    public void GetInteractableObject()
    {
        //print(validTargets[0]);
        XRBaseInteractable tempInteractable = validTargets[0];
        m_Interactable = tempInteractable.gameObject.GetComponent<InteractableObject>();

        tempInteractable.onSelectExited.AddListener(m_Interactable.PutIntoSocket);
    }

    //void OnTriggerEnter(Collider col)
    //{

    //}

    void test()
    {

    }
}
