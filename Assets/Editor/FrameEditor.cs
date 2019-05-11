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
    [CustomEditor(typeof(Frame))]
    public class FrameEditor : Editor
    {
        private Frame frame;

        //Basic editor stuff
        void OnEnable()
        {
            frame = (Frame)target;
        }

        void OnSceneGUI()
        {
            //Drag and drop stuff
            if (Event.current.type == EventType.MouseUp)
            {
                if (frame.GetComponentInParent<Stack>() != null)
                {
                    Stack stack = frame.GetComponentInParent<Stack>();
                    stack.SortStack();
                    stack.ForceStack();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            //Can't believe I didn't add this sooner [insert facepalm emoji]
            frame.frameColor = EditorGUILayout.ColorField("Frame Color", frame.frameColor);

            EditorGUILayout.Space();

            FrameCornerType direction = (FrameCornerType)EditorGUILayout.EnumPopup("Corner Type", frame.cornerType);
            if (frame.cornerType != direction)
            {
                frame.cornerType = direction;
            }

            EditorGUILayout.Space();

            //This toggle allows for easy uniform corner radii
            if (frame.splitCorners = EditorGUILayout.Toggle("Split Corners", frame.splitCorners))
            {
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Corner Radii", EditorStyles.boldLabel);
                if (frame.cornerRadii.Length != 4)
                {
                    frame.cornerRadii = new float[4];
                }
                frame.cornerRadii[0] = EditorGUILayout.Slider("Top Right", frame.cornerRadii[0], 0, Mathf.Min(
                                                                                                        frame.rectTransform.rect.width - frame.cornerRadii[1],
                                                                                                        frame.rectTransform.rect.height - frame.cornerRadii[3])
                                                                                                    );
                frame.cornerRadii[1] = EditorGUILayout.Slider("Top Left", frame.cornerRadii[1], 0, Mathf.Min(
                                                                                                        frame.rectTransform.rect.width - frame.cornerRadii[0],
                                                                                                        frame.rectTransform.rect.height - frame.cornerRadii[2])
                                                                                                    );
                frame.cornerRadii[2] = EditorGUILayout.Slider("Bottom Left", frame.cornerRadii[2], 0, Mathf.Min(
                                                                                                        frame.rectTransform.rect.width - frame.cornerRadii[3],
                                                                                                        frame.rectTransform.rect.height - frame.cornerRadii[1])
                                                                                                    );
                frame.cornerRadii[3] = EditorGUILayout.Slider("Bottom Right", frame.cornerRadii[3], 0, Mathf.Min(
                                                                                                        frame.rectTransform.rect.width - frame.cornerRadii[2],
                                                                                                        frame.rectTransform.rect.height - frame.cornerRadii[0])
                                                                                                    );
            }
            else
            {
                frame.cornerRadii[0] = EditorGUILayout.Slider("Uniform Corner Radius", frame.cornerRadii[0], 0, Mathf.Min(
                                                                                                                    frame.rectTransform.rect.width / 2,
                                                                                                                    frame.rectTransform.rect.height / 2)
                                                                                                                );

                for (int i = 1; i < 4; i++)
                {
                    frame.cornerRadii[i] = frame.cornerRadii[0];
                }
            }

            //You can probably increase it, but 32 should be a high enough max for it to still look good.  Even 4 looks good if the frame is small enough
            frame.levelOfDetail = EditorGUILayout.IntSlider("Vertices Per Corner", frame.levelOfDetail, 4, 32);

            //No clue what this does
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            if (GUILayout.Button("Force Create Frame Mesh"))
            {
                frame.CreateFrameMesh();
            }

            if (GUI.changed && !EditorApplication.isPlaying)
            {
                Undo.RecordObject(this, "Frame Change");
                frame.CreateFrameMesh();
                EditorSceneManager.MarkSceneDirty(frame.gameObject.scene);
                EditorUtility.SetDirty(frame);
            }
        }
    }
}