using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class InteractiveTutorialController : MonoBehaviour
{
    public int m_CurrentActiveIndex = 0;
    public InteractableObject m_CurrentActiveObject;

    public List<InteractableObject> m_ObjectListOrdered = new List<InteractableObject>();
    public List<SocketObject> m_AllSockets = new List<SocketObject>();

    public IndicationArrow m_IndicationArrow;

    public TextMeshProUGUI m_InstructionText;


    public Transform m_TargetObject;
    public List<string> m_ConfigList = new List<string>();

    public static InteractiveTutorialController i;

    private void Awake()
    {
        i = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        DisableAllSockets();
        GetSocketForObject(m_ObjectListOrdered[m_CurrentActiveIndex]).ToggleSocketActiveState(true);
        SetInstructionText_PickUp(m_ObjectListOrdered[m_CurrentActiveIndex].gameObject.name);
        IndicationArrow.i.Move(m_ObjectListOrdered[m_CurrentActiveIndex].transform);
        //IndicationArrow.i.Move(m_ObjectListOrdered[m_CurrentActiveIndex].transform);
        //m_ObjectListOrdered[m_CurrentActiveIndex].SetAsActive();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void DisableAllSockets()
    {
        //print("disabling");
        foreach (SocketObject s in m_AllSockets)
        {
            s.ToggleSocketActiveState(false);
        }
    }

    [Button]
    public void GetTargetObjects()
    {
        m_ObjectListOrdered.Clear();
        m_AllSockets.Clear();

        // Get interactable objects
        foreach (Transform t in m_TargetObject)
        {
            if (t.gameObject.name.Contains("Sockets") == false)
            {
                m_ObjectListOrdered.Add(t.gameObject.GetComponent<InteractableObject>());
            }
        }

        // Get sockets
        Transform socketRoot = m_TargetObject.Find("Sockets");
        foreach (Transform t in socketRoot)
        {
            m_AllSockets.Add(t.GetComponent<SocketObject>());
        }
    }

    [Button]
    public void OrderObjectListFromConfigFile()
    {
        string filename = Application.dataPath + "/Resources/ObjectOrder.txt";
        m_ConfigList = File.ReadAllLines(filename).ToList();

        List<InteractableObject> tempObjectList = new List<InteractableObject>();
        tempObjectList = m_ObjectListOrdered;

        InteractableObject[] tempSortList = new InteractableObject[m_ConfigList.Count];

        foreach (InteractableObject go in tempObjectList)
        {
            foreach (string objName in m_ConfigList)
            {
                if (go.gameObject.name == objName)
                {
                    int indexConfig = m_ConfigList.IndexOf(objName);
                    tempSortList[indexConfig] = go;
                    continue;
                }
            }
        }

        m_ObjectListOrdered = tempSortList.ToList();
    }

    public void ActivateNextSocket()
    {
        GetSocketForObject(m_ObjectListOrdered[m_CurrentActiveIndex]).ToggleSocketActiveState(false);
        m_CurrentActiveIndex += 1;
        GetSocketForObject(m_ObjectListOrdered[m_CurrentActiveIndex]).ToggleSocketActiveState(true);
        SetInstructionText_PickUp(m_ObjectListOrdered[m_CurrentActiveIndex].gameObject.name);
        IndicationArrow.i.Move(m_ObjectListOrdered[m_CurrentActiveIndex].transform);
    }

    public void SetInstructionText_PickUp(string text)
    {
        m_InstructionText.text = $"Paņem {text} detaļu no galda.";
    }

    public void SetInstructionText_Place(string text)
    {
        m_InstructionText.text = $"Novieto {text} detaļu atzīmētajā vietā.";
    }

    //public void ActivateNextSocket(XRBaseInteractable interactable)
    //{
    //GetSocketForObject(m_ObjectListOrdered[m_CurrentActiveIndex]).m_SocketInteractor.socketActive = false;

    //m_CurrentActiveIndex += 1;
    //GetSocketForObject(m_ObjectListOrdered[m_CurrentActiveIndex]).gameObject.SetActive(true);
    //m_ObjectListOrdered[m_CurrentActiveIndex].SetAsActive();
    //GetSocketForObject(m_ObjectListOrdered[m_CurrentActiveIndex]).m_SocketInteractor.socketActive = true;
    //IndicationArrow.i.Move(m_ObjectListOrdered[m_CurrentActiveIndex].transform);
    //}

    private SocketObject GetSocketForObject(InteractableObject obj)
    {
        foreach (SocketObject s in m_AllSockets)
        {
            if (s.m_ExpectedObject == obj)
            {
                return s;
            }
        }
        return null;
    }

}
