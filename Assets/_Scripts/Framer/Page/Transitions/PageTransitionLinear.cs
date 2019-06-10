using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ifelse
{
    namespace Framer
    {
        public class PageTransitionLinear : IPageableTransition
        {
            public List<RectTransform> contents;

            public Vector2[] frameDelta = null;

            public IPageableObject pageInstance;

            public PageTransitionLinear(List<RectTransform> contents, IPageableObject pageInstance)
            {
                this.contents = contents;
                this.pageInstance = pageInstance;
            }

            public Vector3[] LineUpHorizontal(Rect bounds, Vector2[] padding, float spacing)
            {
                Vector3[] assignedPositions = new Vector3[contents.Count];

                float spaceUsed = -bounds.width / 2f + padding[0].x;
                for (int i = 0; i < contents.Count; i++)
                {
                    assignedPositions[i].x = spaceUsed + contents[i].rect.width / 2f;

                    spaceUsed += spacing + contents[i].rect.width;
                }

                return assignedPositions;
            }

            public Vector3[] LineUpVertical(Rect bounds, Vector2[] padding, float spacing)
            {
                Vector3[] assignedPositions = new Vector3[contents.Count];

                float spaceUsed = bounds.height / 2f - padding[1].y;
                for (int i = 0; i < contents.Count; i++)
                {
                    assignedPositions[i].y = spaceUsed - contents[i].rect.height / 2f;

                    spaceUsed -= spacing + contents[i].rect.height;
                }

                return assignedPositions;
            }

            public void ChangePageHorizontal(int initial, int target, float time, float duration, float spacing)
            {
                Vector2[] initialPositions = new Vector2[contents.Count];
                for (int i = 0; i < contents.Count; i++)
                {
                    initialPositions[i].Set(contents[i].transform.localPosition.x, contents[i].transform.localPosition.y);
                }

                frameDelta = new Vector2[contents.Count];

                float clampedTime = Mathf.Clamp(time, 0, duration);
                float deltaX = -contents[target].localPosition.x;
                for (int i = 0; i < contents.Count; i++)
                {
                    frameDelta[i].Set(deltaX, 0);

                    contents[i].localPosition = Easings.Linear(clampedTime, initialPositions[i], frameDelta[i], duration);
                }

                if (time == duration)
                {
                    for (int i = 0; i < contents.Count; i++)
                    {
                        pageInstance.SetAssignedPositions();
                    }
                }
            }

            public void ChangePageVertical(int initial, int target, float time, float duration, float spacing)
            {
                Vector2[] initialPositions = new Vector2[contents.Count];
                for (int i = 0; i < contents.Count; i++)
                {
                    initialPositions[i].Set(contents[i].transform.localPosition.x, contents[i].transform.localPosition.y);
                }

                frameDelta = new Vector2[contents.Count];

                float clampedTime = Mathf.Clamp(time, 0, duration);
                float deltaY = -contents[target].localPosition.y;
                for (int i = 0; i < contents.Count; i++)
                {
                    frameDelta[i].Set(0, deltaY);

                    contents[i].localPosition = Easings.Linear(clampedTime, initialPositions[i], frameDelta[i], duration);
                }

                if (time == duration)
                {
                    for (int i = 0; i < contents.Count; i++)
                    {
                        pageInstance.SetAssignedPositions();
                    }
                }
            }
        }
    }
}