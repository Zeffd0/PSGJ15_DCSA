using UnityEngine;
using UnityEditor;

namespace PSGJ15_DCSA.Editor
{
    [CustomEditor(typeof(HealthComponent))]
    public class HealthComponentEditor : UnityEditor.Editor
    {
        SerializedProperty m_meshRendererProp;
        SerializedProperty m_audioSourceProp;
        SerializedProperty m_deathAnimationProp;

        private void OnEnable()
        {
            m_meshRendererProp = serializedObject.FindProperty("m_meshRenderer");
            m_audioSourceProp = serializedObject.FindProperty("m_audioSource");
            m_deathAnimationProp = serializedObject.FindProperty("m_deathAnimation");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawDefaultInspector();

            if (GUILayout.Button("Setup Destructible Piece"))
            {
                SetupDestructiblePiece();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void SetupDestructiblePiece()
        {
            HealthComponent healthComponent = (HealthComponent)target;
            GameObject obj = healthComponent.gameObject;

            Undo.RecordObject(obj, "Setup Destructible Piece");

            // Add MeshRenderer if not present
            if (obj.GetComponent<MeshRenderer>() == null)
                obj.AddComponent<MeshRenderer>();

            // Add MeshFilter if not present
            if (obj.GetComponent<MeshFilter>() == null)
                obj.AddComponent<MeshFilter>();

            // Add AudioSource if not present
            if (obj.GetComponent<AudioSource>() == null)
                obj.AddComponent<AudioSource>();

            // Add BoxCollider if not present
            BoxCollider boxCollider = obj.GetComponent<BoxCollider>();
            if (boxCollider == null)
                boxCollider = obj.AddComponent<BoxCollider>();

            // Fit the BoxCollider to the mesh
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                boxCollider.center = renderer.bounds.center - boxCollider.transform.position;
                boxCollider.size = renderer.bounds.size;
            }
            else
            {
                Debug.LogWarning("No Renderer found on the object. Box Collider cannot be adjusted.");
            }

            // Add or setup Rigidbody
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
            if (rigidbody == null)
                rigidbody = obj.AddComponent<Rigidbody>();

            // Assign components to HealthComponent using SerializedProperty
            m_meshRendererProp.objectReferenceValue = obj.GetComponent<MeshRenderer>();
            m_audioSourceProp.objectReferenceValue = obj.GetComponent<AudioSource>();
            m_deathAnimationProp.objectReferenceValue = obj.GetComponent<Animator>(); // Add Animator if needed

            serializedObject.ApplyModifiedProperties();
        }
    }
}