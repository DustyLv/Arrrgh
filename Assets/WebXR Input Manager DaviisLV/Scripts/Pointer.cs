using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DaviisLV.WebXR
{
    public class Pointer : MonoBehaviour
    {
        private GameObject dot;
        private VRUIInputModule vrInputModule;
        private LineRenderer lineRenderer = null;
        private float defaultLenght = 0f;


        private void Awake()
        {
            dot = gameObject.transform.GetChild(0).gameObject;
            lineRenderer = GetComponent<LineRenderer>();
            vrInputModule = FindObjectOfType<VRUIInputModule>();
        }
        private void Update()
        {
            updateLine();
        }
        private void updateLine()
        {
            PointerEventData data = vrInputModule.GetData();
            float targetLength = data.pointerCurrentRaycast.distance == 0 ? defaultLenght : data.pointerCurrentRaycast.distance;
            RaycastHit hit = CreateRaycast(targetLength);

            Vector3 endPos = transform.position + (transform.forward * targetLength);

            if (hit.collider != null)
                endPos = hit.point;

            dot.transform.position = endPos;

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, endPos);

        }
        private RaycastHit CreateRaycast(float length)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.forward);
            Physics.Raycast(ray, out hit, defaultLenght);
            return hit;
        }
    }
}
