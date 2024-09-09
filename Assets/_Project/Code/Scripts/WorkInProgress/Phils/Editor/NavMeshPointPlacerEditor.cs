using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class NavMeshPointPlacerEditor : EditorWindow
{
    private List<GameObject> pointObjects = new List<GameObject>();
    private NavMeshPointHolder targetObject;
    private bool isPlacingPoint = false;
    private GameObject currentPoint;

    [MenuItem("Tools/NavMesh Point Placer")]
    public static void ShowWindow()
    {
        GetWindow<NavMeshPointPlacerEditor>("NavMesh Point Placer");
    }

    private void OnGUI()
    {
        GUILayout.Label("NavMesh Point Placer", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox("Select a GameObject with the NavMeshPointHolder script attached.", MessageType.Info);

        targetObject = (NavMeshPointHolder)EditorGUILayout.ObjectField("Target Object", targetObject, typeof(NavMeshPointHolder), true);

        EditorGUI.BeginDisabledGroup(targetObject == null);

        if (GUILayout.Button("Create Point"))
        {
            isPlacingPoint = true;
            CreatePoint();
        }

        if (GUILayout.Button("Clear Points"))
        {
            ClearPoints();
        }

        if (GUILayout.Button("Save Points"))
        {
            SavePoints();
        }

        EditorGUI.EndDisabledGroup();

        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void CreatePoint()
    {
        currentPoint = new GameObject("NavMeshPoint_" + pointObjects.Count);
        currentPoint.transform.position = SceneView.lastActiveSceneView.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
        pointObjects.Add(currentPoint);
        Selection.activeGameObject = currentPoint;
        SceneView.lastActiveSceneView.FrameSelected();
    }

    private void ClearPoints()
    {
        foreach (var point in pointObjects)
        {
            DestroyImmediate(point);
        }
        pointObjects.Clear();
        isPlacingPoint = false;
        
        if (targetObject != null)
        {
            targetObject.ClearPoints();
            EditorUtility.SetDirty(targetObject);
        }
    }

    private void SavePoints()
    {
        if (targetObject != null)
        {
            List<Vector3> points = new List<Vector3>();
            foreach (var pointObject in pointObjects)
            {
                points.Add(pointObject.transform.position);
            }
            targetObject.SetPoints(points);
            EditorUtility.SetDirty(targetObject);
            Debug.Log("Points saved to target object.");
        }
        else
        {
            Debug.LogWarning("No target object selected.");
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (isPlacingPoint && currentPoint != null)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                currentPoint.transform.position = hit.point;
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                isPlacingPoint = false;
                currentPoint = null;
                Event.current.Use();
            }

            sceneView.Repaint();
        }

        foreach (var point in pointObjects)
        {
            Handles.color = Color.green;
            Handles.DrawSolidDisc(point.transform.position, Vector3.up, 0.1f);
            Handles.Label(point.transform.position, point.name);
        }
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
}
