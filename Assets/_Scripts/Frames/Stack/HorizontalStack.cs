using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ifelse.Frames
{
    public class HorizontalStack : IStackableDirection
    {
        public RectTransform RectTransform { get; set; }
        public Rect Bounds { get; set; }
        public List<RectTransform> Contents { get; set; }

        public float inputSpacing;
        public Vector2[] assignedSpacing;

        public StackDistribution distribution;
        public StackAlignment alignment;

        public Vector2[] padding;

        public HorizontalStack(RectTransform rectTrans, List<RectTransform> contents, StackDistribution distribution, StackAlignment alignment, float inputSpacing, Vector2[] padding)
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

            float spaceUsed = -Bounds.width / 2f + padding[0].x;
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedSpacing[i].x = spaceUsed + Contents[i].rect.width / 2f;

                spaceUsed += inputSpacing + Contents[i].rect.width;
            }
        }

        //Centers stack with spacing between content
        private void GetCenterSpacing(out Vector2[] assignedSpacing)
        {
            assignedSpacing = new Vector2[Contents.Count];

            float contentSpace = 0;
            for (int i = 0; i < Contents.Count; i++)
            {
                contentSpace += Contents[i].rect.width;
            }

            float startSpacing = Bounds.width / 2f - (contentSpace / 2f + inputSpacing * (Contents.Count - 1) / 2f);

            float spaceUsed = -Bounds.width / 2f + startSpacing;
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedSpacing[i].x = spaceUsed + Contents[i].rect.width / 2f;

                spaceUsed += inputSpacing + Contents[i].rect.width;
            }
        }

        //Snaps stack at end with spacing between content
        private void GetEndSpacing(out Vector2[] assignedSpacing)
        {
            assignedSpacing = new Vector2[Contents.Count];

            float contentSpace = 0;
            for (int i = 0; i < Contents.Count; i++)
            {
                contentSpace += Contents[i].rect.width;
            }

            float startSpacing = Bounds.width - (contentSpace + inputSpacing * (Contents.Count - 1));

            float spaceUsed = -Bounds.width / 2f + startSpacing + padding[1].x;
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedSpacing[i].x = spaceUsed + Contents[i].rect.width / 2f;

                spaceUsed += inputSpacing + Contents[i].rect.width;
            }
        }

        //Even spaces only between content, not bounds
        private void GetBetweenSpacing(out Vector2[] assignedSpacing)
        {
            assignedSpacing = new Vector2[Contents.Count];

            float contentSpace = 0;
            for (int i = 0; i < Contents.Count; i++)
            {
                contentSpace += Contents[i].rect.width;
            }

            float autoSpacing = (Bounds.width - contentSpace) / (Contents.Count - 1) - (padding[0].x + padding[1].x) / 2f;

            float spaceUsed = -Bounds.width / 2f + padding[0].x;
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedSpacing[i].x = spaceUsed + Contents[i].rect.width / 2f;

                spaceUsed += autoSpacing + Contents[i].rect.width;
            }
        }

        //Cramped content with even spaces around bounds
        private void GetAroundSpacing(out Vector2[] assignedSpacing)
        {
            assignedSpacing = new Vector2[Contents.Count];

            float contentSpace = 0;
            for (int i = 0; i < Contents.Count; i++)
            {
                contentSpace += Contents[i].rect.width;
            }

            float autoSpacing = (Bounds.width - contentSpace) / 2;

            float spaceUsed = -Bounds.width / 2f + autoSpacing;
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedSpacing[i].x = spaceUsed + Contents[i].rect.width / 2f;

                spaceUsed += Contents[i].rect.width;
            }
        }

        //Even spacing between content and bounds
        private void GetEvenSpacing(out Vector2[] assignedSpacing)
        {
            assignedSpacing = new Vector2[Contents.Count];

            float contentSpace = 0;
            for (int i = 0; i < Contents.Count; i++)
            {
                contentSpace += Contents[i].rect.width;
            }

            float autoSpacing = (Bounds.width - contentSpace) / (Contents.Count + 1);

            float spaceUsed = -Bounds.width / 2f + autoSpacing;
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedSpacing[i].x = spaceUsed + Contents[i].rect.width / 2f;

                spaceUsed += autoSpacing + Contents[i].rect.width;
            }
        }

        #endregion

        #region Alignment

        //Snaps elements to bottom edge of bounds
        private void GetBottomAlignment()
        {
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedSpacing[i].y = -Bounds.height / 2f + Contents[i].rect.height / 2f + padding[0].y;
            }
        }

        //Snaps elements to top edge of bounds
        private void GetTopAlignment()
        {
            for (int i = 0; i < Contents.Count; i++)
            {
                assignedSpacing[i].y = Bounds.height / 2f - Contents[i].rect.height / 2f + padding[1].y;
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
                    GetBottomAlignment();
                    break;
                case StackAlignment.Right:
                    GetTopAlignment();
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
            float[] xValues = new float[Contents.Count];

            for (int i = 0; i < xValues.Length; i++)
            {
                xValues[i] = Contents[i].localPosition.x;
            }

            Array.Sort(xValues);

            for (int i = 0; i < xValues.Length; i++)
            {
                foreach (RectTransform contentPiece in Contents)
                {
                    if (contentPiece.localPosition.x == xValues[i])
                    {
                        returnContents[i] = contentPiece;
                    }
                }
            }

            return returnContents;
        }
    }
}