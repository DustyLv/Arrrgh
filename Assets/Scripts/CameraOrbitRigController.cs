using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbitRigController : MonoBehaviour
{
    public float m_CameraMoveSpeed = 100f;
    public float m_CameraZoomSpeed = 2f;

    private Camera m_Camera;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        m_Camera.fieldOfView -= Input.mouseScrollDelta.y * m_CameraZoomSpeed;

        if (Input.GetMouseButton(2))
        {
            float horizontal = Input.GetAxis("Mouse X");
            transform.Rotate(transform.up, horizontal * m_CameraMoveSpeed * Time.deltaTime);
        }
    }
}
