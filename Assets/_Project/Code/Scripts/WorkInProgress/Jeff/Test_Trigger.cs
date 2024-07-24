using System.Collections;
using System.Collections.Generic;
using PSGJ15_DCSA.Enums;
using PSGJ15_DCSA.Gameplay;
using UnityEngine;

public class Test_Trigger : MonoBehaviour
{
    [SerializeField] private int m_damage;
    [SerializeField] private ElementTypes m_element;

    private void OnTriggerEnter(Collider collider)
    {   
        Debug.Log(collider + " HAS ENTERED MY TRIGGER!", collider.gameObject);
        if(collider.TryGetComponent(out Health damageableCharacter))
        {
            Debug.Log("TAG IS RIGHT!");
            damageableCharacter.ReceiveDamage(m_damage, m_element);
        }
    }
}
