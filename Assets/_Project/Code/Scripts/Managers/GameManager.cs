using UnityEngine;
using PSGJ15_DCSA.Interfaces;

namespace PSGJ15_DCSA.Core
{
    public class GameManager : IndestructibleSingletonBehaviour<GameManager>, IGameStateOperator
    {
        [SerializeField] private SceneHandler m_sceneHandler;
        public GameObject ReferenceToPlayer { get; set; }
        public GameObject HUD_Object { get; set; }
        public HUD HUD_Component {get; set; }

        protected override void OnSingletonAwake()
        {
            m_sceneHandler.InitSceneHandling();
        }

        public void LoadScene(string sceneName)
        {
            if (!m_sceneHandler.isLoading)
            {
                m_sceneHandler.isLoading = true;
                StartCoroutine(m_sceneHandler.LoadSceneAsync(sceneName));
            }
        }
    }
}