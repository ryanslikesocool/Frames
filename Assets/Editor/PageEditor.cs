using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;

namespace Framer
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
        private SerializedProperty timeTakenDuringAnimation;
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
            timeTakenDuringAnimation = serializedObject.FindProperty("timeTakenDuringAnimation");
            spacing = serializedObject.FindProperty("spacing");
            padding = serializedObject.FindProperty("padding");

            content = new ReorderableList(serializedObject, serializedObject.FindProperty("contents"), true, true, true, true);
            content.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Contents", EditorStyles.boldLabel);
            };
            content.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = content.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

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
                else
                {
                    Debug.LogWarning("Page index was too small to decrease");
                }
            }
            if (GUILayout.Button("+"))
            {
                if (targetIndex.intValue < page.contents.Count - 1)
                {
                    targetIndex.intValue++;
                }
                else
                {
                    Debug.LogWarning("Page index was too large to increase");
                }
            }
            EditorGUILayout.EndHorizontal();

            timeTakenDuringAnimation.floatValue = EditorGUILayout.FloatField("Animation Length", timeTakenDuringAnimation.floatValue);

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

            //Disgusting removal when clicking the minus button on the reorderable list
            List<RectTransform> toRemove = new List<RectTransform>();
            foreach (RectTransform rt in page.contents)
            {
                if (rt == null)
                {
                    toRemove.Add(rt);
                }
            }
            foreach (RectTransform rt in toRemove)
            {
                page.contents.Remove(rt);
            }

            //Checks if the page order has changed
            if (content != new ReorderableList(serializedObject, serializedObject.FindProperty("contents"), true, true, true, true))
            {
                if (!EditorApplication.isPlaying)
                {
                    page.LineUp();
                    page.SetPage(currentIndex.intValue, targetIndex.intValue);
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Force Line Up Contents"))
            {
                page.LineUp();
            }
            if (GUILayout.Button("Force Reset Children"))
            {
                page.ResetChildren();
            }
            EditorGUILayout.EndHorizontal();

            if (GUI.changed && !EditorApplication.isPlaying)
            {
                serializedObject.ApplyModifiedProperties();
                page.ChangeDirection();
                page.ChangeTransitionType();
                page.LineUp();
                page.SetPage(currentIndex.intValue, targetIndex.intValue);
                EditorUtility.SetDirty(page);
            }
        }
    }
}