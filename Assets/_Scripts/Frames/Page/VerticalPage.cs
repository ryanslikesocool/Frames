using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ifelse.Frames
{
    public class VerticalPage : IPageableObject
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

        public VerticalPage(RectTransform rectTrans, List<Frame> contents, PageAlignment alignment, PageTransition transition, Vector2[] padding, float spacing)
        {
            this.RectTransform = rectTrans;
            this.Bounds = rectTrans.rect;
            this.Contents = contents;
            this.alignment = alignment;
            this.padding = padding;
            this.transition = transition;
            this.spacing = spacing;

            ChangeTransition(transition);
        }

        #region Alignment

        private void GetLeftAlignment()
        {
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedPositions[i].x = -Bounds.width / 2f + Contents[i].Rect.width / 2f + padding[0].x;
            }
        }

        private void GetRightAlignment()
        {
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedPositions[i].x = Bounds.height / 2f - Contents[i].Rect.width / 2f + padding[1].x;
            }
        }

        #endregion

        public void SetAssignedPositions()
        {
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
            assignedPositions = pageTransition.LineUpVertical(Bounds, padding, spacing);
            originalPositions = new Vector3[Contents.Count];

            switch (alignment)
            {
                case PageAlignment.Left:
                    GetLeftAlignment();
                    break;
                case PageAlignment.Right:
                    GetRightAlignment();
                    break;
            }

            for (int i = 0; i < Contents.Count; i++)
            {
                if (Contents[i] != null)
                {
                    Contents[i].LocalPosition = assignedPositions[i];
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