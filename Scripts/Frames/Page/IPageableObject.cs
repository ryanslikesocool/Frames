﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Frames
{
    public interface IPageableObject
    {
        RectTransform RectTransform { get; set; }
        Rect Bounds { get; set; }
        List<Frame> Contents { get; set; }

        ///<summary>
        ///Sets the current assigned position for the next transition use
        ///</summary>
        void SetAssignedPositions();

        ///<summary>
        ///Changes the transition type
        ///</summary>
        void ChangeTransition(PageTransition transition);

        ///<summary>
        ///Lines up all pages
        ///</summary>
        void LineUp(float spacing);

        ///<summary>
        ///Immediately snaps pages to requested index
        ///</summary>
        void SetPage(int initial, int target);

        ///<summary>
        ///Smoothly transitions page to requested index
        ///</summary>
        void TransitionPage(int initial, int target, float time, float duration);
    }
}