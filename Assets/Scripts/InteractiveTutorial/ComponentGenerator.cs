using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Animations;

public class ComponentGenerator : MonoBehaviour
{
    public Transform m_TargetObject;
    public LayerMask m_InteractableLayerMask = -1;
    public LayerMask m_SocketLayerMask = -1;
    public Material m_SocketMaterial;

    public string m_SocketRootObject_Name = "Sockets";
    public string m_SocketObject_Prefix = "Socket_";

    //public bool m_InteractableHasGravityOnRelease = false;
    public bool m_InteractableRigidbodyIsKinematic = false;

    public InteractiveTutorialController m_InteractiveTutorialController;

    public List<SocketObject> m_SocketObjects = new List<SocketObject>();
    public List<InteractableObject> m_InteractableObjects = new List<InteractableObject>();


    [Button]
    public void Generate()
    {
        if (m_TargetObject == null)
        {
            Debug.LogError("No socket target object specified!");
            return;
        }

        m_SocketObjects.Clear();
        m_InteractableObjects.Clear();

        
        GenerateInteractables();
        GenerateSockets();
        SetExpectedObjects();
    }

    private void GenerateSockets()
    {
        if (m_TargetObject.Find(m_SocketRootObject_Name) != null)
        {
            GameObject.DestroyImmediate(m_TargetObject.Find(m_SocketRootObject_Name).gameObject);
        }

        GameObject socketRoot = new GameObject(m_SocketRootObject_Name);
        socketRoot.transform.parent = m_TargetObject;
        socketRoot.transform.localPosition = Vector3.zero;
        socketRoot.transform.localRotation = Quaternion.identity;
        socketRoot.transform.localScale = Vector3.one;

        string socketLayer = LayerMask.LayerToName((int)Mathf.Log(m_SocketLayerMask.value, 2));   // https://answers.unity.com/questions/472886/use-layer-name-for-layermask-instead-of-layer-numb.html  Answer by Vahradrim

        //m_SocketMaterial.SetColor("_BaseColor", SocketGlobalSettings.i.m_ColorIdle);

        foreach (Transform t in m_TargetObject)
        {
            if (t == socketRoot.transform)
            {
                continue;
            }

            GameObject socketObj = new GameObject(m_SocketObject_Prefix + t.gameObject.name);
            socketObj.transform.parent = socketRoot.transform;
            socketObj.transform.SetParent(socketRoot.transform, false);

            socketObj.transform.rotation = t.rotation;
            socketObj.transform.position = t.position;
            socketObj.transform.localScale = t.localScale;

            socketObj.layer = LayerMask.NameToLayer(socketLayer);


            //set layer and tag here if needed 

            BoxCollider boxCollider = socketObj.AddComponent<BoxCollider>();
            MeshRenderer meshRendererObject = t.gameObject.GetComponent<MeshRenderer>();

          
            MeshRenderer meshRendererSocket = socketObj.AddComponent<MeshRenderer>();
            meshRendererSocket.material = m_SocketMaterial;
            meshRendererSocket.receiveShadows = false;
            meshRendererSocket.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            MeshFilter meshFilterSocket = socketObj.AddComponent<MeshFilter>();
            meshFilterSocket.sharedMesh = t.gameObject.GetComponent<MeshFilter>().sharedMesh;
            

            boxCollider.center = meshRendererObject.bounds.center - socketObj.transform.position;
            boxCollider.size = meshRendererObject.bounds.size;
            boxCollider.isTrigger = true;

            

            //XRSocketInteractor socketInteractor = colObj.AddComponent<XRSocketInteractor>();
            //socketInteractor.showInteractableHoverMeshes = true; // Enable/Disable built-in hover mesh
            //socketInteractor.socketActive = false;
            SocketObject socketObjScript = socketObj.AddComponent<SocketObject>();
            //socketObjScript.m_ExpectedObject = GetInteractableByName(t.gameObject.name);

            //UnityEventTools.AddPersistentListener(socketInteractor.onHoverEntered, socketFunc.CheckInteractableValidity);
            //UnityEventTools.AddPersistentListener(socketInteractor.onSelectEntered, socketFunc.SnapInteractableToSocketPosition);
            ////UnityEventTools.AddPersistentListener(socketInteractor.onSelectEntered, socketFunc.DisableSocket);
            //UnityEventTools.AddPersistentListener(socketInteractor.onSelectEntered, m_InteractiveTutorialController.ActivateNextSocket);

            //socketInteractor.interactionLayerMask = m_InteractableLayerMask;
            //socketInteractor.interactionLayerMask = LayerMask.NameToLayer("Interactable");

            m_SocketObjects.Add(socketObjScript);
        }
    }

    private void GenerateInteractables()
    {

        string interactableLayer = LayerMask.LayerToName((int)Mathf.Log(m_InteractableLayerMask.value, 2));   // https://answers.unity.com/questions/472886/use-layer-name-for-layermask-instead-of-layer-numb.html  Answer by Vahradrim

        foreach (Transform t in m_TargetObject)
        {
            if (t.gameObject.name.Contains(m_SocketRootObject_Name))
            {
                continue;
            }

            

            t.gameObject.layer = LayerMask.NameToLayer(interactableLayer);

            Rigidbody rb = (t.gameObject.GetComponent<Rigidbody>() != null) ? t.gameObject.GetComponent<Rigidbody>() : t.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = m_InteractableRigidbodyIsKinematic;

            //MeshCollider meshCollider = (t.gameObject.GetComponent<MeshCollider>() != null) ? t.gameObject.GetComponent<MeshCollider>() : t.gameObject.AddComponent<MeshCollider>();
            //meshCollider.convex = true;

            BoxCollider boxCollider = (t.gameObject.GetComponent<BoxCollider>() != null) ? t.gameObject.GetComponent<BoxCollider>() : t.gameObject.AddComponent<BoxCollider>();
            MeshRenderer meshRenderer = t.gameObject.GetComponent<MeshRenderer>();

            boxCollider.center = meshRenderer.bounds.center - t.transform.position;
            boxCollider.size = meshRenderer.bounds.size * (1f/m_TargetObject.localScale.x);


            //XRGrabInteractable interactable = (t.gameObject.GetComponent<XRGrabInteractable>() != null) ? t.gameObject.GetComponent<XRGrabInteractable>() : t.gameObject.AddComponent<XRGrabInteractable>();
            InteractableObject interactableObj = (t.gameObject.GetComponent<InteractableObject>() != null) ? t.gameObject.GetComponent<InteractableObject>() : t.gameObject.AddComponent<InteractableObject>();
            //intObj.m_ThisTransform = t;
            //intObj.m_Socket = GetSocketTransform(t.gameObject.name);

            if (t.gameObject.GetComponent<Outline>() == null) t.gameObject.AddComponent<Outline>();
            if (t.gameObject.GetComponent<OutlineController>() == null) t.gameObject.AddComponent<OutlineController>();
            if (t.gameObject.GetComponent<DynamicGrabPoint>() == null) t.gameObject.AddComponent<DynamicGrabPoint>(); // XRGrab interactable component will get added along with this

            //ParentConstraint parentConstraint = (t.gameObject.GetComponent<ParentConstraint>() != null) ? t.gameObject.GetComponent<ParentConstraint>() : t.gameObject.AddComponent<ParentConstraint>();
            //ConstraintSource conSource = new ConstraintSource();
            //conSource.sourceTransform = GetSocketTransform(t.gameObject.name);
            //conSource.weight = 1f;
            //parentConstraint.SetSource(0, conSource);

            m_InteractableObjects.Add(interactableObj);

            //UnityEventTools.AddPersistentListener(interactable.onSelectEntered, intObj.SetIndicationArrow_Socket);
            //UnityEventTools.AddPersistentListener(interactable.onSelectExited, intObj.SetIndicationArrow_This);
        }
    }

    private void SetExpectedObjects()
    {
        foreach(InteractableObject intObj in m_InteractableObjects)
        {
            intObj.m_ExpectedSocket = GetSocketObject(intObj.gameObject.name);
        }

        foreach(SocketObject sObj in m_SocketObjects)
        {
            sObj.m_ExpectedObject = GetInteractableObject(sObj.gameObject.name);
        }
    }

    private Transform GetSocketTransform(string objectName)
    {
        foreach (SocketObject t in m_SocketObjects)
        {
            if (t.gameObject.name == m_SocketObject_Prefix + objectName)
            {
                return t.gameObject.transform;
            }
        }
        return null;
    }
    private SocketObject GetSocketObject(string objectName)
    {
        foreach (SocketObject sObj in m_SocketObjects)
        {
            if (sObj.gameObject.name == m_SocketObject_Prefix + objectName)
            {
                return sObj;
            }
        }
        return null;
    }

    private InteractableObject GetInteractableObject(string objectName)
    {
        foreach (InteractableObject iObj in m_InteractableObjects)
        {
            if (iObj.gameObject.name == objectName.Replace(m_SocketObject_Prefix, ""))
            {
                return iObj;
            }
        }
        Debug.LogError("Could not find InteractableObject with name: " + objectName);
        return null;
    }
}

