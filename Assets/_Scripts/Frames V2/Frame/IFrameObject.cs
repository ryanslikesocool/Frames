using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;

namespace FramesV2
{
    public interface IFrameObject
    {
        RectTransform RectTransform { get; set; }
        Rect Rect { get; }

        void GenerateFrame(Mesh mesh, int detail, float[] cornerRadii);
    }
}