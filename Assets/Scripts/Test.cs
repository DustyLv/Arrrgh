using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public RawImage img;
    public Transform obj;
    private void Start()
    {
        img.texture = RuntimePreviewGenerator.GenerateModelPreview(obj);
    }

}
