using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WebXR;

namespace DaviisLV.WebXR
{
    public class VRUIInputModule : BaseInputModule
    {
        private Camera pointer;
        private GameObject currentObj = null;
        private PointerEventData data = null;
        private bool isVR = true;
        protected override void Awake()
        {
            base.Awake();
            data = new PointerEventData(eventSystem);
            pointer = FindObjectOfType<Pointer>().GetComponent<Camera>();
        }
        void Start()
        {
            WebXRManager.Instance.OnXRChange += onXRChange;
        }

        private void onXRChange(WebXRState state)
        {
#if UNITY_EDITOR
            isVR = true;
#elif UNITY_WEBGL
            if (state == WebXRState.ENABLED)
            {
                isVR = true;
            }
            else
            {
                isVR = false;
            }
#endif
        }
        public void UIPress()
        {
            ProcessPress(data);
        }
        public void UIRelease()
        {
            ProcessRelease(data);
        }
        public override void Process()
        {
            if (isVR)
            {
                data.Reset();
                data.position = new Vector2(pointer.pixelWidth / 2, pointer.pixelHeight / 2);
                eventSystem.RaycastAll(data, m_RaycastResultCache);
                data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
                currentObj = data.pointerCurrentRaycast.gameObject;
                m_RaycastResultCache.Clear();
                HandlePointerExitAndEnter(data, currentObj);
            }
        }
        public PointerEventData GetData()
        {
            return data;
        }
        private void ProcessPress(PointerEventData data)
        {
            data.pointerCurrentRaycast = data.pointerCurrentRaycast;
            GameObject newPointerPress = ExecuteEvents.ExecuteHierarchy(currentObj, data, ExecuteEvents.pointerDownHandler);

            if (newPointerPress == null)
                newPointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentObj);

            data.pressPosition = data.position;
            data.pointerPress = newPointerPress;
            data.rawPointerPress = currentObj;
        }
        private void ProcessRelease(PointerEventData data)
        {
            ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerUpHandler);
            GameObject PointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentObj);

            if (data.pointerPress == PointerUpHandler)
                ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerClickHandler);

            eventSystem.SetSelectedGameObject(null);
            data.pressPosition = Vector2.zero;
            data.pointerPress = null;
            data.rawPointerPress = null;
        }
    }
}