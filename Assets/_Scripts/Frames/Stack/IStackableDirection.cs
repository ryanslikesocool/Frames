using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Frames
{
    public interface IStackableDirection
    {
        RectTransform RectTransform { get; set; }
        Rect Bounds { get; set; }
        List<RectTransform> Contents { get; set; }

        void Stack(List<RectTransform> contents);
        List<RectTransform> Sort();
    }
}