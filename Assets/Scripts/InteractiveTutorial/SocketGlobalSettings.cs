using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketGlobalSettings : MonoBehaviour
{
    public Color m_ColorIdle = Color.black;
    public Color m_ColorValid = Color.black;
    public Color m_ColorInvalid = Color.black;

    public static SocketGlobalSettings i;

    private void Awake()
    {
        i = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
