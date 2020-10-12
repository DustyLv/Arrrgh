using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class ShowcaseController : MonoBehaviour
{
    
    public Transform m_TargetObject;
    public Transform m_AuxTargetObject;
    public float m_TweenLength = 0.25f;
    public float m_PauseBetweenTweens = 0.25f;

    [HorizontalLine(color: EColor.Black)]
    [ReadOnly] public int m_SetIndex = 0;

    public List<Set> m_Sets;

    public List<Transform> m_SetHoldingPoints = new List<Transform>();

    [InfoBox("Sets in arrays are referenced starting from 0. (Ex., to get the first set, we need to specify it as 0.). Set names in code are referenced as 'Groups', so for custom objects we need to use that naming (ex., first set would be Group0).", EInfoBoxType.Normal)]
    [Tooltip("Add sets to other sets to animate them together.")] 
    public List<CustomSetObject> m_CustomSetObjects = new List<CustomSetObject>();
    [Tooltip("Add sets that need to be moved to holding points together.")]
    public List<CustomSetMoveToHolding> m_CustomSetObjectHoldingChildren = new List<CustomSetMoveToHolding>();

    [HorizontalLine(color: EColor.Black)]

    [SerializeField] private List<GameObject> m_SetGameobjects = new List<GameObject>();
    [SerializeField] private List<ObjectOrigin> m_OriginalTransforms = new List<ObjectOrigin>();
    [SerializeField] private List<ObjectOrigin> m_ScatteredTransforms = new List<ObjectOrigin>();
    [SerializeField] private List<ObjectOrigin> m_GroupFinalTransforms = new List<ObjectOrigin>();


    private List<Transform> m_TempScatteredTransforms = new List<Transform>();

    private List<SetTweens> m_Tweens = new List<SetTweens>();

    private int m_SetSortTemp = 0;
    private bool m_IsAnimating = false;
    private WaitForSeconds m_PauseBetweenTweensTimer;

    [HorizontalLine(color: EColor.Black)]

    public TextMeshProUGUI m_NowAssemblingText;
    public TextMeshProUGUI m_OrderOutputText;

    [InfoBox("JSON exporting/importing currently disabled.", EInfoBoxType.Warning)]
    [ReadOnly]
    public int ingoreInt;

    private void Start()
    {
        DOTween.defaultAutoKill = false;
        DOTween.defaultRecyclable = true;
        m_PauseBetweenTweensTimer = new WaitForSeconds(m_PauseBetweenTweens);

        //SaveOriginalPositions();
        SaveScatteredPositions();
        SortIntoSets();

        for (int i = 0; i < m_Sets.Count; i++)
        {
            m_Tweens.Add(new SetTweens());
        }

        SetTextAssembling("---");
        SetTextOrder("", true);
    }
    [Button]
    public void CopyOriginalPositionsFromAuxTargetObject()
    {
        m_OriginalTransforms.Clear();
        foreach (Transform t in m_AuxTargetObject)
        {
            ObjectOrigin oo = new ObjectOrigin(t, t.localPosition, t.localRotation.eulerAngles);
            m_OriginalTransforms.Add(oo);
        }
    }

    [Button]
    public void ReplaceOriginalPositionTransformsWithTargetObject()
    {
        for (int i = 0; i < m_OriginalTransforms.Count; i++)
        {
            m_OriginalTransforms[i].m_Object = m_TargetObject.GetChild(i);
        }
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

        // Add container objects for each set group and add the objects to it

        GameObject groupParent = new GameObject();
        groupParent.name = "Group" + m_SetSortTemp;
        groupParent.transform.SetParent(m_TargetObject);

        foreach (GameObject obj in m_Sets[m_SetSortTemp].m_SetObjects)
        {
            obj.transform.SetParent(groupParent.transform);
        }

        m_SetGameobjects.Add(groupParent);

        // To be able to use set groups for animation within another group, we need to insert that group object into the specific List 
        // Example, Group0 and Group1 (motor end shields) are a part of the Group2, and they need to be placed at the correct spots when animating Group3.

        foreach(CustomSetObject cso in m_CustomSetObjects)
        {
            if(m_SetSortTemp == cso.m_TargetSetIndex)
            {
                m_Sets[m_SetSortTemp].m_SetObjects.Insert(cso.m_IndexInListToAdd, FindGroupParentObjectByName(cso.m_CustomObjectName));
            }
        }

        //if(m_SetSortTemp == 2) 
        //{
        //    //GameObject g0 = new GameObject("Group0");
        //    //g0.transform.SetParent(groupParent.transform);

        //    //GameObject g1 = new GameObject("Group1");
        //    //g1.transform.SetParent(groupParent.transform);

        //    m_Sets[m_SetSortTemp].m_SetObjects.Insert(2, FindGroupParentObjectByName("Group0"));
        //    m_Sets[m_SetSortTemp].m_SetObjects.Insert(3, FindGroupParentObjectByName("Group1"));

        //    //print($"g0 id: {g0.GetInstanceID()}; found object id {FindGroupParentObjectByName("Group0").GetInstanceID()}");
        //}
        //if (m_SetSortTemp == 3)
        //{
        //    //GameObject g2 = new GameObject("Group2");
        //    //g2.transform.SetParent(groupParent.transform);

        //    m_Sets[m_SetSortTemp].m_SetObjects.Insert(0, FindGroupParentObjectByName("Group0"));
        //    m_Sets[m_SetSortTemp].m_SetObjects.Insert(0, FindGroupParentObjectByName("Group1"));
        //    m_Sets[m_SetSortTemp].m_SetObjects.Insert(0, FindGroupParentObjectByName("Group2"));
        //}

        if (m_SetSortTemp < m_Sets.Count)
        {
            m_SetSortTemp++;
            SortIntoSets();
        }
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
        SetTextAssembling("Assembling: Group" + index);
        if (m_Tweens[index].m_Tweens == null)
        {
            print("Creating a new tween and playing it");
            m_Tweens[index].m_Tweens = new List<Tweener>();

            SetTextOrder("<b>Assembly order:</b>",true);

            int stepNumber = 1;
            foreach (GameObject go in m_Sets[index].m_SetObjects)
            {
                ObjectOrigin curObj = FindObjectOriginal(go.transform);
                Tweener tp = go.transform.DOLocalMove(curObj.m_Position, m_TweenLength).SetAutoKill(false);
                Tweener tr = go.transform.DOLocalRotate(curObj.m_Rotation, m_TweenLength).SetAutoKill(false);
                m_Tweens[index].m_Tweens.Add(tp);
                m_Tweens[index].m_Tweens.Add(tr);
                tp.stringId = go.name;
                SetTextOrder(stepNumber + ". " + tp.stringId);
                //yield return m_PauseBetweenTweensTimer;
                yield return tp.WaitForCompletion();
                stepNumber++;
            }
            
            ObjectOrigin groupOrigin = new ObjectOrigin(m_SetGameobjects[index].transform, m_SetGameobjects[index].transform.position, m_SetGameobjects[index].transform.rotation.eulerAngles);

            m_OriginalTransforms.Add(groupOrigin);

            if (index < m_SetGameobjects.Count - 1)
            {
                // As long as there are enough holding points specified, each set group will be moved to a holding point
                m_Tweens[index].m_Tweens.Add(m_SetGameobjects[index].transform.DOLocalMove(m_SetHoldingPoints[index].position, m_TweenLength).SetAutoKill(false));

                // For specific set groups there are other/child groups that need to be moved to a holding point with the parent one
                // Here we specify for which parent group, which child groups are moved with them

                foreach(CustomSetMoveToHolding c in m_CustomSetObjectHoldingChildren)
                {
                    if(index == c.m_TargetSetIndex)
                    {
                        m_Tweens[index].m_Tweens.Add(m_SetGameobjects[c.m_ChildSetIndex].transform.DOLocalMove(m_SetHoldingPoints[index].position, m_TweenLength).SetAutoKill(false));
                    }
                }

                //if (index == 2)
                //{
                //    m_Tweens[index].m_Tweens.Add(m_SetGameobjects[0].transform.DOLocalMove(m_SetHoldingPoints[index].position, m_TweenLength).SetAutoKill(false));
                //    m_Tweens[index].m_Tweens.Add(m_SetGameobjects[1].transform.DOLocalMove(m_SetHoldingPoints[index].position, m_TweenLength).SetAutoKill(false));
                //}
                //if (index == 3)
                //{
                //    m_Tweens[index].m_Tweens.Add(m_SetGameobjects[2].transform.DOLocalMove(m_SetHoldingPoints[index].position, m_TweenLength).SetAutoKill(false));
                //}
            }

        }
        else
        {
            SetTextOrder("", true);
            print("Playing a stored sequence");
            int stepNumber = 1;
            foreach (Tween t in m_Tweens[index].m_Tweens)
            {
                SetTextOrder(stepNumber + ". " + t.stringId);
                t.PlayForward();
                yield return t.WaitForCompletion();
                //yield return m_PauseBetweenTweensTimer;
                stepNumber++;
            }

        }

        m_SetIndex++;
        yield return null;
        SetTextAssembling("---");
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
            SetTextAssembling("Disassembling: Group" + index);
            SetTextOrder("<b>Disassembly order:</b>", true);
            int stepNumber = 1;
            foreach (Tween t in m_Tweens[index].m_Tweens.Cast<Tween>().Reverse())
            {
                SetTextOrder(stepNumber + ". " + t.stringId);
                t.PlayBackwards();
                yield return t.WaitForCompletion();
                //yield return m_PauseBetweenTweensTimer;
                stepNumber++;
            }

            m_SetIndex = index;
            yield return null;
            SetTextAssembling("---");
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

    private GameObject FindGroupParentObjectByName(string name)
    {
        foreach (GameObject go in m_SetGameobjects)
        {
            if (go.name == name)
            {
                return go;
            }
        }
        return null;
    }

    private GameObject GetGameObjectByNameInTarget(string name)
    {
        foreach (Transform t in m_TargetObject)
        {
            if (t.gameObject.name == name)
            {
                return t.gameObject;
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

    void SetTextAssembling(string text)
    {
        m_NowAssemblingText.text = text;
    }

    void SetTextOrder(string text, bool reset = false)
    {
        if (reset) { m_OrderOutputText.text = ""; }

        m_OrderOutputText.text += text + System.Environment.NewLine;
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

[System.Serializable]
public class SetTweens
{
    public List<Tweener> m_Tweens;
}

[System.Serializable]
public class CustomSetObject
{
    [Tooltip("Target set to which the custom object will be added to.")]
    public int m_TargetSetIndex = 0;
    [Tooltip("The index in list where the custom object will be added. To specify animation order.")]
    public int m_IndexInListToAdd = 0;
    [Tooltip("The custom objects name to search for.")]
    public string m_CustomObjectName = "";
}

[System.Serializable]
public class CustomSetMoveToHolding
{
    [Tooltip("Target set index to specify with which set to move the other set.")]
    public int m_TargetSetIndex = 0;
    [Tooltip("The other set which will be moved.")]
    public int m_ChildSetIndex = 0;
}