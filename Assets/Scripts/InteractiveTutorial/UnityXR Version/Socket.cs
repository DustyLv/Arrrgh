using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace XRVersion
{
    //[RequireComponent(typeof(XRSocketInteractor))]
    public class Socket : XRSocketInteractor
    {
        public GameObject m_ExpectedObject;
        //[HideInInspector] public XRSocketInteractor m_SocketInteractor;

        // Start is called before the first frame update
        void Start()
        {
            //m_SocketInteractor = GetComponent<XRSocketInteractor>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void CheckInteractableValidity(XRBaseInteractable interactable)
        {
            if (/*CheckInteractableLayer(interactable) && */CheckInteractableObject(interactable))
            {
                //m_SocketInteractor.socketActive = true;
                interactableHoverMeshMaterial.color = SocketGlobalSettings.i.m_ColorValid;
            }
            else
            {
                //m_SocketInteractor.socketActive = false;
                //m_SocketInteractor.interactableHoverMeshMaterial.color = SocketGlobalSettings.i.m_ColorInvalid;
            }
        }

        public void SnapInteractableToSocketPosition(XRBaseInteractable interactable)
        {
            interactable.transform.position = this.transform.position;
            interactable.transform.rotation = this.transform.rotation;
        }

        //public void DisableSocket(XRBaseInteractable interactable)
        //{
        //    m_SocketInteractor.socketActive = false;
        //}

        public void DisableInteractableInSocket(XRBaseInteractable interactable)
        {
            interactable.enabled = false;
        }

        //public void ShowSocketGraphics()
        //{
        //    m_SocketInteractor.showInteractableHoverMeshes
        //}

        private bool CheckInteractableLayer(XRBaseInteractable interactable)
        {
            return interactable.gameObject.layer == LayerMask.NameToLayer("Interactable");
        }

        private bool CheckInteractableObject(XRBaseInteractable interactable)
        {
            //print($"incoming obj: {interactable.gameObject.name} |||||  expected obj: {m_ExpectedObject.name}");
            return interactable.gameObject == m_ExpectedObject;
        }
    }
}
