using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

namespace FramesV2
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Frame : MonoBehaviour
    {
        private MeshFilter meshFilter;

        public RectTransform RectTransform { get; private set; }
        public IFrameObject FrameObject { get; private set; }

        public int detail = 4;
        public float[] cornerRadii = new float[4]
        {
            8,
            8,
            8,
            8
        };

        private void Start()
        {
            SetFrame(FrameType.Round);
        }

        private void OnEnable()
        {
            if (meshFilter == null) { meshFilter = GetComponent<MeshFilter>(); }
            if (RectTransform == null) { RectTransform = GetComponent<RectTransform>(); }
        }

        internal void SetFrame(FrameType frameType)
        {
            switch (frameType)
            {
                case FrameType.Round:
                    FrameObject = new RoundFrame(RectTransform);
                    break;
                case FrameType.Smooth:
                    break;
            }

            RecalculateFrame();
        }

        internal void RecalculateFrame()
        {
            Mesh mesh = meshFilter.sharedMesh;
            if (mesh == null)
            {
                mesh = new Mesh();
                mesh.name = $"Frame {GetHashCode()}";
                meshFilter.sharedMesh = mesh;
            }
            mesh.Clear();

            FrameObject.GenerateFrame(meshFilter.sharedMesh, detail, cornerRadii);
        }
    }
}