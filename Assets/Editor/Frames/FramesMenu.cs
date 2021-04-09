using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Frames.Editors
{
    public class FramesMenu
    {
        //Menu for new Frame
        [MenuItem("GameObject/Frames/Frame", false, 10)]
        static void CreateFrame(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Frame");
            go.AddComponent(typeof(Frame));

            FinalizeCreation(go);
        }

        //Menu for new Stack
        [MenuItem("GameObject/Frames/Stack", false, 10)]
        static void CreateStack(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Stack");
            go.AddComponent(typeof(Stack));

            FinalizeCreation(go);
        }

        //Menu for new Page
        [MenuItem("GameObject/Frames/Page", false, 10)]
        static void CreatePage(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Page");
            go.AddComponent(typeof(Page));

            FinalizeCreation(go);
        }

        static void FinalizeCreation(GameObject go)
        {
            //Parent to first canvas, otherwise create one and parent if needed
            if (GameObject.FindObjectOfType<Canvas>() != null)
            {
                go.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
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

            // Reset local scale and position because why not
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;

            // Disable unnecessary mesh rendering values
            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}