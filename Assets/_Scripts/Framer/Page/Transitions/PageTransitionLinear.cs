using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framer
{
    public class PageTransitionLinear : IPageableTransition
    {
        public List<RectTransform> contents;

        public Vector2[] frameDelta = null;

        public IPageableDirection pageInstance;

        public PageTransitionLinear(List<RectTransform> contents, IPageableDirection pageInstance)
        {
            this.contents = contents;
            this.pageInstance = pageInstance;
        }

        public void ChangePageHorizontal(int target, float time, float duration)
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

        public void ChangePageVertical(int target, float time, float duration)
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