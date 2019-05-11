using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framer
{
    public interface IPageableTransition
    {
        ///<summary>
        ///Resets horizontal content positions for an animation
        ///</summary>
        Vector3[] LineUpHorizontal(RectTransform bounds, Vector2[] padding, float spacing);

        ///<summary>
        ///Resets vertical content positions for an animation
        ///</summary>
        Vector3[] LineUpVertical(RectTransform bounds, Vector2[] padding, float spacing);

        ///<summary>
        ///Changes a horizontal page with animation
        ///</summary>
        void ChangePageHorizontal(int initial, int target, float time, float duration, float spacing);

        ///<summary>
        ///Changes a vertical page with animation
        ///</summary>
        void ChangePageVertical(int initial, int target, float time, float duration, float spacing);
    }
}