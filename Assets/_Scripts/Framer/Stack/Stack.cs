using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace Framer
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(Frame))]
    public class Stack : MonoBehaviour
    {
        [HideInInspector]
        public RectTransform rectTransform = null;
        [HideInInspector]
        public StackDirection direction = StackDirection.Horizontal;
        [HideInInspector]
        public StackDistribution distribution = StackDistribution.SpaceEvenly;
        [HideInInspector]
        public StackAlignment alignment = StackAlignment.Center;
        [HideInInspector]
        public float spacing = 0;
        [HideInInspector]
        public Vector2[] padding = new Vector2[2];
        [HideInInspector]
        public List<RectTransform> contents = new List<RectTransform>();

        public IStackableObject stackInstance;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        //Probably not necessary but you never know
        void OnEnable()
        {
            if (direction == StackDirection.Horizontal)
            {
                stackInstance = new HorizontalStack(rectTransform, contents, distribution, alignment, spacing, padding);
            }
            else
            {
                stackInstance = new VerticalStack(rectTransform, contents, distribution, alignment, spacing, padding);
            }
        }

        //Resets and gathers children on top level
        public void ResetChildren()
        {
            contents.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                contents.Add(transform.GetChild(i).GetComponent<RectTransform>());
            }
        }

        //Used for drag and drop
        public void SortStack()
        {
            contents = stackInstance.Sort();
        }

        //Forces a stack refresh
        public void ForceStack()
        {
            stackInstance.Stack(contents);
        }
    }
}