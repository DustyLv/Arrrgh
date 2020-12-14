using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WebXR;

namespace DaviisLV.WebXR
{
    public class SetUIType : MonoBehaviour
    {
        public GameObject VR_Module;
        public GameObject Standart_Module;
        void Start()
        {
            WebXRManager.Instance.OnXRChange += onXRChange;
#if UNITY_EDITOR
            VR_Module.SetActive(true);
            Standart_Module.SetActive(false);
#elif UNITY_WEBGL
            VR_Module.SetActive(false);
            Standart_Module.SetActive(true);
#endif
        }

        private void onXRChange(WebXRState state)
        {
            if (state == WebXRState.ENABLED)
            {
                VR_Module.SetActive(false);
                Standart_Module.SetActive(true);
            }
            else
            {
                VR_Module.SetActive(true);
                Standart_Module.SetActive(false);
            }
        }
    }
}
