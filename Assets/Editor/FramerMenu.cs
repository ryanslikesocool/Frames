using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace ifelse
{
    namespace Framer
    {
        public class FramerMenu : MonoBehaviour
        {
            //Menu for new Frame
            [MenuItem("GameObject/Framer/Frame", false, 10)]
            static void CreateFrame(MenuCommand menuCommand)
            {
                GameObject go = new GameObject("Frame");
                go.AddComponent(typeof(Frame));

                FinalizeCreation(go);
            }

            //Menu for new Stack
            [MenuItem("GameObject/Framer/Stack", false, 10)]
            static void CreateStack(MenuCommand menuCommand)
            {
                GameObject go = new GameObject("Stack");
                go.AddComponent(typeof(Stack));

                FinalizeCreation(go);
            }

            //Menu for new Page
            [MenuItem("GameObject/Framer/Page", false, 10)]
            static void CreatePage(MenuCommand menuCommand)
            {
                GameObject go = new GameObject("Page");
                go.AddComponent(typeof(Page));

                FinalizeCreation(go);
            }

            static void FinalizeCreation(GameObject go)
            {
                //Parent to first canvas, otherwise create one and parent if needed
                if (FindObjectOfType<Canvas>() != null)
                {
                    go.transform.parent = FindObjectOfType<Canvas>().transform;
                }
                else
                {
                    GameObject canvas = new GameObject("Canvas");
                    canvas.AddComponent(typeof(Canvas));
                    canvas.AddComponent(typeof(CanvasScaler));
                    canvas.AddComponent(typeof(GraphicRaycaster));
                    canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

                    go.transform.SetParent(canvas.transform);
                }

                // Register the creation in the undo system
                Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
                Selection.activeObject = go;
            }
        }
    }
}