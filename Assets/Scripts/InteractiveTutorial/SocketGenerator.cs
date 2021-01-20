using UnityEngine;
using UnityEditor.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketGenerator : MonoBehaviour
{
    public Transform m_TargetObject;
    public LayerMask m_InteractableLayerMask = -1;

    public void Generate()
    {
        if (m_TargetObject == null)
        {
            Debug.LogError("No socket target object specified!");
            return;
        }

        GenerateInteractables();
        GenerateSockets();
    }

    private void GenerateSockets()
    {
        GameObject socketRoot = new GameObject("Sockets");
        socketRoot.transform.parent = m_TargetObject;
        socketRoot.transform.localPosition = Vector3.zero;
        socketRoot.transform.localRotation = Quaternion.identity;

        foreach(Transform t in m_TargetObject)
        {
            if(t == socketRoot.transform)
            {
                continue;
            }

            GameObject colObj = new GameObject("Socket_" + t.gameObject.name);
            colObj.transform.parent = socketRoot.transform;
            colObj.transform.SetParent(socketRoot.transform, false);
            colObj.transform.rotation = t.rotation;
            colObj.transform.position = t.position;
            
            
            //set layer and tag here if needed 

            BoxCollider boxCollider = colObj.AddComponent<BoxCollider>();
            MeshRenderer meshRenderer = t.gameObject.GetComponent<MeshRenderer>();

            boxCollider.center = meshRenderer.bounds.center - colObj.transform.position;
            boxCollider.size = meshRenderer.bounds.size;
            boxCollider.isTrigger = true;

            XRSocketInteractor socketInteractor = colObj.AddComponent<XRSocketInteractor>();
            Socket socketFunc = colObj.AddComponent<Socket>();
            socketFunc.m_ExpectedObject = t.gameObject;

            UnityEventTools.AddPersistentListener(socketInteractor.onHoverEntered, socketFunc.CheckInteractableValidity);
            UnityEventTools.AddPersistentListener(socketInteractor.onSelectEntered, socketFunc.SnapInteractableToSocketPosition);

            socketInteractor.interactionLayerMask = m_InteractableLayerMask;
            //socketInteractor.interactionLayerMask = LayerMask.NameToLayer("Interactable");
        }
    }

    private void GenerateInteractables()
    {
        
        string layer = LayerMask.LayerToName((int)Mathf.Log(m_InteractableLayerMask.value, 2));   // https://answers.unity.com/questions/472886/use-layer-name-for-layermask-instead-of-layer-numb.html  Answer by Vahradrim

        foreach (Transform t in m_TargetObject)
        {

            t.gameObject.layer = LayerMask.NameToLayer(layer);

            MeshCollider meshCollider = t.gameObject.AddComponent<MeshCollider>();
            meshCollider.convex = true;
            //BoxCollider boxCollider = t.gameObject.AddComponent<BoxCollider>();
            //MeshRenderer meshRenderer = t.gameObject.GetComponent<MeshRenderer>();

            //boxCollider.center = meshRenderer.bounds.center - t.transform.position;
            //boxCollider.size = meshRenderer.bounds.size;

            Rigidbody rb = t.gameObject.AddComponent<Rigidbody>();
            XRGrabInteractable interactable = t.gameObject.AddComponent<XRGrabInteractable>();
            /*Outline outline = */t.gameObject.AddComponent<Outline>();
            /*OutlineController outlineController = */t.gameObject.AddComponent<OutlineController>();
            /*DynamicGrabPoint dynamicGrabPoint = */t.gameObject.AddComponent<DynamicGrabPoint>();

            rb.isKinematic = true;

            interactable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        }
    }
}
