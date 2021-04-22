using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Frames
{
    public class VerticalStack : IStackableDirection
    {
        public RectTransform RectTransform { get; set; }
        public Rect Bounds { get; set; }
        public List<RectTransform> Contents { get; set; }

        public float inputSpacing;
        public Vector2[] assignedSpacing;

        public StackDistribution distribution;
        public StackAlignment alignment;

        public Vector2[] padding;

        public VerticalStack(RectTransform rectTrans, List<RectTransform> contents, StackDistribution distribution, StackAlignment alignment, float inputSpacing, Vector2[] padding)
        {
            this.RectTransform = rectTrans;
            this.Bounds = rectTrans.rect;
            this.Contents = contents;
            this.distribution = distribution;
            this.alignment = alignment;
            this.inputSpacing = inputSpacing;
            this.padding = padding;

            this.assignedSpacing = new Vector2[contents.Count];
        }

        #region Distribution

        //Snaps stack at beginning with spacing between content
        private void GetStartSpacing(out Vector2[] assignedSpacing)
        {
            assignedSpacing = new Vector2[Contents.Count];

            float spaceUsed = Bounds.height * 0.5f - padding[1].y;
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedSpacing[i].y = spaceUsed - Contents[i].rect.height * 0.5f;

                spaceUsed -= inputSpacing + Contents[i].rect.height;
            }
        }

        //Centers stack with spacing between content
        private void GetCenterSpacing(out Vector2[] assignedSpacing)
        {
            assignedSpacing = new Vector2[Contents.Count];

            float contentSpace = 0;
            for (int i = 0; i < Contents.Count; i++)
            {
                contentSpace += Contents[i].rect.height;
            }

            float startSpacing = Bounds.height * 0.5f - (contentSpace * 0.5f + inputSpacing * (Contents.Count - 1) * 0.5f);

            float spaceUsed = Bounds.height * 0.5f - startSpacing;
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedSpacing[i].y = spaceUsed - Contents[i].rect.height * 0.5f;

                spaceUsed -= inputSpacing + Contents[i].rect.height;
            }
        }

        //Snaps stack at end with spacing between content
        private void GetEndSpacing(out Vector2[] assignedSpacing)
        {
            assignedSpacing = new Vector2[Contents.Count];

            float contentSpace = 0;
            for (int i = 0; i < Contents.Count; i++)
            {
                contentSpace += Contents[i].rect.height;
            }

            float startSpacing = Bounds.height - (contentSpace + inputSpacing * (Contents.Count - 1));

            float spaceUsed = Bounds.height * 0.5f - startSpacing + padding[0].y;
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedSpacing[i].y = spaceUsed - Contents[i].rect.height * 0.5f;

                spaceUsed -= inputSpacing + Contents[i].rect.height;
            }
        }

        //Even spaces only between content, not bounds
        private void GetBetweenSpacing(out Vector2[] assignedSpacing)
        {
            assignedSpacing = new Vector2[Contents.Count];

            float contentSpace = 0;
            for (int i = 0; i < Contents.Count; i++)
            {
                contentSpace += Contents[i].rect.height;
            }

            float autoSpacing = (Bounds.height - contentSpace) / (Contents.Count - 1) - (padding[0].y + padding[1].y) * 0.5f;

            float spaceUsed = Bounds.height * 0.5f - padding[0].y;
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedSpacing[i].y = spaceUsed - Contents[i].rect.height * 0.5f;

                spaceUsed -= autoSpacing + Contents[i].rect.height;
            }
        }

        //Cramped content with even spaces around bounds
        private void GetAroundSpacing(out Vector2[] assignedSpacing)
        {
            assignedSpacing = new Vector2[Contents.Count];

            float contentSpace = 0;
            for (int i = 0; i < Contents.Count; i++)
            {
                contentSpace += Contents[i].rect.height;
            }

            float autoSpacing = (Bounds.height - contentSpace) / 2;

            float spaceUsed = Bounds.height * 0.5f - autoSpacing;
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedSpacing[i].y = spaceUsed - Contents[i].rect.height * 0.5f;

                spaceUsed -= Contents[i].rect.height;
            }
        }

        //Even spacing between content and bounds
        private void GetEvenSpacing(out Vector2[] assignedSpacing)
        {
            assignedSpacing = new Vector2[Contents.Count];

            float contentSpace = 0;
            for (int i = 0; i < Contents.Count; i++)
            {
                if (Contents[i] == null) { continue; }

                contentSpace += Contents[i].rect.height;
            }

            float autoSpacing = (Bounds.height - contentSpace) / (Contents.Count + 1);

            float spaceUsed = Bounds.height * 0.5f - autoSpacing;
            for (int i = 0; i < Contents.Count; i++)
            {
                if (Contents[i] == null) { continue; }

                assignedSpacing[i].y = spaceUsed - Contents[i].rect.height * 0.5f;

                spaceUsed -= autoSpacing + Contents[i].rect.height;
            }
        }

        #endregion

        #region Alignment

        //Snaps elements to bottom edge of bounds
        private void GetLeftAlignment()
        {
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedSpacing[i].x = -Bounds.width * 0.5f + Contents[i].rect.width * 0.5f + padding[0].x;
            }
        }

        //Snaps elements to right edge of bounds
        private void GetRightAlignment()
        {
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedSpacing[i].x = Bounds.width * 0.5f - Contents[i].rect.width * 0.5f + padding[1].x;
            }
        }

        #endregion

        //Gets spacing and alignments and sets transforms accordingly
        public void Stack(List<RectTransform> contents)
        {
            switch (distribution)
            {
                case StackDistribution.Start:
                    GetStartSpacing(out assignedSpacing);
                    break;
                case StackDistribution.Center:
                    GetCenterSpacing(out assignedSpacing);
                    break;
                case StackDistribution.End:
                    GetEndSpacing(out assignedSpacing);
                    break;
                case StackDistribution.SpaceBetween:
                    GetBetweenSpacing(out assignedSpacing);
                    break;
                case StackDistribution.SpaceAround:
                    GetAroundSpacing(out assignedSpacing);
                    break;
                case StackDistribution.SpaceEvenly:
                    GetEvenSpacing(out assignedSpacing);
                    break;
            }

            switch (alignment)
            {
                case StackAlignment.Left:
                    GetLeftAlignment();
                    break;
                case StackAlignment.Right:
                    GetRightAlignment();
                    break;
            }

            for (int i = 0; i < contents.Count; i++)
            {
                if (contents[i] != null)
                {
                    contents[i].localPosition = assignedSpacing[i];
                }
            }
        }

        //Used for drag and drop
        public List<RectTransform> Sort()
        {
            List<RectTransform> returnContents = new List<RectTransform>(Contents);
            float[] yValues = new float[Contents.Count];

            for (int i = 0; i < yValues.Length; i++)
            {
                yValues[i] = Contents[i].localPosition.y;
            }

            Array.Sort(yValues);
            Array.Reverse(yValues); //Vertical sorting needs to be reversed so it doens't start at the bottom

            for (int i = 0; i < yValues.Length; i++)
            {
                foreach (RectTransform contentPiece in Contents)
                {
                    if (contentPiece.localPosition.y == yValues[i])
                    {
                        returnContents[i] = contentPiece;
                    }
                }
            }

            return returnContents;
        }
    }
}