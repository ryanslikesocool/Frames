using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Frames
{
    public interface IFrameableObject
    {
        RectTransform RectTransform { get; set; }
        Rect Bounds { get; }

        FrameCornerType CornerType { get; }
        Triangulator Triangulator { get; }

        void CreateMesh(Mesh mesh);
    }
}