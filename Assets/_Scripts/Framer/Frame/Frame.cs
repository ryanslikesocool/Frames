using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace Framer
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Frame : MonoBehaviour
    {
        [HideInInspector]
        public RectTransform rectTransform = null;
        [HideInInspector]
        public Color32 frameColor = Color.white;
        [HideInInspector]
        public FrameCornerType cornerType = FrameCornerType.Round;
        [HideInInspector]
        public float[] cornerRadii = new float[4] { 20, 20, 20, 20 };
        [HideInInspector]
        public int levelOfDetail = 8;
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

            GetComponent<MeshFilter>().sharedMesh = frameInstance.CreateMesh();
            UpdateFrameColor(GetComponent<MeshFilter>().sharedMesh);
        }

        void UpdateFrameColor(Mesh mesh)
        {
            //Making an entirely new color array is necessary since Mesh.colors and Mesh.colors32 are immutable
            Color32[] vertexColors = new Color32[mesh.vertexCount];
            for (int i = 0; i < vertexColors.Length; i++)
            {
                vertexColors[i] = frameColor;
            }

            mesh.colors32 = vertexColors;
        }
    }
}