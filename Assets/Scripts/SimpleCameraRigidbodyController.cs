using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleCameraRigidbodyController : MonoBehaviour
{


    [Header("Movement Settings")]
    [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
    public float boost = 6f;

    [Header("Rotation Settings")]
    [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
    public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

    [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
    public bool invertY = false;

    private Rigidbody rb;

    private float yaw = 0f;
    private float pitch = 0f;
    private float roll = 0f;

    private Vector3 translation = Vector3.zero;

    private Keyboard kb;
    private Mouse mouse;

    void Start()
    {
        kb = Keyboard.current;
        mouse = Mouse.current;
        rb = GetComponent<Rigidbody>();

        yaw = transform.rotation.eulerAngles.y;
        pitch = transform.rotation.eulerAngles.x;
        roll = transform.rotation.eulerAngles.z;
    }

    Vector3 GetInputTranslationDirection()
    {
        if(kb == null)
        {
            return Vector3.zero;
        }
        Vector3 direction = new Vector3();


        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            direction += Vector3.down;
        }
        if (Input.GetKey(KeyCode.E))
        {
            direction += Vector3.up;
        }

        //if (kb.wKey.isPressed)
        //{
        //    direction += Vector3.forward;
        //}
        //if (kb.sKey.isPressed)
        //{
        //    direction += Vector3.back;
        //}
        //if (kb.aKey.isPressed)
        //{
        //    direction += Vector3.left;
        //}
        //if (kb.dKey.isPressed)
        //{
        //    direction += Vector3.right;
        //}
        //if (kb.qKey.isPressed)
        //{
        //    direction += Vector3.down;
        //}
        //if (kb.eKey.isPressed)
        //{
        //    direction += Vector3.up;
        //}
        return direction;
    }

    void Update()
    {
        translation = Vector3.zero;
        
        // Exit Sample  
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        // Hide and lock cursor when right mouse button pressed
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        

        // Unlock and show cursor when right mouse button released
        if (Input.GetMouseButtonUp(1))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // Rotation
        if (Input.GetMouseButton(1))
        {
            var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertY ? 1 : -1));
            //var mouseMovement = mouse.


            var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

            yaw += mouseMovement.x * mouseSensitivityFactor;
            pitch += mouseMovement.y * mouseSensitivityFactor;
        }

        // Translation
        translation = GetInputTranslationDirection() * Time.deltaTime;

        // Speed up movement when shift key held
        if (Input.GetKey(KeyCode.LeftShift))
        {
            translation *= 10.0f;
        }

        // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
        boost += Input.mouseScrollDelta.y * 0.2f;
        translation *= Mathf.Pow(2.0f, boost);

        transform.eulerAngles = new Vector3(pitch, yaw, roll);
    }

    private void FixedUpdate()
    {
        Vector3 adjustedDirection = transform.TransformDirection(translation);
        rb.velocity = adjustedDirection;
    }
}
