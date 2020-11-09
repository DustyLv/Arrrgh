using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.IO;

namespace ShowcaseV2
{
    public class ShowcaseControllerV2 : MonoBehaviour
    {

        public Transform m_TargetObject;
        public Transform m_AuxTargetObject;
        public float m_TweenLength = 0.25f;
        public float m_PauseBetweenTweens = 0.25f;

        //[HorizontalLine(color: EColor.Black)]
        //public float m_TweenTimescale = 1f;

        [HorizontalLine(color: EColor.Black)]
        [ReadOnly] public int m_groupIndex = 0;
        [ReadOnly] public int m_StepIndex = 0;

        public List<Group> m_Groups;

        //public List<Transform> m_SetHoldingPoints = new List<Transform>();
        public BoxCollider m_ObjectHoldingVolume;

        public List<BoxCollider> m_ObjectScatterVolumes = new List<BoxCollider>();

        [InfoBox("Groups in arrays are referenced starting from 0. (Ex., to get the first group, we need to specify it as 0.). For custom objects we need to use that naming (ex., first group would be Group0).", EInfoBoxType.Normal)]
        [Tooltip("Add sets to other sets to animate them together.")]
        public List<CustomGroupObject> m_CustomGroupObjects = new List<CustomGroupObject>();
        //[Tooltip("Add sets that need to be moved to holding points together.")]
        //public List<CustomSetMoveToHolding> m_CustomSetObjectHoldingChildren = new List<CustomSetMoveToHolding>();

        [HorizontalLine(color: EColor.Black)]

        [SerializeField] private List<GroupGameObjects> m_GroupGameObjects = new List<GroupGameObjects>();
        [SerializeField] private List<ObjectOrigin> m_OriginalTransforms = new List<ObjectOrigin>();
        [SerializeField] private List<ObjectOrigin> m_ScatteredTransforms = new List<ObjectOrigin>();
        [SerializeField] private List<ObjectOrigin> m_GroupFinalTransforms = new List<ObjectOrigin>();


        private List<Transform> m_TempScatteredTransforms = new List<Transform>();

        public List<GroupTweens> m_GroupTweens = new List<GroupTweens>();

        private int m_SetSortTemp = 0;
        private bool m_IsAnimating = false;
        private WaitForSeconds m_PauseBetweenTweensTimer;

        [HorizontalLine(color: EColor.Black)]

        public TextMeshProUGUI m_NowAssemblingText;
        public TextMeshProUGUI m_OrderOutputText;

        [InfoBox("JSON exporting/importing currently disabled.", EInfoBoxType.Warning)]
        [ReadOnly]
        public int ingoreInt;

        public TweenMasterController m_TweenMaster;

        //public int m_RemoveFromGroudIndex = 0;
        //public int m_RemoveFromStepIndex = 0;

        //[Button]
        //public void RemoveFromGroup()
        //{
        //    m_Groups[m_RemoveFromGroudIndex].m_Group.RemoveAt(m_RemoveFromStepIndex);
        //}

        private void Start()
        {
            DOTween.defaultAutoKill = false;
            DOTween.defaultRecyclable = true;
            m_PauseBetweenTweensTimer = new WaitForSeconds(m_PauseBetweenTweens);

            //SaveOriginalPositions();

            ScatterObjects();
            SaveScatteredPositions();
            SortIntoSets();

            // after parts have been sorted into sets, they are now under Group parent objects. Those objects also need their original positions saved.
            foreach(Transform t in m_TargetObject)
            {
                ObjectOrigin oo = new ObjectOrigin(t, t.localPosition, t.localRotation.eulerAngles);
                m_OriginalTransforms.Add(oo);
            }

            int id = 0;
            foreach(GroupGameObjects ggo in m_GroupGameObjects)
            {
                m_GroupTweens.Add(new GroupTweens());
                m_GroupTweens[id].m_Steps = new List<StepTweens>();
                foreach(GameObject go in ggo.m_StepObjects)
                {
                    
                    m_GroupTweens[id].m_Steps.Add(new StepTweens());
                }
                id++;
            }

            SetTextAssembling("---");
            SetTextOrder("", true);

            //print(m_Groups[m_groupIndex].m_Group[0]);
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

        //[Button]
        public void ScatterObjects()
        {
            foreach (Transform t in m_TargetObject)
            {
                BoxCollider targetVolume = m_ObjectScatterVolumes[Random.Range(0, m_ObjectScatterVolumes.Count)];
                Bounds bounds = t.GetComponent<MeshRenderer>().bounds;
                //float radius = Mathf.Max(boundsMax.x, boundsMax.y);
                PlaceObjectInVolume(t, bounds, targetVolume, 0);
                
            }
        }

        private void PlaceObjectInVolume(Transform target, Bounds bounds, BoxCollider targetVolume, int iteration)
        {
            Vector3 newRandPos = GetRandomPositionInsideBox(targetVolume);
            newRandPos.y = 0f;
            newRandPos.y += bounds.extents.y;

            float radius = Mathf.Max(bounds.max.x, bounds.max.z);

            Collider[] hitColliders = Physics.OverlapSphere(newRandPos, radius);
            if (hitColliders.Length < 0 && iteration < 5)
            {
                PlaceObjectInVolume(target, bounds, targetVolume, iteration++);
            }
            else
            {
                target.localPosition = newRandPos;
            }
        }

        private int GetRandomPosNeg()
        {
            return Random.Range(0, 2) * 2 - 1;
        }

        //[Button]
        public void SaveScatteredPositions()
        {
            foreach (Transform t in m_TargetObject)
            {
                ObjectOrigin oo = new ObjectOrigin(t, t.localPosition, t.localRotation.eulerAngles);
                m_ScatteredTransforms.Add(oo);
            }
        }

        //[Button]
        public void SortIntoSets()
        {
            if (m_SetSortTemp >= m_Groups.Count)
            {
                return;
            }

            // Add container objects for each set group and add the objects to it

            GameObject groupParent = new GameObject();
            groupParent.name = "Group" + m_SetSortTemp;
            groupParent.transform.SetParent(m_TargetObject);
            groupParent.transform.position = Vector3.zero;

            int stepID = 0;
            GroupGameObjects ggo = new GroupGameObjects();
            ggo.m_StepObjects = new List<GameObject>();
            ggo.m_GroupObject = groupParent;

            foreach (GroupStep group in m_Groups[m_SetSortTemp].m_Group)
            {
                GameObject stepParent = new GameObject();
                stepParent.name = "Step" + stepID;
                stepParent.transform.SetParent(groupParent.transform);

                if (group.m_GroupStepObjects.Count > 0)
                {
                    foreach (GameObject obj in group.m_GroupStepObjects)
                    {
                        
                        obj.transform.SetParent(stepParent.transform);
                    }
                }
                if (stepParent != null)
                {
                    ggo.m_StepObjects.Add(stepParent);
                }

                stepID++;
            }

            m_GroupGameObjects.Add(ggo);

            // To be able to use set groups for animation within another group, we need to insert that group object into the specific List 
            // Example, Group0 and Group1 (motor end shields) are a part of the Group2, and they need to be placed at the correct spots when animating Group3.

            foreach (CustomGroupObject cso in m_CustomGroupObjects)
            {
                if (m_SetSortTemp == cso.m_TargetGroupIndex)
                {
                    m_Groups[m_SetSortTemp].m_Group[cso.m_TargetStepIndex].m_GroupStepObjects.Insert(cso.m_IndexInListToAdd, FindGroupParentObjectByName(cso.m_CustomObjectName));
                }
            }

            if (m_SetSortTemp < m_Groups.Count)
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
            if (m_groupIndex >= m_GroupGameObjects.Count)
            {
                return;
            }

            if (m_IsAnimating == false)
            {
                if(m_StepIndex < m_Groups[m_groupIndex].m_Group.Count)
                {
                    //print($"stepIndex:  {m_StepIndex}/{m_Groups[m_groupIndex].m_Group.Count}");
                    StartCoroutine(AssembleNextStepCoroutine(/*m_StepIndex*/));
                }
                else if(m_groupIndex + 1 < m_GroupGameObjects.Count)
                {
                    //print($"groupIndex:  {m_groupIndex}/{m_Groups.Count}");
                    m_groupIndex++;
                    m_StepIndex = 0;
                    StartCoroutine(AssembleNextStepCoroutine(/*m_StepIndex*/));
                }
                
            }
        }


        [Button]
        public void AssemblePreviousGroup()
        {
            if (m_groupIndex <= 0 && m_StepIndex <= 0)
            {
                return;
            }

            if (m_IsAnimating == false)
            {
                m_StepIndex -= 1;
                if (m_StepIndex >= 0)
                {
                    StartCoroutine(AssemblePreviousStepCoroutine(/*m_StepIndex*/));
                }
                else
                {
                    m_groupIndex--;
                    m_StepIndex = m_Groups[m_groupIndex].m_Group.Count - 1;
                    StartCoroutine(AssemblePreviousStepCoroutine(/*m_StepIndex*/));
                }
            }
        }

        IEnumerator AssembleNextStepCoroutine(/*int index*/)
        {
            m_IsAnimating = true;
            SetTextAssembling(m_Groups[m_groupIndex].m_Group[m_StepIndex].m_GroupStepOutputText);
            if (m_GroupTweens[m_groupIndex].m_Steps[m_StepIndex].m_StepTweens == null)
            {
                //print("Creating a new tween and playing it");
                m_GroupTweens[m_groupIndex].m_Steps[m_StepIndex].m_StepTweens = new List<Tweener>();

                foreach (GameObject go in m_Groups[m_groupIndex].m_Group[m_StepIndex].m_GroupStepObjects)
                {
                    ObjectOrigin curObj = FindObjectOriginal(go.transform);
                    Tweener tp = go.transform.DOMove(curObj.m_Position, m_TweenLength).SetAutoKill(false);
                    Tweener tr = go.transform.DORotate(curObj.m_Rotation, m_TweenLength).SetAutoKill(false);
                    m_GroupTweens[m_groupIndex].m_Steps[m_StepIndex].m_StepTweens.Add(tp);
                    m_GroupTweens[m_groupIndex].m_Steps[m_StepIndex].m_StepTweens.Add(tr);
                    tp.stringId = go.name;

                    //yield return m_PauseBetweenTweensTimer;
                    yield return tp.WaitForCompletion();
                }

                if(m_StepIndex >= m_Groups[m_groupIndex].m_Group.Count - 1)
                {
                    if (m_Groups[m_groupIndex].m_Group[m_StepIndex].m_PutAsideWhenAssembled == true)
                    {
                        Transform groupParent = m_Groups[m_groupIndex].m_Group[m_StepIndex].m_GroupStepObjects[0].transform.parent.transform.parent.transform;
                        m_GroupTweens[m_groupIndex].m_Steps[m_StepIndex].m_StepTweens.Add(groupParent.DOMove(GetRandomPositionInsideBox(m_ObjectHoldingVolume), m_TweenLength).SetAutoKill(false));
                    }
                }

            }
            else
            {
                //print("Playing a stored sequence");
                foreach (Tween t in m_GroupTweens[m_groupIndex].m_Steps[m_StepIndex].m_StepTweens)
                {
                    t.PlayForward();
                    yield return t.WaitForCompletion();
                    //yield return m_PauseBetweenTweensTimer;
                }

            }

            m_StepIndex++;
            yield return null;
            m_IsAnimating = false;
        }
        IEnumerator AssemblePreviousStepCoroutine()
        {
            if (m_StepIndex < 0)
            {
                yield return null;
            }
            else
            {
                m_IsAnimating = true;
                SetTextAssembling(m_Groups[m_groupIndex].m_Group[m_StepIndex].m_GroupStepOutputText);

                foreach (Tween t in m_GroupTweens[m_groupIndex].m_Steps[m_StepIndex].m_StepTweens.Cast<Tween>().Reverse())
                {
                    t.PlayBackwards();
                    yield return t.WaitForCompletion();
                    //yield return m_PauseBetweenTweensTimer;
                }



                //Transform groupObjectToReset = m_GroupGameObjects[m_groupIndex].m_GroupObject.transform;
                //groupObjectToReset.localPosition = Vector3.zero;
                //groupObjectToReset.localRotation = Quaternion.Euler(Vector3.zero);

                //foreach (GameObject go in m_Groups[m_groupIndex].m_Group[m_StepIndex].m_GroupStepObjects.Cast<GameObject>().Reverse())
                //{
                //    //print(go.name);

                //    // SUCH A JANKY WAY, BUT SHOULD WORK
                //    if (go.name.Contains("Group"))
                //    {
                //        foreach (Transform subGo in go.transform)
                //        {
                //            //print(subGo.name);
                //            if (subGo.name.Contains("Step"))
                //            {
                //                foreach (Transform subStepGo in subGo)
                //                {
                //                    ObjectOrigin curObjSub = FindObjectScattered(subStepGo.transform);
                //                    Tweener tpSub = subStepGo.transform.DOMove(curObjSub.m_Position, m_TweenLength).SetAutoKill(false);
                //                    Tweener trSub = subStepGo.transform.DORotate(curObjSub.m_Rotation, m_TweenLength).SetAutoKill(false);

                //                    //tp.stringId = go.name;

                //                    yield return tpSub.WaitForCompletion();
                //                }
                //            }
                //            else
                //            {
                //                ObjectOrigin curObjSub = FindObjectScattered(subGo.transform);
                //                Tweener tpSub = subGo.transform.DOMove(curObjSub.m_Position, m_TweenLength).SetAutoKill(false);
                //                Tweener trSub = subGo.transform.DORotate(curObjSub.m_Rotation, m_TweenLength).SetAutoKill(false);

                //                //tp.stringId = go.name;

                //                yield return tpSub.WaitForCompletion();
                //            }

                //        }
                //    }
                //    else
                //    {

                //        ObjectOrigin curObj = FindObjectScattered(go.transform);
                //        Tweener tp = go.transform.DOMove(curObj.m_Position, m_TweenLength).SetAutoKill(false);
                //        Tweener tr = go.transform.DORotate(curObj.m_Rotation, m_TweenLength).SetAutoKill(false);

                //        tp.stringId = go.name;

                //        yield return tp.WaitForCompletion();
                //    }

                //}

                //m_groupIndex = index;
                yield return null;
                //SetTextAssembling("---");
                m_IsAnimating = false;
                
            }
            
        }

        ObjectOrigin FindObjectOriginal(Transform targetObject)
        {
            foreach (ObjectOrigin t in m_OriginalTransforms)
            {
                if (t.m_Object == targetObject)
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
            foreach (GroupGameObjects go in m_GroupGameObjects)
            {
                if(go.m_GroupObject.name == name)
                {
                    return go.m_GroupObject;
                }
                else
                {
                    foreach(GameObject sgo in go.m_StepObjects)
                    {
                        if(sgo.name == name)
                        {
                            return sgo;
                        }
                    }
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

        private Vector3 GetRandomPositionInsideBox(BoxCollider targetVolume)
        {
            Vector3 pos = new Vector3(
                Random.Range(-targetVolume.size.x / 2f, targetVolume.size.x / 2f),
                Random.Range(-targetVolume.size.y / 2f, targetVolume.size.y / 2f),
                Random.Range(-targetVolume.size.z / 2f, targetVolume.size.z / 2f)
            );

            pos += targetVolume.transform.position;

            //print(pos);

            return pos;
        }

        //[Button]
        //public void ExportSetConfig()
        //{
        //    List<SetGOExport> m_SetObjectListTemp = new List<SetGOExport>();
        //    m_SetObjectListTemp.Clear();


        //    foreach (Set set in m_Sets)
        //    {
        //        SetGOExport goExp = new SetGOExport();
        //        goExp.m_SetObjects = new List<string>();
        //        foreach (GameObject go in set.m_SetObjects)
        //        {
        //            goExp.m_SetObjects.Add(go.name);
        //        }
        //        m_SetObjectListTemp.Add(goExp);
        //    }
        //    SetGOExportWrapper wrapper = new SetGOExportWrapper();
        //    wrapper.objects = m_SetObjectListTemp;

        //    string json = JsonUtility.ToJson(wrapper, true);
        //    //System.IO.File.WriteAllText(Application.dataPath + "/SetExport.json", json);
        //}

        [Button]
        public void ExportGroupsToTextFile()
        {
            int groupID = 0;
            int stepID = 0;

            string path = "Assets/Resources/GroupExport.txt";
            StreamWriter writer = new StreamWriter(path, true);

            foreach (Group g in m_Groups)
            {
                stepID = 0;
                //print($"Group{groupID}");
                writer.WriteLine($"Group{groupID}");
                foreach (GroupStep group in m_Groups[m_SetSortTemp].m_Group)
                {
                    writer.WriteLine($"--- Step{stepID}");
                    //print($"---Step{stepID}");

                    foreach (GameObject obj in group.m_GroupStepObjects)
                    {

                        //print($"------{obj.name}");
                        writer.WriteLine($"------{obj.name}");
                    }
                    stepID++;

                }
                groupID++;
            }
            writer.Close();

        }

        //[Button]
        //public void ImportSetConfig()
        //{
        //    string path = System.IO.Path.Combine(Application.dataPath, "SetExport.json");
        //    string json = System.IO.File.ReadAllText(path);


        //    SetGOExportWrapper wrapper = new SetGOExportWrapper();
        //    wrapper = JsonUtility.FromJson<SetGOExportWrapper>(json);
        //    foreach (SetGOExport goExp in wrapper.objects)
        //    {
        //        Set newSet = new Set();
        //        newSet.m_SetObjects = new List<GameObject>();
        //        foreach (string go in goExp.m_SetObjects)
        //        {
        //            newSet.m_SetObjects.Add(GetGameObjectByNameInTarget(go));
        //        }
        //        //m_SetsTemp.Add(newSet);
        //    }
        //}

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
    public class Group // A group of objects that need to be assembled. Consists of one or more steps
    {
        public List<GroupStep> m_Group;
    }

    [System.Serializable]
    public class GroupStep // A step in assembling the group. Has a description for UI output, and a list of objects in that group step.
    {
        public string m_GroupStepOutputText;
        public List<GameObject> m_GroupStepObjects;
        public bool m_PutAsideWhenAssembled = true;
    }

    [System.Serializable]
    public class GroupGameObjects
    {
        public GameObject m_GroupObject;
        public List<GameObject> m_StepObjects;
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
    public class GroupTweens
    {
        public List<StepTweens> m_Steps;
    }
    [System.Serializable]
    public class StepTweens
    {
        public List<Tweener> m_StepTweens;
    }

    [System.Serializable]
    public class CustomGroupObject
    {
        [Tooltip("Target group to which the custom object will be added to.")]
        public int m_TargetGroupIndex = 0;
        [Tooltip("Target step to which the custom object will be added to.")]
        public int m_TargetStepIndex = 0;
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
}