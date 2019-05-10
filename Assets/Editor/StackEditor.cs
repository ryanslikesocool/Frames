using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;
using System;
using UnityEditor.SceneManagement;

namespace Framer
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Stack))]
    public class StackBaseEditor : Editor
    {
        private Stack stack;
        private ReorderableList content;

        //Just your basic custom inspector enable stuff
        public void OnEnable()
        {
            stack = (Stack)target;

            //I think (???) this is for the reorderable list.  I don't remember
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
            StackDirection direction = (StackDirection)EditorGUILayout.EnumPopup("Direction", stack.direction);
            if (stack.direction != direction)
            {
                stack.direction = direction;
            }

            StackDistribution distribution = (StackDistribution)EditorGUILayout.EnumPopup("Distribution", stack.distribution);
            if (stack.distribution != distribution)
            {
                stack.distribution = distribution;
            }

            StackAlignment alignment = (StackAlignment)EditorGUILayout.EnumPopup("Alignment", stack.alignment);
            if (stack.alignment != alignment)
            {
                stack.alignment = alignment;
            }

            EditorGUILayout.Space();

            if (stack.padding.Length != 2)
            {
                stack.padding = new Vector2[2];
            }
            stack.padding[0] = EditorGUILayout.Vector2Field("Padding Min", stack.padding[0]);
            stack.padding[1] = EditorGUILayout.Vector2Field("Padding Max", stack.padding[1]);

            EditorGUILayout.Space();

            //Enables spacing for distributions without auto-spacing
            switch (stack.distribution)
            {
                case StackDistribution.Start:
                case StackDistribution.End:
                case StackDistribution.Center:
                    stack.spacing = EditorGUILayout.FloatField("Spacing", stack.spacing);
                    break;
            }

            //No clue what this does
            serializedObject.Update();
            content.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            //For some reason, the minus button on the reorderable list in the editor
            //doens't remove the entry in the list.  This fixes that
            List<RectTransform> toRemove = new List<RectTransform>();
            foreach (RectTransform rt in stack.contents)
            {
                if (rt == null)
                {
                    toRemove.Add(rt);
                }
            }
            foreach (RectTransform rt in toRemove)
            {
                stack.contents.Remove(rt);
            }

            //Checks if the stack order has changed
            if (content != new ReorderableList(serializedObject, serializedObject.FindProperty("contents"), true, true, true, true))
            {
                if (stack.direction == StackDirection.Horizontal)
                {
                    stack.stackInstance = new HorizontalStack(stack.rectTransform, stack.contents, stack.distribution, stack.alignment, stack.spacing, stack.padding);
                }
                else
                {
                    stack.stackInstance = new VerticalStack(stack.rectTransform, stack.contents, stack.distribution, stack.alignment, stack.spacing, stack.padding);
                }
                stack.ForceStack();
            }

            //Fixes stuff when stuff goes wrong.  Shouldn't need to be used *too* often (hopefully)
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Force Restack"))
            {
                if (stack.direction == StackDirection.Horizontal)
                {
                    stack.stackInstance = new HorizontalStack(stack.rectTransform, stack.contents, stack.distribution, stack.alignment, stack.spacing, stack.padding);
                }
                else
                {
                    stack.stackInstance = new VerticalStack(stack.rectTransform, stack.contents, stack.distribution, stack.alignment, stack.spacing, stack.padding);
                }
                stack.ForceStack();
            }
            if (GUILayout.Button("Force Reset Children"))
            {
                stack.ResetChildren();
            }
            GUILayout.EndHorizontal();

            if (GUI.changed && !EditorApplication.isPlaying)
            {
                Undo.RecordObject(this, "Stack Change");
                EditorSceneManager.MarkSceneDirty(stack.gameObject.scene);
                EditorUtility.SetDirty(stack);
            }
        }
    }
}