using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Frames
{
    public class HorizontalPage : IPageableObject
    {
        public RectTransform RectTransform { get; set; }
        public Rect Bounds { get; set; }
        public List<Frame> Contents { get; set; }

        public PageAlignment alignment;
        public PageTransition transition;

        public Vector3[] originalPositions,
                         assignedPositions;
        public Vector2[] padding;

        public float spacing;

        public IPageableTransition pageTransition;

        public HorizontalPage(RectTransform rectTransform, List<Frame> contents, PageAlignment alignment, PageTransition transition, Vector2[] padding, float spacing)
        {
            this.RectTransform = rectTransform;
            this.Bounds = rectTransform.rect;
            this.Contents = contents;
            this.alignment = alignment;
            this.padding = padding;
            this.transition = transition;
            this.spacing = spacing;

            ChangeTransition(transition);
        }

        #region Alignment

        private void GetBottomAlignment()
        {
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedPositions[i].y = -Bounds.height * 0.5f + Contents[i].Rect.height * 0.5f + padding[0].y;
            }
        }

        private void GetTopAlignment()
        {
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedPositions[i].y = Bounds.height * 0.5f - Contents[i].Rect.height * 0.5f + padding[1].y;
            }
        }

        #endregion

        public void SetAssignedPositions()
        {
            if (Contents.Contains(null)) { return; }
            assignedPositions = new Vector3[Contents.Count];
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedPositions[i] = Contents[i].LocalPosition;
            }
        }

        public void ChangeTransition(PageTransition transition)
        {
            switch (transition)
            {
                case PageTransition.Linear:
                    pageTransition = new PageTransitionLinear(Contents, this);
                    break;
                case PageTransition.Pile:
                    pageTransition = new PageTransitionPile(Contents, this);
                    break;
            }
        }

        public void LineUp(float spacing)
        {
            assignedPositions = pageTransition.LineUpHorizontal(Bounds, padding, spacing);
            originalPositions = new Vector3[Contents.Count];

            switch (alignment)
            {
                case PageAlignment.Left:
                    GetBottomAlignment();
                    break;
                case PageAlignment.Right:
                    GetTopAlignment();
                    break;
            }

            for (int i = 0; i < Contents.Count; i++)
            {
                if (Contents[i] != null)
                {
                    Contents[i].LocalPosition = assignedPositions[i];
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