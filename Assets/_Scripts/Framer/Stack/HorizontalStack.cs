﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Framer
{
    public class HorizontalStack : IStackableObject
    {
        public float inputSpacing;
        public Vector2[] assignedSpacing;

        public StackDistribution distribution;
        public StackAlignment alignment;

        public RectTransform bounds;
        public List<RectTransform> contents;

        public Vector2[] padding;

        public HorizontalStack(RectTransform bounds, List<RectTransform> contents, StackDistribution distribution, StackAlignment alignment, float inputSpacing, Vector2[] padding)
        {
            this.bounds = bounds;
            this.contents = contents;
            this.distribution = distribution;
            this.alignment = alignment;
            this.inputSpacing = inputSpacing;
            this.padding = padding;

            this.assignedSpacing = new Vector2[contents.Count];
        }

        #region Distribution

        //Snaps stack at beginning with spacing between content
        void GetStartSpacing(out Vector2[] assignedSpacing)
        {
            assignedSpacing = new Vector2[contents.Count];

            float spaceUsed = -bounds.sizeDelta.x / 2f + padding[0].x;
            for (int i = 0; i < contents.Count; i++)
            {
                assignedSpacing[i].x = spaceUsed + contents[i].sizeDelta.x / 2f;

                spaceUsed += inputSpacing + contents[i].sizeDelta.x;
            }
        }

        //Centers stack with spacing between content
        void GetCenterSpacing(out Vector2[] assignedSpacing)
        {
            assignedSpacing = new Vector2[contents.Count];

            float contentSpace = 0;
            for (int i = 0; i < contents.Count; i++)
            {
                contentSpace += contents[i].sizeDelta.x;
            }

            float startSpacing = bounds.sizeDelta.x / 2f - (contentSpace / 2f + inputSpacing * (contents.Count - 1) / 2f);

            float spaceUsed = -bounds.sizeDelta.x / 2f + startSpacing;
            for (int i = 0; i < contents.Count; i++)
            {
                assignedSpacing[i].x = spaceUsed + contents[i].sizeDelta.x / 2f;

                spaceUsed += inputSpacing + contents[i].sizeDelta.x;
            }
        }

        //Snaps stack at end with spacing between content
        void GetEndSpacing(out Vector2[] assignedSpacing)
        {
            assignedSpacing = new Vector2[contents.Count];

            float contentSpace = 0;
            for (int i = 0; i < contents.Count; i++)
            {
                contentSpace += contents[i].sizeDelta.x;
            }

            float startSpacing = bounds.sizeDelta.x - (contentSpace + inputSpacing * (contents.Count - 1));

            float spaceUsed = -bounds.sizeDelta.x / 2f + startSpacing + padding[1].x;
            for (int i = 0; i < contents.Count; i++)
            {
                assignedSpacing[i].x = spaceUsed + contents[i].sizeDelta.x / 2f;

                spaceUsed += inputSpacing + contents[i].sizeDelta.x;
            }
        }

        //Even spaces only between content, not bounds
        void GetBetweenSpacing(out Vector2[] assignedSpacing)
        {
            assignedSpacing = new Vector2[contents.Count];

            float contentSpace = 0;
            for (int i = 0; i < contents.Count; i++)
            {
                contentSpace += contents[i].sizeDelta.x;
            }

            float autoSpacing = (bounds.sizeDelta.x - contentSpace) / (contents.Count - 1) - (padding[0].x + padding[1].x) / 2f;

            float spaceUsed = -bounds.sizeDelta.x / 2f + padding[0].x;
            for (int i = 0; i < contents.Count; i++)
            {
                assignedSpacing[i].x = spaceUsed + contents[i].sizeDelta.x / 2f;

                spaceUsed += autoSpacing + contents[i].sizeDelta.x;
            }
        }

        //Cramped content with even spaces around bounds
        void GetAroundSpacing(out Vector2[] assignedSpacing)
        {
            assignedSpacing = new Vector2[contents.Count];

            float contentSpace = 0;
            for (int i = 0; i < contents.Count; i++)
            {
                contentSpace += contents[i].sizeDelta.x;
            }

            float autoSpacing = (bounds.sizeDelta.x - contentSpace) / 2;

            float spaceUsed = -bounds.sizeDelta.x / 2f + autoSpacing;
            for (int i = 0; i < contents.Count; i++)
            {
                assignedSpacing[i].x = spaceUsed + contents[i].sizeDelta.x / 2f;

                spaceUsed += contents[i].sizeDelta.x;
            }
        }

        //Even spacing between content and bounds
        void GetEvenSpacing(out Vector2[] assignedSpacing)
        {
            assignedSpacing = new Vector2[contents.Count];

            float contentSpace = 0;
            for (int i = 0; i < contents.Count; i++)
            {
                contentSpace += contents[i].sizeDelta.x;
            }

            float autoSpacing = (bounds.sizeDelta.x - contentSpace) / (contents.Count + 1);

            float spaceUsed = -bounds.sizeDelta.x / 2f + autoSpacing;
            for (int i = 0; i < contents.Count; i++)
            {
                assignedSpacing[i].x = spaceUsed + contents[i].sizeDelta.x / 2f;

                spaceUsed += autoSpacing + contents[i].sizeDelta.x;
            }
        }

        #endregion

        #region Alignment

        //Snaps elements to bottom edge of bounds
        void GetBottomAlignment()
        {
            for (int i = 0; i < contents.Count; i++)
            {
                assignedSpacing[i].y = -bounds.sizeDelta.y / 2f + contents[i].sizeDelta.y / 2f + padding[0].y;
            }
        }

        //Snaps elements to top edge of bounds
        void GetTopAlignment()
        {
            for (int i = 0; i < contents.Count; i++)
            {
                assignedSpacing[i].y = bounds.sizeDelta.y / 2f - contents[i].sizeDelta.y / 2f + padding[1].y;
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
            List<RectTransform> returnContents = new List<RectTransform>(contents);
            float[] xValues = new float[contents.Count];

            for (int i = 0; i < xValues.Length; i++)
            {
                xValues[i] = contents[i].localPosition.x;
            }

            Array.Sort(xValues);

            for (int i = 0; i < xValues.Length; i++)
            {
                foreach (RectTransform contentPiece in contents)
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