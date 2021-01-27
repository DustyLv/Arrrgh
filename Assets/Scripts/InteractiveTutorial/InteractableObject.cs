using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.XR.Interaction.Toolkit;
using DG.Tweening;

public class InteractableObject : MonoBehaviour
{


    public SocketObject m_IntersectingSocket;

    public SocketObject m_ExpectedSocket;

    private Rigidbody m_Rigidbody;
    private Collider m_Collider;
    //private ParentConstraint m_ParentConstraint;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
        m_Collider = gameObject.GetComponent<Collider>();
        //m_ParentConstraint = gameObject.GetComponent<ParentConstraint>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PutIntoSocket(XRBaseInteractor interactor)
    {
        if (m_IntersectingSocket != null && m_IntersectingSocket == m_ExpectedSocket)
        {
            //m_IntersectingSocket.PlaceInSocket(this);

            transform.DOMove(m_ExpectedSocket.transform.position, 0.5f);
            transform.DORotateQuaternion(m_ExpectedSocket.transform.rotation, 0.5f).OnComplete(() =>
            {
                print("Completed");
                OnFinishPlaceInSocket();
                InteractiveTutorialController.i.ActivateNextSocket();
                //m_ExpectedSocket.OnPlacedInSocket();

                
            });
        }
    }

    public void OnFinishPlaceInSocket()
    {
        m_Rigidbody.isKinematic = true;
        m_Collider.enabled = false;
        //m_ParentConstraint.constraintActive = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if(other.gameObject != m_ExpectedSocket.gameObject || other.gameObject.tag != "PlayerHand")
        //{
        //    Physics.IgnoreCollision(other, gameObject.GetComponent<Collider>());
        //}

        SocketObject incomingTest = other.gameObject.GetComponent<SocketObject>();
        if (incomingTest != null)
        {
            if (incomingTest == m_ExpectedSocket)
            {
                m_IntersectingSocket = incomingTest;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(m_IntersectingSocket != null)
        {
            m_IntersectingSocket = null;
        }
    }

}
