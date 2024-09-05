using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using PSGJ15_DCSA.Core.DependencyAgents;
using PSGJ15_DCSA.Enums;
using PSGJ15_DCSA.Interfaces;


namespace PSGJ15_DCSA.Core
{
    [CreateAssetMenu(fileName = "SceneHandler", menuName = "Project/Scene Handler", order = 2)]
    public class SceneHandler :  ScriptableObject , IGameStateOperator
    {
        public enum SceneTypes
        {
            IntroScreen,
            World,
            Other
        }
        [SerializeField] private DA_GameStates m_GameStates;

        public bool isLoading;
        private Dictionary<string, SceneTypes> m_sceneTypeDictionnary;
        private Dictionary<SceneTypes, Action> m_viewHandlers;

        private void Awake()
        {
            //SceneManager.sceneLoaded += (scene, _) => OnSceneLoaded(scene);
        }

        // Init the dictionnary that associates Scene enums
        public void InitializeSceneNameDictionnary()
        {
            m_sceneTypeDictionnary = new Dictionary<string, SceneTypes>
            {
                { "IntroScreen", SceneTypes.IntroScreen },
                { "World", SceneTypes.World },
                { "Gym", SceneTypes.Other },
            };
        }

        public void InitializeViewHandlers()
        {
            m_viewHandlers = new Dictionary<SceneTypes, Action>()
            {
                {SceneTypes.IntroScreen, InitIntro },
                {SceneTypes.World, InitWorld },
                {SceneTypes.Other, InitGym },
            };
        }

        public IEnumerator LoadSceneAsync(string sceneName, bool additive = false)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            while (asyncOperation.progress < 0.9f)
            {
                yield return null;
            }
        }

        public void OnSceneLoaded(Scene scene)
        {
            if (m_sceneTypeDictionnary.TryGetValue(scene.name, out SceneTypes view))
            {
                if(m_viewHandlers.TryGetValue(view, out var handler))
                {
                    handler?.Invoke();
                }
            }
        }

        private void InitIntro()
        {
            var m_GameStates = (DA_GameStates)REG_DependencyAgents.Instance.GetDependencyAgent(DependencyAgentType.GameState);
            m_GameStates.InvokeChangeGameState(this, GameState.Menu);
            //Debug.Log("Loading intro stuff");
        }

        private void InitWorld()
        {
            //Debug.Log("Loading world stuff");
        }

        private void InitGym()
        {
            //Debug.Log("Loading gym stuff");
            var m_GameStates = (DA_GameStates)REG_DependencyAgents.Instance.GetDependencyAgent(DependencyAgentType.GameState);
            m_GameStates.InvokeChangeGameState(this, GameState.Play);     
        }
    }
}
