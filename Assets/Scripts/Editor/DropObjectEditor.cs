using UnityEditor;
using UnityEngine;
using System.Collections;


class DropObjectEditor : EditorWindow
{
    // add menu item
    [MenuItem("Window/Drop Object")]


    static void Init()
    {
        // Get existing open window or if none, make a new one:
        DropObjectEditor window = (DropObjectEditor)EditorWindow.GetWindow(typeof(DropObjectEditor), true);
        window.Show();
    }

    public void OnGUI()
    {
        GUILayout.Label("Drop Using:", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Bottom"))
        {
            DropObjects("Bottom");
        }

        if (GUILayout.Button("Origin"))
        {
            DropObjects("Origin");
        }

        if (GUILayout.Button("Center"))
        {
            DropObjects("Center");
        }

        GUILayout.EndHorizontal();
    }

    void DropObjects(string method)
    {
        // drop multi-selected objects using the right method
        for (int i = 0; i < Selection.transforms.Length; i++)
        {
            // get the game object
            GameObject go = Selection.transforms[i].gameObject;

            // don't think I need to check, but just to be sure...
            if (!go)
            {
                continue;
            }

            // get the bounds
            Bounds bounds = go.GetComponent<Renderer>().bounds;
            RaycastHit hit;
                float yOffset = 0f;

            // override layer so it doesn't hit itself
            int savedLayer = go.layer;
            go.layer = 2; // ignore raycast
                            // see if this ray hit something
            if (Physics.Raycast(go.transform.position, -Vector3.up, out hit))
            {
                // determine how the y will need to be adjusted
                switch (method)
                {
                    case "Bottom":
                        yOffset = go.transform.position.y - bounds.min.y;
                        break;
                    case "Origin":
                        yOffset = 0f;
                        break;
                    case "Center":
                        yOffset = bounds.center.y - go.transform.position.y;
                        break;
                }
                go.transform.position = hit.point;
                go.transform.position += new Vector3(0f, yOffset, 0f);
            }
            // restore layer
            go.layer = savedLayer;
        }
    }
}