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

        //Basic editor stuff
        void OnEnable()
        {
            page = (Page)target;

            content = new ReorderableList(serializedObject, serializedObject.FindProperty("contents"), true, true, true, true);
            content.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Contents", EditorStyles.boldLabel);
            };
            content.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = content.serializedProperty.GetArrayElementAtIndex(index);
                    EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
                };
        }

        public override void OnInspectorGUI()
        {
            PageDirection direction = (PageDirection)EditorGUILayout.EnumPopup("Direction", page.direction);
            if (page.direction != direction)
            {
                page.direction = direction;
                page.ChangeDirection();
            }

            PageAlignment alignment = (PageAlignment)EditorGUILayout.EnumPopup("Distribution", page.alignment);
            if (page.alignment != alignment)
            {
                page.alignment = alignment;
            }

            PageTransition transition = (PageTransition)EditorGUILayout.EnumPopup("Transition", page.transition);
            if (page.transition != transition)
            {
                page.transition = transition;
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            page.targetIndex = EditorGUILayout.IntField("Target Index", page.targetIndex);
            if (page.targetIndex < 0 && page.targetIndex < page.contents.Count)
            {
                page.targetIndex = 0;
            }
            else if (page.targetIndex >= page.contents.Count)
            {
                page.targetIndex = page.contents.Count - 1;
            }
            if (GUILayout.Button("-"))
            {
                if (page.targetIndex > 0)
                {
                    page.targetIndex--;
                    if (!Application.isPlaying)
                    {
                        page.SetPage(page.targetIndex);
                    }
                }
                else
                {
                    Debug.LogWarning("Page index was too small to decrease");
                }
            }
            if (GUILayout.Button("+"))
            {
                if (page.targetIndex < page.contents.Count - 1)
                {
                    page.targetIndex++;
                    if (!Application.isPlaying)
                    {
                        page.SetPage(page.targetIndex);
                    }
                }
                else
                {
                    Debug.LogWarning("Page index was too large to increase");
                }
            }
            EditorGUILayout.EndHorizontal();

            page.timeTakenDuringAnimation = EditorGUILayout.FloatField("Animation Length", page.timeTakenDuringAnimation);

            EditorGUILayout.Space();

            page.spacing = EditorGUILayout.FloatField("Spacing", page.spacing);

            EditorGUILayout.Space();

            if (page.padding.Length != 2)
            {
                page.padding = new Vector2[2];
            }
            page.padding[0] = EditorGUILayout.Vector2Field("Padding Min", page.padding[0]);
            page.padding[1] = EditorGUILayout.Vector2Field("Padding Max", page.padding[1]);

            EditorGUILayout.Space();

            //No clue what this does
            serializedObject.Update();
            content.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

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
                    page.SetPage(page.targetIndex);
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
                Undo.RecordObject(this, "Page Change");
                page.LineUp();
                page.SetPage(page.targetIndex);
                EditorSceneManager.MarkSceneDirty(page.gameObject.scene);
                EditorUtility.SetDirty(page);
            }
        }
    }
}