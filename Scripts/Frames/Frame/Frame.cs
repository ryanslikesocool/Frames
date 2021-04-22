using UnityEngine;

namespace Frames
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Frame : MonoBehaviour
    {
        public RectTransform RectTransform { get; private set; }
        public Rect Rect { get { return RectTransform.rect; } }
        public MeshFilter MeshFilter { get; private set; }
        public MeshRenderer MeshRenderer { get; private set; }

        public Vector3 LocalPosition
        {
            get { return RectTransform.localPosition; }
            set { RectTransform.localPosition = value; }
        }
        public Vector2 AnchoredPosition
        {
            get { return RectTransform.anchoredPosition; }
            set { RectTransform.anchoredPosition = value; }
        }

        public Color32 frameColor = Color.white;
        public FrameCornerType cornerType = FrameCornerType.Round;
        public RendererType rendererType = RendererType.Image;
        public float[] cornerRadii = new float[4] { 20, 20, 20, 20 };
        public int levelOfDetail = 8;
        public bool splitCorners = false;
        public bool overrideSorting = false;
        public int sortingOrderOverride = 0;

        public IFrameableObject FrameInstance { get; private set; }

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            MeshFilter = GetComponent<MeshFilter>();
            MeshRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            if (transform.hasChanged && !Application.isPlaying)
            {
                if (RectTransform == null) { RectTransform = GetComponent<RectTransform>(); }
                if (MeshFilter == null) { MeshFilter = GetComponent<MeshFilter>(); }
                if (MeshRenderer == null) { MeshRenderer = GetComponent<MeshRenderer>(); }
                CreateFrame();
            }
        }

        public void CreateFrame()
        {
            if (FrameInstance == null || FrameInstance.CornerType != cornerType)
            {
                switch (cornerType)
                {
                    case FrameCornerType.Round:
                        FrameInstance = new RoundFrame(RectTransform, cornerRadii, levelOfDetail);
                        break;
                    case FrameCornerType.Smooth:
                        FrameInstance = new SmoothFrame(RectTransform, cornerRadii, levelOfDetail);
                        break;
                }
            }

            FrameInstance.CreateMesh(MeshFilter.sharedMesh);
            UpdateFrameColor(MeshFilter.sharedMesh);
            UpdateSortingOrder();
        }

        private void UpdateFrameColor(Mesh mesh)
        {
            //Making an entirely new color array is necessary since Mesh.colors and Mesh.colors32 are immutable
            Color32[] vertexColors = new Color32[mesh.vertexCount];
            for (int i = 0; i < vertexColors.Length; i++)
            {
                vertexColors[i] = frameColor;
            }

            mesh.colors32 = vertexColors;
        }

        private void UpdateSortingOrder()
        {
            MeshRenderer.sortingOrder = overrideSorting ? sortingOrderOverride : 0;
        }
    }
}