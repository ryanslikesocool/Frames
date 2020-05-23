using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ifelse.Frames
{
    public interface IFrameableObject
    {
        RectTransform RectTransform { get; set; }
        Rect Bounds { get; set; }

        Mesh CreateMesh();
    }
}