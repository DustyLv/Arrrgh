using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.IO;
using System.Linq;

public class InteractiveTutorialController : MonoBehaviour
{
    public List<GameObject> m_ObjectOrderList = new List<GameObject>();

    public List<string> m_ConfigList = new List<string>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button]
    public void OrderObjectListFromConfigFile()
    {
        string filename = Application.dataPath + "/Resources/ObjectOrder.txt";
        m_ConfigList = File.ReadAllLines(filename).ToList();

        List<GameObject> tempObjectList = new List<GameObject>();
        tempObjectList = m_ObjectOrderList;

        GameObject[] tempSortList = new GameObject[m_ConfigList.Count];
        
        foreach (GameObject go in tempObjectList)
        {
            foreach (string objName in m_ConfigList)
            {
                if (go.name == objName)
                {
                    int indexConfig = m_ConfigList.IndexOf(objName);
                    tempSortList[indexConfig] = go;
                    continue;
                }
            }
        }

        m_ObjectOrderList = tempSortList.ToList();
    }
}
