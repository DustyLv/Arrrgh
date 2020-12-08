using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using ShowcaseV2;

public class PartToggler : MonoBehaviour
{
    public string m_ShaderNormal_N = "Universal Render Pipeline/Lit";
    public string m_ShaderTransparent_N = "Shader Graphs/Fresnel";

    public GameObject m_UITogglePrefab;
    public Transform m_UIContentRoot;


    public Transform m_Target;
    //private List<Transform> m_TargetObjects = new List<Transform>();

    private ShowcaseControllerV2 m_ShowcaseController;
    private Shader m_ShaderNormal;
    private Shader m_ShaderTransparent;

    private void Awake()
    {
        //foreach(Transform t in m_Target)
        //{
        //    m_TargetObjects.Add(t);
        //}

        m_ShowcaseController = GameObject.FindObjectOfType<ShowcaseControllerV2>();
        m_ShaderNormal = Shader.Find(m_ShaderNormal_N);
        m_ShaderTransparent = Shader.Find(m_ShaderTransparent_N);
        
    }

    void Start()
    {
        SetUpUI();
    }

    void Update()
    {
        
    }

    public void SetUpUI()
    {
        foreach(GameObject obj in m_ShowcaseController.m_AllObjects)
        {
            GameObject newObject = Instantiate(m_UITogglePrefab, m_UIContentRoot);
            Toggle toggle = newObject.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(delegate {
                ToggleShader(obj);
            });
            TextMeshProUGUI text = newObject.GetComponentInChildren<TextMeshProUGUI>();
            string formattedText = obj.name.Replace("_", " ");
            text.text = formattedText;
            RawImage prev = newObject.transform.Find("preview").GetComponent<RawImage>();

            RuntimePreviewGenerator.OrthographicMode = true;
            RuntimePreviewGenerator.BackgroundColor = new Color(1f, 1f, 1f, 0f);

            Texture prevImg = RuntimePreviewGenerator.GenerateModelPreviewWithShader(obj.transform, Shader.Find("Unlit/Texture"), string.Empty, 128, 128, true);
            prevImg.name = obj.gameObject.name + "_Thumb";

            prev.texture = prevImg;
        }
    }

    public void ToggleShader(GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if(rend != null)
        {
            if(rend.material.shader == m_ShaderNormal)
            {
                rend.material.shader = m_ShaderTransparent;
            }
            else if (rend.material.shader == m_ShaderTransparent)
            {
                rend.material.shader = m_ShaderNormal;
            }
        }
    }
}
