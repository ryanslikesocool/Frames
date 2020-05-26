using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;

namespace ifelse.Frames
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Page))]
    public class PageEditor : Editor
    {
        private Page page;
        private ReorderableList content;

        private SerializedProperty direction;
        private SerializedProperty alignment;
        private SerializedProperty transition;
        private SerializedProperty targetIndex;
        private SerializedProperty currentIndex;
        private SerializedProperty animationDuration;
        private SerializedProperty spacing;
        private SerializedProperty padding;

        void OnEnable()
        {
            page = (Page)target;

            direction = serializedObject.FindProperty("direction");
            alignment = serializedObject.FindProperty("alignment");
            transition = serializedObject.FindProperty("transition");
            targetIndex = serializedObject.FindProperty("targetIndex");
            currentIndex = serializedObject.FindProperty("currentIndex");
            animationDuration = serializedObject.FindProperty("animationDuration");
            spacing = serializedObject.FindProperty("spacing");
            padding = serializedObject.FindProperty("padding");

            content = new ReorderableList(serializedObject, serializedObject.FindProperty("contents"), true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Contents", EditorStyles.boldLabel);
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty element = content.serializedProperty.GetArrayElementAtIndex(index);
                    EditorGUI.ObjectField(new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
                },
                onChangedCallback = content =>
                {
                    page.LineUp();
                },
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            direction.enumValueIndex = (int)(PageDirection)EditorGUILayout.EnumPopup("Direction", (PageDirection)direction.enumValueIndex);
            alignment.enumValueIndex = (int)(PageAlignment)EditorGUILayout.EnumPopup("Alignment", (PageAlignment)alignment.enumValueIndex);
            transition.enumValueIndex = (int)(PageTransition)EditorGUILayout.EnumPopup("Transition", (PageTransition)transition.enumValueIndex);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            targetIndex.intValue = EditorGUILayout.IntField("Target Index", targetIndex.intValue);
            if (targetIndex.intValue < 0 && targetIndex.intValue < page.contents.Count)
            {
                targetIndex.intValue = 0;
            }
            else if (targetIndex.intValue >= page.contents.Count)
            {
                targetIndex.intValue = page.contents.Count - 1;
            }
            if (GUILayout.Button("-"))
            {
                if (targetIndex.intValue > 0)
                {
                    targetIndex.intValue--;
                }
            }
            if (GUILayout.Button("+"))
            {
                if (targetIndex.intValue < page.contents.Count - 1)
                {
                    targetIndex.intValue++;
                }
            }
            EditorGUILayout.EndHorizontal();

            animationDuration.floatValue = EditorGUILayout.FloatField("Animation Length", animationDuration.floatValue);

            EditorGUILayout.Space();

            spacing.floatValue = EditorGUILayout.FloatField("Spacing", spacing.floatValue);

            EditorGUILayout.Space();

            if (padding.arraySize != 2)
            {
                padding.arraySize = 2;
            }
            padding.GetArrayElementAtIndex(0).vector2Value = EditorGUILayout.Vector2Field("Padding Min", padding.GetArrayElementAtIndex(0).vector2Value);
            padding.GetArrayElementAtIndex(1).vector2Value = EditorGUILayout.Vector2Field("Padding Max", padding.GetArrayElementAtIndex(1).vector2Value);

            EditorGUILayout.Space();

            content.DoLayoutList();

            if (EditorGUI.EndChangeCheck())
            {
                page.ChangeDirection();
                page.ChangeTransitionType();
                page.SetPage(currentIndex.intValue, targetIndex.intValue);
                page.LineUp();
                EditorUtility.SetDirty(page);
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Line Up Contents"))
            {
                page.LineUp();
            }
            if (GUILayout.Button("Reset Children"))
            {
                page.ResetChildren();
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}