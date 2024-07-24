using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSGJ15_DCSA.Gameplay;

namespace PSGJ15_DCSA.Core.DependencyAgents
{
    [CreateAssetMenu(fileName = "DA_CharacterEvents", menuName = "Project/DependencyAgents/CharacterEvents", order = 1)]
    public class DA_CharacterEvents : ScriptableObject
    {
        public event Action<Health, GameObject, STAT_Character> OnCharacterGettingHit;

        public void InvokeCharacterGettingHit(Health script, GameObject character, STAT_Character stats)
        {
            Debug.Log(character.name + " is being hit! ");
        }
    }
}
