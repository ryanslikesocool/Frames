using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framer
{
    public interface IFrameableObject
    {
        Mesh CreateMesh();
    }
}