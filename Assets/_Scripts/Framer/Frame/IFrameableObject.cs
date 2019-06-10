using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ifelse
{
    namespace Framer
    {
        public interface IFrameableObject
        {
            Mesh CreateMesh();
        }
    }
}