using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Framer
{
    public class FrameMaker : MonoBehaviour
    {
        //Menu for new Frame
        [MenuItem("GameObject/Framer/Frame", false, 10)]
        static void CreateFrame(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Frame");
            go.AddComponent(typeof(Frame));

            //Add canvas if needed, otherwise parent to first canvas
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
                go.transform.parent = canvas.transform;
            }

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        //Menu for new Stack
        [MenuItem("GameObject/Framer/Stack", false, 10)]
        static void CreateStack(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Stack");
            go.AddComponent(typeof(Stack));

            //Add canvas if needed, otherwise parent to first canvas
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
                go.transform.parent = canvas.transform;
            }

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}