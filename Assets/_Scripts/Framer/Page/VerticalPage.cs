using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framer
{
    public class VerticalPage : IPageableDirection
    {
        public RectTransform rectTrans;
        public Rect bounds;
        public List<RectTransform> contents;

        public PageAlignment alignment;
        public PageTransition transition;

        public Vector3[] originalPositions,
                         assignedPositions;
        public Vector2[] padding;

        public float spacing;

        public IPageableTransition pageTransition;

        public VerticalPage(RectTransform rectTrans, List<RectTransform> contents, PageAlignment alignment, PageTransition transition, Vector2[] padding, float spacing)
        {
            this.rectTrans = rectTrans;
            this.bounds = rectTrans.rect;
            this.contents = contents;
            this.alignment = alignment;
            this.padding = padding;
            this.transition = transition;
            this.spacing = spacing;

            ChangeTransition(transition);
        }

        #region Alignment

        void GetLeftAlignment()
        {
            for (int i = 0; i < contents.Count; i++)
            {
                assignedPositions[i].x = -bounds.width / 2f + contents[i].rect.width / 2f + padding[0].x;
            }
        }

        void GetRightAlignment()
        {
            for (int i = 0; i < contents.Count; i++)
            {
                assignedPositions[i].x = bounds.height / 2f - contents[i].rect.width / 2f + padding[1].x;
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
            assignedPositions = pageTransition.LineUpVertical(bounds, padding, spacing);
            originalPositions = new Vector3[contents.Count];

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

        public void SetPage(int initial, int target)
        {
            pageTransition.ChangePageVertical(initial, target, 1, 1, spacing);
            SetAssignedPositions();
        }

        public void TransitionPage(int initial, int target, float time, float duration)
        {
            float clampedTime = Mathf.Clamp(time, 0, duration);

            pageTransition.ChangePageVertical(initial, target, clampedTime, duration, spacing);
        }
    }
}