using UnityEngine;
using UnityEditor;

public class ArrangeInGrid : EditorWindow
{
    private int rowLength = 5;
    private float spacing = 1.0f;

    [MenuItem("Tools/Arrange Selected in Grid")]
    static void Init()
    {
        ArrangeInGrid window = (ArrangeInGrid)EditorWindow.GetWindow(typeof(ArrangeInGrid));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Grid Settings", EditorStyles.boldLabel);
        rowLength = EditorGUILayout.IntField("Row Length", rowLength);
        spacing = EditorGUILayout.FloatField("Spacing", spacing);

        if (GUILayout.Button("Arrange"))
        {
            ArrangeObjects();
        }
        
        // New button for renaming GameObjects based on material name
        if (GUILayout.Button("Rename to Material Name"))
        {
            RenameToMaterialName();
        }
    }

    void ArrangeObjects()
    {
        if (Selection.transforms.Length == 0)
        {
            Debug.LogWarning("No GameObjects selected. Please select at least one GameObject.");
            return;
        }

        Vector3 centerPoint = GetCenterPoint();
        Vector3 startingPoint = CalculateStartingPoint(centerPoint);

        Undo.RecordObjects(Selection.transforms, "Arrange in Grid");

        for (int i = 0, row = 0, col = 0; i < Selection.transforms.Length; i++, col++)
        {
            if (col >= rowLength)
            {
                col = 0;
                row++;
            }

            var selectedTransform = Selection.transforms[i];
            Vector3 newPosition = startingPoint + new Vector3(col * spacing, 0, row * spacing);
            selectedTransform.position = newPosition;
        }
    }

    Vector3 GetCenterPoint()
    {
        var bounds = new Bounds(Selection.transforms[0].position, Vector3.zero);
        foreach (var transform in Selection.transforms)
        {
            bounds.Encapsulate(transform.position);
        }
        return bounds.center;
    }

    Vector3 CalculateStartingPoint(Vector3 centerPoint)
    {
        // Calculate offset based on the number of items and spacing
        float offsetX = ((rowLength - 1) * spacing) / 2;
        float offsetZ = ((Mathf.CeilToInt(Selection.transforms.Length / (float)rowLength) - 1) * spacing) / 2;

        return new Vector3(centerPoint.x - offsetX, centerPoint.y, centerPoint.z - offsetZ);
    }

    // New method for renaming GameObjects
    void RenameToMaterialName()
    {
        foreach (var transform in Selection.transforms)
        {
            Renderer renderer = transform.GetComponent<Renderer>();
            if (renderer != null && renderer.sharedMaterial != null)
            {
                Undo.RecordObject(transform.gameObject, "Rename to Material Name");
                transform.gameObject.name = renderer.sharedMaterial.name;
            }
            else
            {
                Debug.LogWarning($"GameObject '{transform.gameObject.name}' does not have a Renderer with a Material.");
            }
        }
    }
}
