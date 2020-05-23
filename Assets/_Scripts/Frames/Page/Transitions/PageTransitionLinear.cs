using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ifelse.Frames
{
    public class PageTransitionLinear : IPageableTransition
    {
        public List<Frame> contents;

        public Vector2[] frameDelta = null;

        public IPageableObject PageInstance { get; set; }

        public PageTransitionLinear(List<Frame> contents, IPageableObject pageInstance)
        {
            this.contents = contents;
            this.PageInstance = pageInstance;
        }

        public Vector3[] LineUpHorizontal(Rect bounds, Vector2[] padding, float spacing)
        {
            Vector3[] assignedPositions = new Vector3[contents.Count];

            float spaceUsed = -bounds.width / 2f + padding[0].x;
            for (int i = 0; i < contents.Count; i++)
            {
                assignedPositions[i].x = spaceUsed + contents[i].RectTransform.rect.width / 2f;

                spaceUsed += spacing + contents[i].Rect.width;
            }

            return assignedPositions;
        }

        public Vector3[] LineUpVertical(Rect bounds, Vector2[] padding, float spacing)
        {
            Vector3[] assignedPositions = new Vector3[contents.Count];

            float spaceUsed = bounds.height / 2f - padding[1].y;
            for (int i = 0; i < contents.Count; i++)
            {
                assignedPositions[i].y = spaceUsed - contents[i].Rect.height / 2f;

                spaceUsed -= spacing + contents[i].Rect.height;
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
            float deltaX = -contents[target].LocalPosition.x;
            for (int i = 0; i < contents.Count; i++)
            {
                frameDelta[i].Set(deltaX, 0);

                contents[i].LocalPosition = Easings.Linear(clampedTime, initialPositions[i], frameDelta[i], duration);
            }

            if (time == duration)
            {
                for (int i = 0; i < contents.Count; i++)
                {
                    PageInstance.SetAssignedPositions();
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
            float deltaY = -contents[target].LocalPosition.y;
            for (int i = 0; i < contents.Count; i++)
            {
                frameDelta[i].Set(0, deltaY);

                contents[i].LocalPosition = Easings.Linear(clampedTime, initialPositions[i], frameDelta[i], duration);
            }

            if (time == duration)
            {
                for (int i = 0; i < contents.Count; i++)
                {
                    PageInstance.SetAssignedPositions();
                }
            }
        }
    }
}