#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FramesV2.Editors
{
    [CustomEditor(typeof(Frame))]
    //[CanEditMultipleObjects]
    public class FrameEditor : Editor
    {
        //SerializedProperty value;

        public void OnEnable()
        {
            //value = serializedObject.FindProperty("Value");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // EditorGUI content

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif