using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
using System.Linq;

public class ShowcaseController : MonoBehaviour
{
    
    public Transform m_TargetObject;
    public float m_TweenLength = 0.25f;
    public float m_PauseBetweenTweens = 0.25f;

    [HorizontalLine(color: EColor.Black)]
    [ReadOnly] public int m_SetIndex = 0;

    public List<Set> m_Sets;

    public List<Transform> m_SetHoldingPoints = new List<Transform>();

    public List<GameObject> m_SetGameobjects = new List<GameObject>();
    public List<ObjectOrigin> m_OriginalTransforms = new List<ObjectOrigin>();
    public List<ObjectOrigin> m_ScatteredTransforms = new List<ObjectOrigin>();
    public List<ObjectOrigin> m_GroupFinalTransforms = new List<ObjectOrigin>();

    private int m_SetSortTemp = 0;
    private bool m_IsAnimating = false;

    [InfoBox("JSON exporting/importing currently disabled.", EInfoBoxType.Warning)]
    [ReadOnly]
    public int ingoreInt;

    private void Start()
    {
        //SaveOriginalPositions();
        //ScatterObjects();
        SaveScatteredPositions();
        SortIntoSets();
    }

    [Button]
    public void SaveOriginalPositions()
    {
        m_OriginalTransforms.Clear();
        foreach (Transform t in m_TargetObject)
        {
            ObjectOrigin oo = new ObjectOrigin(t, t.localPosition, t.localRotation.eulerAngles);
            m_OriginalTransforms.Add(oo);
        }
    }

    [Button]
    public void ScatterObjects()
    {
        foreach (Transform t in m_TargetObject)
        {
            Vector3 randomPos = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
            t.localPosition += randomPos;
            t.localRotation = Random.rotation;
            ObjectOrigin oo = new ObjectOrigin(t, t.localPosition, t.localRotation.eulerAngles);
            m_ScatteredTransforms.Add(oo);
        }
    }

    private void SaveScatteredPositions()
    {
        foreach (Transform t in m_TargetObject)
        {
            ObjectOrigin oo = new ObjectOrigin(t, t.localPosition, t.localRotation.eulerAngles);
            m_ScatteredTransforms.Add(oo);
        }
    }

    [Button]
    public void SortIntoSets()
    {
        if(m_SetSortTemp >= m_Sets.Count)
        {
            return;
        }
        GameObject groupParent = new GameObject();
        groupParent.name = "Group" + m_SetSortTemp;
        groupParent.transform.SetParent(m_TargetObject);

        foreach (GameObject obj in m_Sets[m_SetSortTemp].m_SetObjects)
        {
            obj.transform.SetParent(groupParent.transform);
        }

        m_SetGameobjects.Add(groupParent);

        if(m_SetSortTemp == 2)
        {
            GameObject g0 = new GameObject("Group0");
            g0.transform.SetParent(groupParent.transform);

            GameObject g1 = new GameObject("Group1");
            g1.transform.SetParent(groupParent.transform);

            m_Sets[m_SetSortTemp].m_SetObjects.Insert(2, GetGroupParentObjectByName("Group0"));
            m_Sets[m_SetSortTemp].m_SetObjects.Insert(3, GetGroupParentObjectByName("Group1"));
        }
        if (m_SetSortTemp == 3)
        {
            GameObject g2 = new GameObject("Group2");
            g2.transform.SetParent(groupParent.transform);

            m_Sets[m_SetSortTemp].m_SetObjects.Insert(0, GetGroupParentObjectByName("Group0"));
            m_Sets[m_SetSortTemp].m_SetObjects.Insert(0, GetGroupParentObjectByName("Group1"));
            m_Sets[m_SetSortTemp].m_SetObjects.Insert(0, GetGroupParentObjectByName("Group2"));
        }

        if (m_SetSortTemp < m_Sets.Count)
        {
            m_SetSortTemp++;
            SortIntoSets();
        }
    }

    private GameObject GetGroupParentObjectByName(string name)
    {
        foreach(GameObject go in m_SetGameobjects)
        {
            if (go.name == name)
            {
                return go;
            }
        }
        return null;
    }

    [Button]
    public void ResetObjectPositions()
    {

        for (int i = 0; i < m_OriginalTransforms.Count; i++)
        {
            m_TargetObject.GetChild(i).localPosition = m_OriginalTransforms[i].m_Position;
            m_TargetObject.GetChild(i).localRotation = Quaternion.Euler(m_OriginalTransforms[i].m_Rotation);
        }
    }

    [Button]
    public void AssembleNextGroup()
    {
        if(m_SetIndex >= m_SetGameobjects.Count)
        {
            return;
        }

        if(m_IsAnimating == false)
        {
            StartCoroutine(AssembleNextGroupCoroutine(m_SetIndex));
        }
    }

    [Button]
    public void AssemblePreviousGroup()
    {
        if (m_SetIndex < 0)
        {
            return;
        }

        if (m_IsAnimating == false)
        {
            StartCoroutine(AssemblePreviousGroupCoroutine(m_SetIndex));
        }
    }

    IEnumerator AssembleNextGroupCoroutine(int index)
    {
        m_IsAnimating = true;
        foreach(GameObject go in m_Sets[index].m_SetObjects)
        {
            print($"Processing: {go.gameObject.name}");
            ObjectOrigin curObj = FindObjectOriginal(go.transform);
            go.transform.DOLocalMove(curObj.m_Position, m_TweenLength);
            go.transform.DOLocalRotate(curObj.m_Rotation, m_TweenLength);
            yield return new WaitForSeconds(m_PauseBetweenTweens);
        }

        ObjectOrigin groupOrigin = new ObjectOrigin(m_SetGameobjects[index].transform, m_SetGameobjects[index].transform.position, m_SetGameobjects[index].transform.rotation.eulerAngles);

        m_OriginalTransforms.Add(groupOrigin);

        if (index < m_SetGameobjects.Count - 1)
        {
            m_SetGameobjects[index].transform.DOLocalMove(m_SetHoldingPoints[index].position, m_TweenLength);

            if (index == 2)
            {
                m_SetGameobjects[0].transform.DOLocalMove(m_SetHoldingPoints[index].position, m_TweenLength);
                m_SetGameobjects[1].transform.DOLocalMove(m_SetHoldingPoints[index].position, m_TweenLength);
            }
            if (index == 3)
            {
                m_SetGameobjects[2].transform.DOLocalMove(m_SetHoldingPoints[index].position, m_TweenLength);
            }
        }

        m_SetIndex++;
        yield return null;
        m_IsAnimating = false;
    }
    IEnumerator AssemblePreviousGroupCoroutine(int index)
    {
        index -= 1;
        if (index < 0)
        {
            yield return null;
        }
        else
        {
            m_IsAnimating = true;
            //foreach (Transform t in m_SetGameobjects[index].transform.Cast<Transform>().Reverse())
            //{
            //    ObjectOrigin curObj = FindObjectScattered(t);

            //    t.DOLocalMove(curObj.m_Position, m_TweenLength);
            //    t.DOLocalRotate(curObj.m_Rotation, m_TweenLength);
            //    yield return new WaitForSeconds(m_PauseBetweenTweens);
            //}
            foreach (GameObject go in m_Sets[index].m_SetObjects.Cast<GameObject>().Reverse())
            {
                //print($"Processing: {go.gameObject.name}");
                ObjectOrigin curObj = FindObjectScattered(go.transform);
                go.transform.DOLocalMove(curObj.m_Position, m_TweenLength);
                go.transform.DOLocalRotate(curObj.m_Rotation, m_TweenLength);
                yield return new WaitForSeconds(m_PauseBetweenTweens);
            }

            m_SetIndex = index;
            yield return null;
            m_IsAnimating = false;
        }
    }

    ObjectOrigin FindObjectOriginal(Transform targetObject)
    {
        foreach(ObjectOrigin t in m_OriginalTransforms)
        {
            if(t.m_Object == targetObject)
            {
                return t;
            }
        }
        return null;
    }

    ObjectOrigin FindObjectScattered(Transform targetObject)
    {
        foreach (ObjectOrigin t in m_ScatteredTransforms)
        {
            if (t.m_Object == targetObject)
            {
                return t;
            }
        }
        return null;
    }
    ObjectOrigin FindGroupContainer(Transform targetObject)
    {
        foreach (ObjectOrigin t in m_GroupFinalTransforms)
        {
            if (t.m_Object == targetObject)
            {
                return t;
            }
        }
        return null;
    }


    [Button]
    public void ExportSetConfig()
    {
        List<SetGOExport> m_SetObjectListTemp = new List<SetGOExport>();
        m_SetObjectListTemp.Clear();


        foreach(Set set in m_Sets)
        {
            SetGOExport goExp = new SetGOExport();
            goExp.m_SetObjects = new List<string>();
            foreach(GameObject go in set.m_SetObjects)
            {
                goExp.m_SetObjects.Add(go.name);
            }
            m_SetObjectListTemp.Add(goExp);
        }
        SetGOExportWrapper wrapper = new SetGOExportWrapper();
        wrapper.objects = m_SetObjectListTemp;

        string json = JsonUtility.ToJson(wrapper, true);
        //System.IO.File.WriteAllText(Application.dataPath + "/SetExport.json", json);
    }

    [Button]
    public void ImportSetConfig()
    {
        string path = System.IO.Path.Combine(Application.dataPath, "SetExport.json");
        string json = System.IO.File.ReadAllText(path);


        SetGOExportWrapper wrapper = new SetGOExportWrapper();
        wrapper = JsonUtility.FromJson<SetGOExportWrapper>(json);
        foreach (SetGOExport goExp in wrapper.objects)
        {
            Set newSet = new Set();
            newSet.m_SetObjects = new List<GameObject>();
            foreach(string go in goExp.m_SetObjects)
            {
                newSet.m_SetObjects.Add(GetGameObjectByNameInTarget(go));
            }
            //m_SetsTemp.Add(newSet);
        }
    }

    private GameObject GetGameObjectByNameInTarget(string name)
    {
        foreach(Transform t in m_TargetObject)
        {
            if(t.gameObject.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }

}

[System.Serializable]
public class Set
{
    public List<GameObject> m_SetObjects;
}

[System.Serializable]
public class SetGOExport
{
    [SerializeField] public List<string> m_SetObjects;
}

[System.Serializable]
public class SetGOExportWrapper
{
    [SerializeField] public List<SetGOExport> objects;
}

[System.Serializable]
public class ObjectOrigin
{
    public Transform m_Object;
    public Vector3 m_Position;
    public Vector3 m_Rotation;

    public ObjectOrigin(Transform _transform, Vector3 _position, Vector3 _rotation)
    {
        m_Object = _transform;
        m_Position = _position;
        m_Rotation = _rotation;
    }
}