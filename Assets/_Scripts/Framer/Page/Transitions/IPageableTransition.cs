using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framer
{
    public interface IPageableTransition
    {
        ///<summary>
        ///Changes a horizontal page with animation
        ///</summary>
        void ChangePageHorizontal(int target, float time, float duration);

        ///<summary>
        ///Changes a vertical page with animation
        ///</summary>
        void ChangePageVertical(int target, float time, float duration);
    }
}