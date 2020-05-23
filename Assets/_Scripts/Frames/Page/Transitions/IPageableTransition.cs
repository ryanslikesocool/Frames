using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ifelse.Frames
{
    public interface IPageableTransition
    {
        IPageableObject PageInstance { get; set; }

        ///<summary>
        ///Resets horizontal content positions for an animation
        ///</summary>
        Vector3[] LineUpHorizontal(Rect bounds, Vector2[] padding, float spacing);

        ///<summary>
        ///Resets vertical content positions for an animation
        ///</summary>
        Vector3[] LineUpVertical(Rect bounds, Vector2[] padding, float spacing);

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