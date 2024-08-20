using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoxCollider))]
public class BoxColliderFitter : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BoxCollider boxCollider = (BoxCollider)target;

        if (GUILayout.Button("Fit to Mesh"))
        {
            if (boxCollider.gameObject.GetComponent<Renderer>() != null)
            {
                Renderer renderer = boxCollider.gameObject.GetComponent<Renderer>();
                boxCollider.center = renderer.bounds.center - boxCollider.transform.position;
                boxCollider.size = renderer.bounds.size;
            }
            else
            {
                Debug.LogWarning("No Renderer found on the object. Box Collider cannot be adjusted.");
            }
        }
    }
}