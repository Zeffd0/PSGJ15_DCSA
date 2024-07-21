using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PSGJ15_DCSA.Core
{
    public class GameManager : IndestructibleSingletonBehaviour<GameManager>
    {
        [SerializeField] private SceneHandler m_sceneHandler;

        protected override void OnSingletonAwake()
        {
            initSceneHandling();
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
    }
}
