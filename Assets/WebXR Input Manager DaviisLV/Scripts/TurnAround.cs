using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaviisLV.WebXR
{
    public class TurnAround : MonoBehaviour
    {
        [SerializeField]
        private Transform VRPlayerRig;
        [SerializeField]
        private Transform VRCamera;
        [SerializeField]
        private int TurnAngle = 45;
        private bool isRotating = false;

        public void RotateLeft()
        {
            if (!isRotating)
                Rotate(-1);
        }
        public void RotateRight()
        {
            if (!isRotating)
                Rotate(1);
        }
        private void Rotate(float direction)
        {
            isRotating = true;
            VRPlayerRig.RotateAround(VRCamera.position, Vector3.up, TurnAngle * direction);
            isRotating = false;
        }
    }
}
