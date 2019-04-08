using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;

namespace Framer
{
    [CustomEditor(typeof(Frame))]
    public class FrameEditor : Editor
    {
        private Frame element;

        //Basic editor stuff
        void OnEnable()
        {
            element = (Frame)target;
        }

        void OnSceneGUI()
        {
            //Drag and drop stuff
            if (Event.current.type == EventType.MouseUp)
            {
                if (element.GetComponentInParent<Stack>() != null)
                {
                    element.GetComponentInParent<Stack>().SortStack();
                    element.GetComponentInParent<Stack>().ForceStack();
                }
            }
        }
    }
}