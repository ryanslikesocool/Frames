using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framer
{
    public class VerticalPage : IPageableDirection
    {
        public RectTransform bounds;
        public List<RectTransform> contents;

        public PageAlignment alignment;
        public PageTransition transition;

        public Vector2[] originalPositions;
        public Vector2[] assignedPositions;
        public Vector2[] padding;

        public IPageableTransition pageTransition;

        public VerticalPage(RectTransform bounds, List<RectTransform> contents, PageAlignment alignment, PageTransition transition, Vector2[] padding)
        {
            this.bounds = bounds;
            this.contents = contents;
            this.alignment = alignment;
            this.padding = padding;
            this.transition = transition;

            ChangeTransition(transition);
        }

        #region Alignment

        void GetLeftAlignment()
        {
            for (int i = 0; i < contents.Count; i++)
            {
                assignedPositions[i].x = -bounds.sizeDelta.x / 2f + contents[i].sizeDelta.x / 2f + padding[0].x;
            }
        }

        void GetRightAlignment()
        {
            for (int i = 0; i < contents.Count; i++)
            {
                assignedPositions[i].x = bounds.sizeDelta.x / 2f - contents[i].sizeDelta.x / 2f + padding[1].x;
            }
        }

        #endregion

        public void SetAssignedPositions()
        {
            assignedPositions = new Vector2[contents.Count];
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
            }
        }

        public void LineUp(float spacing)
        {
            assignedPositions = new Vector2[contents.Count];
            originalPositions = new Vector2[contents.Count];

            float spaceUsed = bounds.sizeDelta.y / 2f - padding[1].y;
            for (int i = 0; i < contents.Count; i++)
            {
                assignedPositions[i].y = spaceUsed - contents[i].sizeDelta.y / 2f;

                spaceUsed -= spacing + contents[i].sizeDelta.y;
            }

            switch (alignment)
            {
                case PageAlignment.Left:
                    GetLeftAlignment();
                    break;
                case PageAlignment.Right:
                    GetRightAlignment();
                    break;
            }

            for (int i = 0; i < contents.Count; i++)
            {
                if (contents[i] != null)
                {
                    contents[i].localPosition = assignedPositions[i];
                    originalPositions[i] = assignedPositions[i];
                }
            }
        }

        public void SetPage(int target)
        {
            pageTransition.ChangePageVertical(target, 1, 1);
            SetAssignedPositions();
        }

        public void TransitionPage(int target, float time, float duration)
        {
            float clampedTime = Mathf.Clamp(time, 0, duration);

            pageTransition.ChangePageVertical(target, clampedTime, duration);
        }
    }
}