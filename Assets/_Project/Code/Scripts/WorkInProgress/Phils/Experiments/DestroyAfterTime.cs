using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    private float timeSpawn;
    private void Update()
    {
        timeSpawn += Time.deltaTime;

        if (timeSpawn >= 4f)
        {
            Destroy(gameObject);
        }
    }
}
