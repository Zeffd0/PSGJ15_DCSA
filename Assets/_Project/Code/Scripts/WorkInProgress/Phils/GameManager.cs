using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PSGJ15_DCSA.Core.DependencyAgents;

using UnityEditor;
using System.IO;

namespace PSGJ15_DCSA.Core
{
    public class GameManager : IndestructibleSingletonBehaviour<GameManager>
    {
        [SerializeField] private SceneHandler m_sceneHandler;
        // [SerializeField] private DA_GameStates m_GameStates;

        // public DA_GameStates GameStates { get => m_GameStates; set => m_GameStates = value; }

        protected override void OnSingletonAwake()
        {
            initSceneHandling();
            SetResolution();
        }

        #region Scene Handling

        private void initSceneHandling()
        {
            m_sceneHandler.isLoading = false;
            SceneManager.sceneLoaded += (scene, _) => m_sceneHandler.OnSceneLoaded(scene);
            m_sceneHandler.InitializeSceneNameDictionnary();
            m_sceneHandler.InitializeViewHandlers();
        }

        public void LoadScene(string sceneName)
        {
            if (!m_sceneHandler.isLoading)
            {
                m_sceneHandler.isLoading = true;
                StartCoroutine(m_sceneHandler.LoadSceneAsync(sceneName));
            }
        }

        #endregion

        #region Screen Init

        void SetResolution()
        {
            // TODO: Detect platform (PC/WebGL) and set resolution accordingly

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // WebGL - set resolution based on screen size
                int screenWidth = Screen.width;
                int screenHeight = Screen.height;
                Screen.SetResolution(screenWidth, screenHeight, FullScreenMode.Windowed);
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer)
            {
                // PC build - set to 1920x1080
                Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
            }

            // TODO: Add more platform-specific resolution settings if needed
        }
        #endregion
    }
}
