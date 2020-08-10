using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;

namespace ifelse.Frames.v2
{
    public interface IFrameObjectv2
    {
        RectTransform RectTransform { get; set; }
        Rect Rect { get; }

        void GenerateFrame(Mesh mesh, int detail, float[] cornerRadii);
    }
}