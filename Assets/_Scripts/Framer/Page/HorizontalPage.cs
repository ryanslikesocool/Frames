using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framer
{
    public class HorizontalPage : IPageableDirection
    {
        public RectTransform bounds;
        public List<RectTransform> contents;

        public PageAlignment alignment;
        public PageTransition transition;

        public Vector3[] originalPositions,
                         assignedPositions;
        public Vector2[] padding;

        public float spacing;

        public IPageableTransition pageTransition;

        public HorizontalPage(RectTransform bounds, List<RectTransform> contents, PageAlignment alignment, PageTransition transition, Vector2[] padding, float spacing)
        {
            this.bounds = bounds;
            this.contents = contents;
            this.alignment = alignment;
            this.padding = padding;
            this.transition = transition;
            this.spacing = spacing;

            ChangeTransition(transition);
        }

        #region Alignment

        void GetBottomAlignment()
        {
            for (int i = 0; i < contents.Count; i++)
            {
                assignedPositions[i].y = -bounds.sizeDelta.y / 2f + contents[i].sizeDelta.y / 2f + padding[0].y;
            }
        }

        void GetTopAlignment()
        {
            for (int i = 0; i < contents.Count; i++)
            {
                assignedPositions[i].y = bounds.sizeDelta.y / 2f - contents[i].sizeDelta.y / 2f + padding[1].y;
            }
        }

        #endregion

        public void SetAssignedPositions()
        {
            assignedPositions = new Vector3[contents.Count];
            for (int i = 0; i < contents.Count; i++)
            {
                assignedPositions[i] = contents[i].localPosition;
            }
        }

        public void ChangeTransition(PageTransition transition)
        {
            switch (transition)
            {
                case PageTransition.Linear:
                    pageTransition = new PageTransitionLinear(contents, this);
                    break;
                case PageTransition.Pile:
                    pageTransition = new PageTransitionPile(contents, this);
                    break;
            }
        }

        public void LineUp(float spacing)
        {
            assignedPositions = pageTransition.LineUpHorizontal(bounds, padding, spacing);
            originalPositions = new Vector3[contents.Count];

            switch (alignment)
            {
                case PageAlignment.Left:
                    GetBottomAlignment();
                    break;
                case PageAlignment.Right:
                    GetTopAlignment();
                    break;
            }

            for (int i = 0; i < contents.Count; i++)
            {
                if (contents[i] != null)
                {
                    contents[i].localPosition = assignedPositions[i];
                }
            }
            originalPositions = assignedPositions;
        }

        public void SetPage(int initial, int target)
        {
            pageTransition.ChangePageHorizontal(initial, target, 1, 1, spacing);
            SetAssignedPositions();
        }

        public void TransitionPage(int initial, int target, float time, float duration)
        {
            float clampedTime = Mathf.Clamp(time, 0, duration);

            pageTransition.ChangePageHorizontal(initial, target, clampedTime, duration, spacing);
        }
    }
}