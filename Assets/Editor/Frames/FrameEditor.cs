﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;

namespace Frames.Editors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Frame))]
    public class FrameEditor : Editor
    {
        private Frame frame;

        private SerializedProperty frameColor;
        private SerializedProperty cornerType;
        private SerializedProperty rendererType;
        private SerializedProperty splitCorners;
        private SerializedProperty cornerRadii;
        private SerializedProperty levelOfDetail;
        private SerializedProperty overrideSorting;
        private SerializedProperty sortingOrderOverride;

        void OnEnable()
        {
            frame = (Frame)target;

            frameColor = serializedObject.FindProperty("frameColor");
            cornerType = serializedObject.FindProperty("cornerType");
            rendererType = serializedObject.FindProperty("rendererType");
            splitCorners = serializedObject.FindProperty("splitCorners");
            cornerRadii = serializedObject.FindProperty("cornerRadii");
            levelOfDetail = serializedObject.FindProperty("levelOfDetail");
            overrideSorting = serializedObject.FindProperty("overrideSorting");
            sortingOrderOverride = serializedObject.FindProperty("sortingOrderOverride");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            frameColor.colorValue = EditorGUILayout.ColorField("Frame Color", frameColor.colorValue);

            EditorGUILayout.Space();

            cornerType.enumValueIndex = (int)(FrameCornerType)EditorGUILayout.EnumPopup("Corner Type", (FrameCornerType)cornerType.enumValueIndex);

            //This toggle allows for easy uniform corner radii
            if (splitCorners.boolValue = EditorGUILayout.Toggle("Split Corners", splitCorners.boolValue))
            {
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Corner Radii", EditorStyles.boldLabel);
                if (cornerRadii.arraySize != 4)
                {
                    cornerRadii.arraySize = 4;
                }
                EditorGUILayout.BeginHorizontal();
                cornerRadii.GetArrayElementAtIndex(0).floatValue = EditorGUILayout.Slider("Top Right", cornerRadii.GetArrayElementAtIndex(0).floatValue, 0, Mathf.Min(
                                                                                                        frame.RectTransform.rect.width - cornerRadii.GetArrayElementAtIndex(1).floatValue,
                                                                                                        frame.RectTransform.rect.height - cornerRadii.GetArrayElementAtIndex(3).floatValue)
                                                                                                    );
                cornerRadii.GetArrayElementAtIndex(1).floatValue = EditorGUILayout.Slider("Top Left", cornerRadii.GetArrayElementAtIndex(1).floatValue, 0, Mathf.Min(
                                                                                                        frame.RectTransform.rect.width - cornerRadii.GetArrayElementAtIndex(2).floatValue,
                                                                                                        frame.RectTransform.rect.height - cornerRadii.GetArrayElementAtIndex(0).floatValue)
                                                                                                    );
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                cornerRadii.GetArrayElementAtIndex(2).floatValue = EditorGUILayout.Slider("Bottom Left", cornerRadii.GetArrayElementAtIndex(2).floatValue, 0, Mathf.Min(
                                                                                                        frame.RectTransform.rect.width - cornerRadii.GetArrayElementAtIndex(3).floatValue,
                                                                                                        frame.RectTransform.rect.height - cornerRadii.GetArrayElementAtIndex(1).floatValue)
                                                                                                    );
                cornerRadii.GetArrayElementAtIndex(3).floatValue = EditorGUILayout.Slider("Bottom Right", cornerRadii.GetArrayElementAtIndex(3).floatValue, 0, Mathf.Min(
                                                                                                        frame.RectTransform.rect.width - cornerRadii.GetArrayElementAtIndex(2).floatValue,
                                                                                                        frame.RectTransform.rect.height - cornerRadii.GetArrayElementAtIndex(0).floatValue)
                                                                                                    );
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                cornerRadii.GetArrayElementAtIndex(0).floatValue = EditorGUILayout.Slider("Uniform Corner Radius", cornerRadii.GetArrayElementAtIndex(0).floatValue, 0, Mathf.Min(
                                                                                                                    frame.RectTransform.rect.width * 0.5f,
                                                                                                                    frame.RectTransform.rect.height * 0.5f)
                                                                                                                );

                for (int i = 1; i < 4; i++)
                {
                    cornerRadii.GetArrayElementAtIndex(i).floatValue = cornerRadii.GetArrayElementAtIndex(0).floatValue;
                }
            }

            //You can increase this, but 32 should be a high enough max for it to still look good.  Even 4 looks good if the frame is small enough
            levelOfDetail.intValue = EditorGUILayout.IntSlider("Vertices Per Corner", levelOfDetail.intValue, 4, 32);

            EditorGUILayout.Space();

            overrideSorting.boolValue = EditorGUILayout.Toggle("Override Sorting", overrideSorting.boolValue);
            if (overrideSorting.boolValue)
            {
                sortingOrderOverride.intValue = EditorGUILayout.IntField("Sorting Layer", sortingOrderOverride.intValue);
            }

            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck() || frame.RectTransform.hasChanged)
            {
                serializedObject.ApplyModifiedProperties();
                frame.CreateFrame();
                EditorUtility.SetDirty(frame);
            }

            if (GUILayout.Button("Create Frame"))
            {
                frame.CreateFrame();
            }
        }
    }
}