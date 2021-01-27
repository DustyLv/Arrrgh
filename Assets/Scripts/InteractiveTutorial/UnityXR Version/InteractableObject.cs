using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRVersion
{
    public class InteractableObject : MonoBehaviour
    {
        public Transform m_ThisTransform;
        public Transform m_Socket;

        public bool m_IsActiveObject = false;


        public void SetIndicationArrow_This()
        {
            IndicationArrow.i.Move(m_ThisTransform);
        }

        public void SetIndicationArrow_This(XRBaseInteractor target)
        {
            IndicationArrow.i.Move(m_ThisTransform);
        }

        public void SetIndicationArrow_Socket(XRBaseInteractor target)
        {
            if (m_IsActiveObject)
            {
                IndicationArrow.i.Move(m_Socket);
            }
        }

        public void SetAsActive()
        {
            m_IsActiveObject = true;
            SetIndicationArrow_This();
        }

    }
}

