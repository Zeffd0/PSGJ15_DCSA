using UnityEngine;
using System.Collections.Generic;

public class NavMeshPointHolder : MonoBehaviour
{
    [SerializeField]
    private List<Vector3> points = new List<Vector3>();

    // Static reference to the instance
    private static NavMeshPointHolder instance;

    private void Awake()
    {
        // Ensure there's only one instance
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogWarning("Multiple NavMeshPointHolder instances detected. Only one will be used.");
            Destroy(gameObject);
        }
    }

    public void SetPoints(List<Vector3> newPoints)
    {
        points = new List<Vector3>(newPoints);
    }

    public List<Vector3> GetPoints()
    {
        return points;
    }

    public void ClearPoints()
    {
        points.Clear();
    }

    // Static getter for the points
    public static List<Vector3> Points
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("No NavMeshPointHolder instance found in the scene.");
                return new List<Vector3>();
            }
            return instance.points;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawSphere(points[i], 0.1f);
            if (i < points.Count - 1)
            {
                Gizmos.DrawLine(points[i], points[i + 1]);
            }
        }
    }
}