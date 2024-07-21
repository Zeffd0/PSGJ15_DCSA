using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

namespace PSGJ15_DCSA.Core
{
    [CreateAssetMenu(fileName = "SceneHandler", menuName = "Project/Scene Handler", order = 2)]
    public class SceneHandler :  ScriptableObject
    {
        public enum SceneTypes
        {
            IntroScreen,
            World,
            Other
        }

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
            Debug.Log("do I ever even come in here wtf?");
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
            Debug.Log("Loading intro stuff");
        }

        private void InitWorld()
        {
            Debug.Log("Loading world stuff");
        }

         private void InitGym()
        {
            Debug.Log("Loading gym stuff");
        }
    }
}
