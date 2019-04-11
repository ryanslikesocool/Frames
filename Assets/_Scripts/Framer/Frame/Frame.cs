using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;

namespace Framer
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Frame : MonoBehaviour
    {
        [HideInInspector]
        public RectTransform rectTransform = null;
        [HideInInspector]
        public FrameCornerType cornerType = FrameCornerType.Round;
        [HideInInspector]
        public float[] cornerRadii = new float[4] { 20, 20, 20, 20 };
        [HideInInspector]
        public int levelOfDetail = 32;
        [HideInInspector]
        public bool splitCorners = false;

        public IFrameableObject frameInstance;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        //Probably not necessary but you never know
        void OnEnable()
        {
            CreateFrameMesh();
        }

        void Update()
        {
            if (transform.hasChanged && !Application.isPlaying)
            {
                CreateFrameMesh();
            }
        }

        public void CreateFrameMesh()
        {
            if (cornerType == FrameCornerType.Round)
            {
                frameInstance = new RoundFrame(rectTransform, cornerRadii, levelOfDetail);
            }
            else
            {
                frameInstance = new SmoothFrame(rectTransform, cornerRadii, levelOfDetail);
            }

            GetComponent<MeshFilter>().mesh = frameInstance.CreateMesh();
        }
    }
}