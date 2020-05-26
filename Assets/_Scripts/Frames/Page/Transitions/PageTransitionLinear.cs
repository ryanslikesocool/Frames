using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ifelse.Frames
{
    public class PageTransitionLinear : IPageableTransition
    {
        public List<Frame> Contents { get; set; }

        public Vector2[] frameDelta = null;

        public IPageableObject PageInstance { get; set; }

        public PageTransitionLinear(List<Frame> contents, IPageableObject pageInstance)
        {
            this.Contents = contents;
            this.PageInstance = pageInstance;
        }

        public Vector3[] LineUpHorizontal(Rect bounds, Vector2[] padding, float spacing)
        {
            Vector3[] assignedPositions = new Vector3[Contents.Count];

            float spaceUsed = -bounds.width * 0.5f + padding[0].x;
            for (int i = 0; i < Contents.Count; i++)
            {
                if (Contents[i] == null) { continue; }

                assignedPositions[i].x = spaceUsed + Contents[i].RectTransform.rect.width * 0.5f;

                spaceUsed += spacing + Contents[i].Rect.width;
            }

            return assignedPositions;
        }

        public Vector3[] LineUpVertical(Rect bounds, Vector2[] padding, float spacing)
        {
            Vector3[] assignedPositions = new Vector3[Contents.Count];

            float spaceUsed = bounds.height * 0.5f - padding[1].y;
            for (int i = 0; i < Contents.Count; i++)
            {
                if (Contents[i] == null) { continue; }

                assignedPositions[i].y = spaceUsed - Contents[i].Rect.height * 0.5f;

                spaceUsed -= spacing + Contents[i].Rect.height;
            }

            return assignedPositions;
        }

        public void ChangePageHorizontal(int initial, int target, float time, float duration, float spacing)
        {
            if (Contents.Contains(null)) { return; }
            Vector2[] initialPositions = new Vector2[Contents.Count];
            for (int i = 0; i < Contents.Count; i++)
            {
                initialPositions[i].Set(Contents[i].transform.localPosition.x, Contents[i].transform.localPosition.y);
            }

            frameDelta = new Vector2[Contents.Count];

            float clampedTime = Mathf.Clamp(time, 0, duration);
            float deltaX = -Contents[target].LocalPosition.x;
            for (int i = 0; i < Contents.Count; i++)
            {
                frameDelta[i].Set(deltaX, 0);
                Contents[i].LocalPosition = Easings.Linear(clampedTime, initialPositions[i], frameDelta[i], duration);
            }

            if (time == duration)
            {
                for (int i = 0; i < Contents.Count; i++)
                {
                    PageInstance.SetAssignedPositions();
                }
            }
        }

        public void ChangePageVertical(int initial, int target, float time, float duration, float spacing)
        {
            Vector2[] initialPositions = new Vector2[Contents.Count];
            for (int i = 0; i < Contents.Count; i++)
            {
                initialPositions[i].Set(Contents[i].transform.localPosition.x, Contents[i].transform.localPosition.y);
            }

            frameDelta = new Vector2[Contents.Count];

            float clampedTime = Mathf.Clamp(time, 0, duration);
            float deltaY = -Contents[target].LocalPosition.y;
            for (int i = 0; i < Contents.Count; i++)
            {
                frameDelta[i].Set(0, deltaY);

                Contents[i].LocalPosition = Easings.Linear(clampedTime, initialPositions[i], frameDelta[i], duration);
            }

            if (time == duration)
            {
                for (int i = 0; i < Contents.Count; i++)
                {
                    PageInstance.SetAssignedPositions();
                }
            }
        }
    }
}