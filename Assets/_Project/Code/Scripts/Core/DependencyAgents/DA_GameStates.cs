using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PSGJ15_DCSA.Enums;
using PSGJ15_DCSA.Interfaces;
using UnityEngine;

namespace PSGJ15_DCSA.Core.DependencyAgents
{
    [CreateAssetMenu(fileName = "DA_GameStates", menuName = "Project/DependencyAgents/GameStates", order = 1)]
    public class DA_GameStates : ScriptableObject
    {
        public event Action<GameState> OnGameStateChanged;
        private GameState m_CurrentGameState;

        public GameState CurrentGameState()
        {
            return m_CurrentGameState;
        }

        public void InvokeChangeGameState(IGameStateOperator invoker, GameState newState)
        {
            if(CheckUnauthorizedRequest(invoker))
            {
                #if UNITY_EDITOR
                Debug.LogError("Unauthorized access attempt to change game state. ", invoker as MonoBehaviour);
                #endif
                return;
            }

            m_CurrentGameState = newState;
            OnGameStateChanged?.Invoke(newState);
            #if UNITY_EDITOR
            Debug.Log($"Game State changed to {newState} by {invoker.GetType().Name}", invoker as MonoBehaviour);
            #endif
        }

        private bool CheckUnauthorizedRequest(IGameStateOperator invoker)
        {
            return invoker == null;
        }

        private void OnDisable()
        {
            #if UNITY_EDITOR
            CheckAndLogSubscribers(OnGameStateChanged, nameof(OnGameStateChanged));
            #endif

            OnGameStateChanged = null;
        }

        #if UNITY_EDITOR
        private void CheckAndLogSubscribers(Delegate eventDelegate, string eventName)
        {
            if(eventDelegate != null)
            {
                var subscribers = eventDelegate.GetInvocationList();
                var subscriberName = subscribers.Select(d => d.Method.ToString()).ToArray();
                Debug.LogWarning($"[ScriptableObject] '{eventName}' still has subscribers. Subscribers: {string.Join(", ", subscriberName)}. As a safeguard, it is being set to null. Ensure all event listeners are unsubscribed properly to prevent memory leaks and unexpected behavior.");
            }
        }
        #endif
    }
}
