using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cubePrefab;  // Reference to the cube prefab
    public float spawnInterval = 1f;  // Time interval between spawns
    public Vector3 spawnPositionOffset = new Vector3(0, 5, 0);  // Offset for spawn position

    private float timeSinceLastSpawn;

    void Start()
    {
        // If no prefab is set, create a primitive cube
        if (cubePrefab == null)
        {
            cubePrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubePrefab.AddComponent<Rigidbody>(); // Add Rigidbody to enable physics
            cubePrefab.SetActive(false); // Make sure the prefab is not visible
        }
    }

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnCube();
            timeSinceLastSpawn = 0f;
        }
    }

    void SpawnCube()
    {
        // Instantiate the cube at the spawner's position plus the offset
        GameObject cube = Instantiate(cubePrefab, transform.position + spawnPositionOffset, Quaternion.identity);
        cube.SetActive(true); // Make sure the cube is active
    }
}
