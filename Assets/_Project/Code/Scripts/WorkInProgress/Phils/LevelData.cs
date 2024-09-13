using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSGJ15_DCSA.Core
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObject/LevelData")]

    public class LevelData : ScriptableObject
    {
        [SerializeField] private GameObject m_playerPrefab;
        [SerializeField] private GameObject m_enemyPrefab;
        [SerializeField] private GameObject m_HUD;

        public GameObject PlayerPrefab { get => m_playerPrefab; private set => m_playerPrefab = value; }
        public GameObject EnemyPrefab { get => m_enemyPrefab; private set => m_enemyPrefab = value; }
        public GameObject HUD { get => m_HUD; private set => m_HUD = value; }
    }
}
