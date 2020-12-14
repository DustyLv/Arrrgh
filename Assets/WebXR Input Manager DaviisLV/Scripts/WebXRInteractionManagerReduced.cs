using UnityEngine;
using WebXR;
using UnityEngine.Events;
using NaughtyAttributes;
using UnityEngine.XR;
using System;

namespace DaviisLV.WebXR
{
    [RequireComponent(typeof(WebXRController))]

    public class WebXRInteractionManagerReduced : MonoBehaviour
    {
        private WebXRController controller;
        private char leftController = 'L';
        private char rightController = 'R';
        private char controllerSide;
        private bool UseWrongInput = false;

        #region UnityEvents
        [Foldout("Trigger")]
        public UnityEvent OnTriggerDown = new UnityEvent();
        [Foldout("Trigger")]
        public UnityEvent OnTriggerUp = new UnityEvent();

        [Foldout("Primary button")]
        public UnityEvent OnBtn1Down = new UnityEvent();
        [Foldout("Primary button")]
        public UnityEvent OnBtn1Up = new UnityEvent();

        [Foldout("Grip")]
        public UnityEvent OnGripDown = new UnityEvent();
        [Foldout("Grip")]
        public UnityEvent OnGripUp = new UnityEvent();

        [Foldout("Touchpad/Joystick")]
        public UnityEvent OnTouchpadDown = new UnityEvent();
        [Foldout("Touchpad/Joystick")]
        public UnityEvent OnTouchpadUp = new UnityEvent();

        [Foldout("Menu")]
        public UnityEvent OnMenuDown = new UnityEvent();
        #endregion

        void Awake()
        {
            //Debug.Log(InputDevice.name);

            controller = GetComponent<WebXRController>();
            if (controller.hand == WebXRControllerHand.RIGHT)
            {
#if UNITY_EDITOR
                UseWrongInput = false;
#elif UNITY_WEBGL
                UseWrongInput = true;
#endif
              //  UseWrongInput = false;
                controllerSide = rightController;
            }
            else
                controllerSide = leftController;
        }

        void Update()
        {
            #region Trigger btn
            if (controller.GetButtonUp("Trigger_btn_" + controllerSide))
                OnTriggerUp.Invoke();

            if (controller.GetButtonDown("Trigger_btn_" + controllerSide))
                OnTriggerDown.Invoke();

            #endregion
            #region Grip btn
            if (controller.GetButtonDown("Grip_btn_" + controllerSide))
                OnGripDown.Invoke();

            if (controller.GetButtonUp("Grip_btn_" + controllerSide))
                OnGripUp.Invoke();

            #endregion
            #region Touchpad/Joystick btn
            if (controller.GetButtonDown("Joystic_btn_" + controllerSide))
                if (UseWrongInput)
                    OnBtn1Down.Invoke();
                else
                    OnTouchpadDown.Invoke();

            if (controller.GetButtonUp("Joystic_btn_" + controllerSide))
                if (UseWrongInput)
                    OnBtn1Up.Invoke();
                else
                    OnTouchpadUp.Invoke();

            #endregion
            #region Primary btn
            if (controller.GetButtonDown("1_btn_" + controllerSide))
                if (UseWrongInput)
                    OnTouchpadDown.Invoke();
                else
                    OnBtn1Down.Invoke();

            if (controller.GetButtonUp("1_btn_" + controllerSide))
                if (UseWrongInput)
                    OnTouchpadUp.Invoke();
                else
                    OnBtn1Up.Invoke();

            if (controller.GetButtonDown("2_btn_" + controllerSide))
                if (UseWrongInput)
                    OnBtn1Down.Invoke();

            if (controller.GetButtonUp("2_btn_" + controllerSide))
                if (UseWrongInput)
                    OnBtn1Up.Invoke();
            #endregion
            #region Menu 
            if (controller.GetButtonDown("Menu_btn_" + controllerSide))
                OnMenuDown.Invoke();
            #endregion
        }
    }
}