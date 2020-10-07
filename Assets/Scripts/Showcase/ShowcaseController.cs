using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

public class ShowcaseController : MonoBehaviour
{
    public int m_SetIndex = 0;
    public Transform m_TargetObject;

    public List<Set> m_Sets;

    public List<GameObject> m_SetGameobjects = new List<GameObject>();

    public List<ObjectOrigin> m_OriginalTransforms = new List<ObjectOrigin>();

    private int m_SetSortTemp = 0;
    private bool m_IsAnimating = false;

    [Button]
    public void SaveOriginalPositions()
    {
        m_OriginalTransforms.Clear();
        foreach (Transform t in m_TargetObject)
        {
            ObjectOrigin oo = new ObjectOrigin();
            oo.m_Object = t;
            oo.m_Position = t.localPosition;
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
            m_Sets[m_SetSortTemp].m_SetObjects.Insert(2, GetGroupParentObjectByName("Group0"));
            m_Sets[m_SetSortTemp].m_SetObjects.Insert(3, GetGroupParentObjectByName("Group1"));
        }
        if (m_SetSortTemp == 3)
        {
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

    IEnumerator AssembleNextGroupCoroutine(int index)
    {
        m_IsAnimating = true;
        foreach (Transform t in m_SetGameobjects[index].transform)
        {
            t.DOLocalMove(FindObjectOriginalLocation(t), 0.5f);
            yield return new WaitForSeconds(0.5f);
        }
        m_SetIndex++;
        yield return null;
        m_IsAnimating = false;
    }

    Vector3 FindObjectOriginalLocation(Transform targetObject)
    {
        foreach(ObjectOrigin t in m_OriginalTransforms)
        {
            if(t.m_Object == targetObject)
            {
                return t.m_Position;
            }
        }
        return Vector3.zero;
    }

}

[System.Serializable]
public class Set
{
    public List<GameObject> m_SetObjects;
}

[System.Serializable]
public class ObjectOrigin
{
    public Transform m_Object;
    public Vector3 m_Position;
}