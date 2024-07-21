using System.Collections;
using System.Collections.Generic;
using PSGJ15_DCSA.Core.DependencyAgents;
using PSGJ15_DCSA.Enums;
using PSGJ15_DCSA.Interfaces;
using UnityEngine;

public class GymManager : MonoBehaviour, IGameStateOperator
{
    [SerializeField] private DA_GameStates m_GameStates;

    private void Start()
    {
        m_GameStates = (DA_GameStates)REG_DependencyAgents.Instance.GetDependencyAgent(DependencyAgentType.GameState);
        m_GameStates.InvokeChangeGameState(this, GameState.Play);
    }
}
