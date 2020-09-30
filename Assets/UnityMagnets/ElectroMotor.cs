using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ElectroMotor : MonoBehaviour
{
    public List<ElectroMagnet> m_ElectroMagnets = new List<ElectroMagnet>();
    public float m_MagnetSwitchTimer = 1f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AnimatePolarityChanges());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator AnimatePolarityChanges(){

        foreach(ElectroMagnet em in m_ElectroMagnets){
            em.ChangeChildMagnetPolarity();    
            yield return null;
        }

        yield return new WaitForSeconds(m_MagnetSwitchTimer);
        StartCoroutine(AnimatePolarityChanges());

    }
}
