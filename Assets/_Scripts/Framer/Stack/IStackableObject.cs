using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framer
{
    public interface IStackableObject
    {
        void Stack(List<RectTransform> contents);
        List<RectTransform> Sort();
    }
}
