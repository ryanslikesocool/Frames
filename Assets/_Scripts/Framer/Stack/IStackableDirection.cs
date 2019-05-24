using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framer
{
    public interface IStackableDirection
    {
        void Stack(List<RectTransform> contents);
        List<RectTransform> Sort();
    }
}
