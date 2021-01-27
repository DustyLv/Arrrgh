using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRVersion
{
    public class SocketGenerator : MonoBehaviour
    {
        public Transform m_TargetObject;
        public LayerMask m_InteractableLayerMask = -1;
        public Material m_SocketMaterial;

        public string m_SocketRootObject_Name = "Sockets";
        public string m_SocketObject_Prefix = "Socket_";

        public bool m_InteractableHasGravityOnRelease = false;
        public bool m_InteractableRigidbodyIsKinematic = false;

        public InteractiveTutorialController m_InteractiveTutorialController;

        private List<Transform> m_SocketTransforms = new List<Transform>();

        public void Generate()
        {
            if (m_TargetObject == null)
            {
                Debug.LogError("No socket target object specified!");
                return;
            }

            m_SocketTransforms.Clear();

            GenerateSockets();
            GenerateInteractables();
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

            //m_SocketMaterial.SetColor("_BaseColor", SocketGlobalSettings.i.m_ColorIdle);

            foreach (Transform t in m_TargetObject)
            {
                if (t == socketRoot.transform)
                {
                    continue;
                }

                GameObject colObj = new GameObject(m_SocketObject_Prefix + t.gameObject.name);
                colObj.transform.parent = socketRoot.transform;
                colObj.transform.SetParent(socketRoot.transform, false);
                colObj.transform.rotation = t.rotation;
                colObj.transform.position = t.position;


                //set layer and tag here if needed 

                BoxCollider boxCollider = colObj.AddComponent<BoxCollider>();
                MeshRenderer meshRendererObject = t.gameObject.GetComponent<MeshRenderer>();

                if (false)
                {
                    MeshRenderer meshRendererSocket = colObj.AddComponent<MeshRenderer>();
                    meshRendererSocket.material = m_SocketMaterial;
                    meshRendererSocket.receiveShadows = false;
                    meshRendererSocket.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                    MeshFilter meshFilterSocket = colObj.AddComponent<MeshFilter>();
                    meshFilterSocket.sharedMesh = t.gameObject.GetComponent<MeshFilter>().sharedMesh;
                }

                boxCollider.center = meshRendererObject.bounds.center - colObj.transform.position;
                boxCollider.size = meshRendererObject.bounds.size;
                boxCollider.isTrigger = true;



                //XRSocketInteractor socketInteractor = colObj.AddComponent<XRSocketInteractor>();
                //socketInteractor.showInteractableHoverMeshes = true; // Enable/Disable built-in hover mesh
                //socketInteractor.socketActive = false;
                Socket socket = colObj.AddComponent<Socket>();
                socket.showInteractableHoverMeshes = true;
                socket.m_ExpectedObject = t.gameObject;

                UnityEventTools.AddPersistentListener(socket.onHoverEntered, socket.CheckInteractableValidity);
                UnityEventTools.AddPersistentListener(socket.onSelectEntered, socket.SnapInteractableToSocketPosition);
                //UnityEventTools.AddPersistentListener(socketInteractor.onSelectEntered, socketFunc.DisableSocket);
                UnityEventTools.AddPersistentListener(socket.onSelectEntered, m_InteractiveTutorialController.ActivateNextSocket);

                socket.interactionLayerMask = m_InteractableLayerMask;
                //socketInteractor.interactionLayerMask = LayerMask.NameToLayer("Interactable");

                m_SocketTransforms.Add(colObj.transform);
            }
        }

        private void GenerateInteractables()
        {

            string layer = LayerMask.LayerToName((int)Mathf.Log(m_InteractableLayerMask.value, 2));   // https://answers.unity.com/questions/472886/use-layer-name-for-layermask-instead-of-layer-numb.html  Answer by Vahradrim

            foreach (Transform t in m_TargetObject)
            {
                if (t.gameObject.name.Contains(m_SocketRootObject_Name))
                {
                    continue;
                }


                t.gameObject.layer = LayerMask.NameToLayer(layer);

                Rigidbody rb = (t.gameObject.GetComponent<Rigidbody>() != null) ? t.gameObject.GetComponent<Rigidbody>() : t.gameObject.AddComponent<Rigidbody>();
                rb.isKinematic = m_InteractableRigidbodyIsKinematic;

                MeshCollider meshCollider = (t.gameObject.GetComponent<MeshCollider>() != null) ? t.gameObject.GetComponent<MeshCollider>() : t.gameObject.AddComponent<MeshCollider>();
                meshCollider.convex = true;
                //BoxCollider boxCollider = t.gameObject.AddComponent<BoxCollider>();
                //MeshRenderer meshRenderer = t.gameObject.GetComponent<MeshRenderer>();

                //boxCollider.center = meshRenderer.bounds.center - t.transform.position;
                //boxCollider.size = meshRenderer.bounds.size;


                XRGrabInteractable interactable = (t.gameObject.GetComponent<XRGrabInteractable>() != null) ? t.gameObject.GetComponent<XRGrabInteractable>() : t.gameObject.AddComponent<XRGrabInteractable>();
                InteractableObject intObj = (t.gameObject.GetComponent<InteractableObject>() != null) ? t.gameObject.GetComponent<InteractableObject>() : t.gameObject.AddComponent<InteractableObject>();
                intObj.m_ThisTransform = t;
                intObj.m_Socket = GetSocketTransform(t.gameObject.name);

                if (t.gameObject.GetComponent<Outline>() == null) t.gameObject.AddComponent<Outline>();
                if (t.gameObject.GetComponent<OutlineController>() == null) t.gameObject.AddComponent<OutlineController>();
                if (t.gameObject.GetComponent<DynamicGrabPoint>() == null) t.gameObject.AddComponent<DynamicGrabPoint>();

                interactable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
                interactable.gravityOnDetach = m_InteractableHasGravityOnRelease;

                //UnityEventTools.AddPersistentListener(interactable.onSelectEntered, intObj.SetIndicationArrow_Socket);
                //UnityEventTools.AddPersistentListener(interactable.onSelectExited, intObj.SetIndicationArrow_This);
            }
        }

        private Transform GetSocketTransform(string objectName)
        {
            foreach (Transform t in m_SocketTransforms)
            {
                if (t.gameObject.name == m_SocketObject_Prefix + objectName)
                {
                    return t;
                }
            }
            return null;
        }
    }
}
