#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Frames.Editors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Stack))]
    public class StackBaseEditor : Editor
    {
        private Stack stack;

        private SerializedProperty direction;
        private SerializedProperty distribution;
        private SerializedProperty alignment;
        private SerializedProperty padding;
        private SerializedProperty spacing;

        private ReorderableList content;

        public void OnEnable()
        {
            stack = (Stack)target;

            direction = serializedObject.FindProperty("direction");
            distribution = serializedObject.FindProperty("distribution");
            alignment = serializedObject.FindProperty("alignment");
            padding = serializedObject.FindProperty("padding");
            spacing = serializedObject.FindProperty("spacing");

            content = new ReorderableList(serializedObject, serializedObject.FindProperty("contents"), true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Contents", EditorStyles.boldLabel);
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty element = content.serializedProperty.GetArrayElementAtIndex(index);
                    EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
                },
                onChangedCallback = content =>
                {
                    stack.ForceStack();
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            direction.enumValueIndex = (int)(StackDirection)EditorGUILayout.EnumPopup("Direction", (StackDirection)direction.enumValueIndex);

            distribution.enumValueIndex = (int)(StackDistribution)EditorGUILayout.EnumPopup("Distribution", (StackDistribution)distribution.enumValueIndex);

            alignment.enumValueIndex = (int)(StackAlignment)EditorGUILayout.EnumPopup("Alignment", (StackAlignment)alignment.enumValueIndex);

            EditorGUILayout.Space();

            if (padding.arraySize != 2)
            {
                padding.arraySize = 2;
            }
            padding.GetArrayElementAtIndex(0).vector2Value = EditorGUILayout.Vector2Field("Padding Min", padding.GetArrayElementAtIndex(0).vector2Value);
            padding.GetArrayElementAtIndex(1).vector2Value = EditorGUILayout.Vector2Field("Padding Max", padding.GetArrayElementAtIndex(1).vector2Value);

            EditorGUILayout.Space();

            //Enables spacing for distributions without auto-spacing
            switch ((StackDistribution)distribution.enumValueIndex)
            {
                case StackDistribution.Start:
                case StackDistribution.End:
                case StackDistribution.Center:
                    spacing.floatValue = EditorGUILayout.FloatField("Spacing", spacing.floatValue);
                    break;
            }

            content.DoLayoutList();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(stack);
                stack.ForceStack();
            }

            //Fixes stuff when stuff goes wrong.  Shouldn't need to be used *too* often (hopefully)
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Restack"))
            {
                if ((StackDirection)direction.enumValueIndex == StackDirection.Horizontal)
                {
                    stack.stackInstance = new HorizontalStack(stack.rectTransform, stack.contents, stack.distribution, stack.alignment, stack.spacing, stack.padding);
                }
                else
                {
                    stack.stackInstance = new VerticalStack(stack.rectTransform, stack.contents, stack.distribution, stack.alignment, stack.spacing, stack.padding);
                }
                stack.ForceStack();
            }
            if (GUILayout.Button("Reset Children"))
            {
                stack.ResetChildren();
            }
            GUILayout.EndHorizontal();
        }
    }
}
#endif