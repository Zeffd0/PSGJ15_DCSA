using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using PSGJ15_DCSA.Interfaces;
using UnityEngine;
using PSGJ15_DCSA.Enums;

[CreateAssetMenu(fileName = "STAT_Character", menuName = "Project/STAT_Character")]
public class STAT_Character : ScriptableObject, ICharacterStats
{
    [Header("Base Starting Stats")]
    [SerializeField] private float m_baseMovementSpeed;
    [SerializeField] private int m_baseMaxHealth;
    [SerializeField] private int m_baseDamage;

    [Header("Current Modifiable Stats")]
    [SerializeField] private float  m_currentMovementSpeed;
    [SerializeField] private int m_currentMaxHealth;
    [SerializeField] private int m_currenthealth;
    [SerializeField] private int m_currentDamage;

    [Header("Attributes")]
    [SerializeField] private ElementTypes m_elementalAffinity;

    public void Initialize()
    {
        ResetAllValues();
    }
    public int GetMaxHealthValue()
    {
        return m_currentMaxHealth;
    }
    public void SetMaxHealthValue(int value)
    {
        m_currentMaxHealth = value;
    }
    public int GetHealthValue()
    {
        return m_currenthealth;
    }
    public void SetHealthValue(int value)
    {
        m_currenthealth = value;
    }
    public float GetMovementSpeedValue()
    {
        return m_currentMovementSpeed;
    }
    public void SetMovementSpeedValue(float value)
    {
        m_currentMovementSpeed = value;
    }
    public int GetDamageValue()
    {
        return m_currentDamage;
    }
    public void SetDamageValue(int value)
    {
        m_currentDamage = value;
    }
    public ElementTypes GetElementAffinity()
    {
        return m_elementalAffinity;
    }
    public void SetElementAffinity(ElementTypes element)
    {
        m_elementalAffinity = element;
    }
    public void ResetAllValues()
    {
        m_currentMovementSpeed = m_baseMovementSpeed;
        m_currentMaxHealth = m_baseMaxHealth;
        m_currenthealth = m_baseMaxHealth;
        m_currentDamage = m_baseDamage;
    }
}
