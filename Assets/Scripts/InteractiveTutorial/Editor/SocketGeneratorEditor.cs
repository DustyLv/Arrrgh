using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace XRVersion
{
    [CustomEditor(typeof(SocketGenerator))]
    public class SocketGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SocketGenerator myScript = (SocketGenerator)target;
            if (GUILayout.Button("Generate"))
            {
                myScript.Generate();
            }
        }

    }
}
