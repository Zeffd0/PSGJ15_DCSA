using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSGJ15_DCSA.Core.DependencyAgents;
using PSGJ15_DCSA.Enums;
using PSGJ15_DCSA.Interfaces;


namespace PSGJ15_DCSA.Core
{
    public class LevelInfo : SingletonBehaviour<LevelInfo> , IGameStateOperator
    {
        [SerializeField] private List<Vector3> spawnPoints = new List<Vector3>();
        public LevelData levelData;
        protected override void OnSingletonAwake()
        {
            base.OnSingletonAwake();
        }

        public Vector3 GetSpawnPoint(int index)
        {
            if (spawnPoints.Count == 0)
            {
                Debug.LogWarning("No spawn points defined in LevelInfo!");
                return Vector3.zero;
            }
            return spawnPoints[index % spawnPoints.Count];
        }

        public Vector3 GetSpawnPointEnnemies(int index)
        {
            if (spawnPoints.Count == 0)
            {
                Debug.LogWarning("No spawn points defined in LevelInfo!");
                return Vector3.zero;
            }
            return spawnPoints[index % spawnPoints.Count];
        }

        protected override void OnSingletonDestroy()
        {
            base.OnSingletonDestroy();
        }
    }
}