using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ifelse
{
    namespace Framer
    {
        public class VerticalStack : IStackableDirection
        {
            public RectTransform rectTrans;
            public Rect bounds;
            public List<RectTransform> contents;

            public float inputSpacing;
            public Vector2[] assignedSpacing;

            public StackDistribution distribution;
            public StackAlignment alignment;

            public Vector2[] padding;

            public VerticalStack(RectTransform rectTrans, List<RectTransform> contents, StackDistribution distribution, StackAlignment alignment, float inputSpacing, Vector2[] padding)
            {
                this.rectTrans = rectTrans;
                this.bounds = rectTrans.rect;
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

                float spaceUsed = bounds.height / 2f - padding[1].y;
                for (int i = 0; i < contents.Count; i++)
                {
                    assignedSpacing[i].y = spaceUsed - contents[i].rect.height / 2f;

                    spaceUsed -= inputSpacing + contents[i].rect.height;
                }
            }

            //Centers stack with spacing between content
            void GetCenterSpacing(out Vector2[] assignedSpacing)
            {
                assignedSpacing = new Vector2[contents.Count];

                float contentSpace = 0;
                for (int i = 0; i < contents.Count; i++)
                {
                    contentSpace += contents[i].rect.height;
                }

                float startSpacing = bounds.height / 2f - (contentSpace / 2f + inputSpacing * (contents.Count - 1) / 2f);

                float spaceUsed = bounds.height / 2f - startSpacing;
                for (int i = 0; i < contents.Count; i++)
                {
                    assignedSpacing[i].y = spaceUsed - contents[i].rect.height / 2f;

                    spaceUsed -= inputSpacing + contents[i].rect.height;
                }
            }

            //Snaps stack at end with spacing between content
            void GetEndSpacing(out Vector2[] assignedSpacing)
            {
                assignedSpacing = new Vector2[contents.Count];

                float contentSpace = 0;
                for (int i = 0; i < contents.Count; i++)
                {
                    contentSpace += contents[i].rect.height;
                }

                float startSpacing = bounds.height - (contentSpace + inputSpacing * (contents.Count - 1));

                float spaceUsed = bounds.height / 2f - startSpacing + padding[0].y;
                for (int i = 0; i < contents.Count; i++)
                {
                    assignedSpacing[i].y = spaceUsed - contents[i].rect.height / 2f;

                    spaceUsed -= inputSpacing + contents[i].rect.height;
                }
            }

            //Even spaces only between content, not bounds
            void GetBetweenSpacing(out Vector2[] assignedSpacing)
            {
                assignedSpacing = new Vector2[contents.Count];

                float contentSpace = 0;
                for (int i = 0; i < contents.Count; i++)
                {
                    contentSpace += contents[i].rect.height;
                }

                float autoSpacing = (bounds.height - contentSpace) / (contents.Count - 1) - (padding[0].y + padding[1].y) / 2f;

                float spaceUsed = bounds.height / 2f - padding[0].y;
                for (int i = 0; i < contents.Count; i++)
                {
                    assignedSpacing[i].y = spaceUsed - contents[i].rect.height / 2f;

                    spaceUsed -= autoSpacing + contents[i].rect.height;
                }
            }

            //Cramped content with even spaces around bounds
            void GetAroundSpacing(out Vector2[] assignedSpacing)
            {
                assignedSpacing = new Vector2[contents.Count];

                float contentSpace = 0;
                for (int i = 0; i < contents.Count; i++)
                {
                    contentSpace += contents[i].rect.height;
                }

                float autoSpacing = (bounds.height - contentSpace) / 2;

                float spaceUsed = bounds.height / 2f - autoSpacing;
                for (int i = 0; i < contents.Count; i++)
                {
                    assignedSpacing[i].y = spaceUsed - contents[i].rect.height / 2f;

                    spaceUsed -= contents[i].rect.height;
                }
            }

            //Even spacing between content and bounds
            void GetEvenSpacing(out Vector2[] assignedSpacing)
            {
                assignedSpacing = new Vector2[contents.Count];

                float contentSpace = 0;
                for (int i = 0; i < contents.Count; i++)
                {
                    contentSpace += contents[i].rect.height;
                }

                float autoSpacing = (bounds.height - contentSpace) / (contents.Count + 1);

                float spaceUsed = bounds.height / 2f - autoSpacing;
                for (int i = 0; i < contents.Count; i++)
                {
                    assignedSpacing[i].y = spaceUsed - contents[i].rect.height / 2f;

                    spaceUsed -= autoSpacing + contents[i].rect.height;
                }
            }

            #endregion

            #region Alignment

            //Snaps elements to bottom edge of bounds
            void GetLeftAlignment()
            {
                for (int i = 0; i < contents.Count; i++)
                {
                    assignedSpacing[i].x = -bounds.width / 2f + contents[i].rect.width / 2f + padding[0].x;
                }
            }

            //Snaps elements to right edge of bounds
            void GetRightAlignment()
            {
                for (int i = 0; i < contents.Count; i++)
                {
                    assignedSpacing[i].x = bounds.width / 2f - contents[i].rect.width / 2f + padding[1].x;
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
                List<RectTransform> returnContents = new List<RectTransform>(contents);
                float[] yValues = new float[contents.Count];

                for (int i = 0; i < yValues.Length; i++)
                {
                    yValues[i] = contents[i].localPosition.y;
                }

                Array.Sort(yValues);
                Array.Reverse(yValues); //Vertical sorting needs to be reversed so it doens't start at the bottom

                for (int i = 0; i < yValues.Length; i++)
                {
                    foreach (RectTransform contentPiece in contents)
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
}