using UnityEngine;

namespace ifelse
{
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
            public RendererType rendererType = RendererType.Image;
            [HideInInspector]
            public float[] cornerRadii = new float[4] { 20, 20, 20, 20 };
            [HideInInspector]
            public int levelOfDetail = 8;
            [HideInInspector]
            public bool splitCorners = false;
            [HideInInspector]
            public bool overrideSorting = false;
            [HideInInspector]
            public int sortingOrderOverride = 0;

            public IFrameableObject frameInstance;

            void Awake()
            {
                rectTransform = GetComponent<RectTransform>();
            }

            void Update()
            {
                if (transform.hasChanged && !Application.isPlaying)
                {
                    CreateFrame();
                }
            }

            public void CreateFrame()
            {
                switch (cornerType)
                {
                    case FrameCornerType.Round:
                        frameInstance = new RoundFrame(rectTransform, cornerRadii, levelOfDetail);
                        break;
                    case FrameCornerType.Smooth:
                        frameInstance = new SmoothFrame(rectTransform, cornerRadii, levelOfDetail);
                        break;
                }

                GetComponent<MeshFilter>().sharedMesh = frameInstance.CreateMesh();
                UpdateFrameColor(GetComponent<MeshFilter>().sharedMesh);
                UpdateSortingOrder();
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

            void UpdateSortingOrder()
            {
                if (overrideSorting)
                {
                    GetComponent<MeshRenderer>().sortingOrder = sortingOrderOverride;
                }
                else
                {
                    GetComponent<MeshRenderer>().sortingOrder = 0;
                }
            }
        }
    }
}