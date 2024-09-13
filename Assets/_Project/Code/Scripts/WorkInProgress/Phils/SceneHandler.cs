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
using Unity.Mathematics;


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
        [SerializeField] private LevelData m_levelData;

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
                { "Empty", SceneTypes.World },
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

        public void InitSceneHandling()
        {
            isLoading = false;
            SceneManager.sceneLoaded += (scene, _) => OnSceneLoaded(scene);
            InitializeSceneNameDictionnary();
            InitializeViewHandlers();
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
        }

        private void InitWorld()
        {
            var m_GameStates = (DA_GameStates)REG_DependencyAgents.Instance.GetDependencyAgent(DependencyAgentType.GameState);
            m_GameStates.InvokeChangeGameState(this, GameState.Play);
        }

        private void InitGym()
        {
            var spawnPoint = LevelInfo.Instance.GetSpawnPoint(0);
            GameManager.Instance.ReferenceToPlayer = Instantiate(LevelInfo.Instance.levelData.PlayerPrefab, spawnPoint, quaternion.identity);

            GameManager.Instance.HUD_Object = Instantiate(LevelInfo.Instance.levelData.HUD);
            GameManager.Instance.HUD_Component = GameManager.Instance.HUD_Object.GetComponent<HUD>();

            var m_GameStates = (DA_GameStates)REG_DependencyAgents.Instance.GetDependencyAgent(DependencyAgentType.GameState);
            m_GameStates.InvokeChangeGameState(this, GameState.Play);
        

            
            // spawn the ennemies ? or maybe the ennemy spawning mechanism
        }

        private void CleanUp()
        {
            // remove reference to the Player in the gamemanager
        }
    }
}
