using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ifelse.Frames
{
    using Easings.Core;

    public class PageTransitionPile : IPageableTransition
    {
        public List<Frame> Contents { get; set; }

        public Vector3[] frameDelta = null;

        public IPageableObject PageInstance { get; set; }

        public PageTransitionPile(List<Frame> contents, IPageableObject pageInstance)
        {
            this.Contents = contents;
            this.PageInstance = pageInstance;
        }

        public Vector3[] LineUpHorizontal(Rect bounds, Vector2[] padding, float spacing)
        {
            Vector3[] assignedPositions = new Vector3[Contents.Count];

            float expoSpacing = Mathf.Abs(spacing) * 0.1f;
            float power = 2f;

            int j = assignedPositions.Length - 1;
            for (int i = 0; i < assignedPositions.Length; i++)
            {
                if (Contents[i] == null) { continue; }

                float x = Mathf.Sign(spacing) * (Mathf.Pow(power, i * expoSpacing) - Mathf.Pow(power, (Contents.Count - 1) * expoSpacing));
                assignedPositions[j].Set(x, 0, -(i - Contents.Count + 1) * Mathf.Abs(spacing) * 2);
                j--;
            }

            return assignedPositions;
        }

        public Vector3[] LineUpVertical(Rect bounds, Vector2[] padding, float spacing)
        {
            Vector3[] assignedPositions = new Vector3[Contents.Count];

            float expoSpacing = Mathf.Abs(spacing) * 0.1f;
            float power = 2f;

            int j = assignedPositions.Length - 1;
            for (int i = 0; i < assignedPositions.Length; i++)
            {
                if (Contents[i] == null) { continue; }

                float y = Mathf.Sign(spacing) * (Mathf.Pow(power, i * expoSpacing) - Mathf.Pow(power, (Contents.Count - 1) * expoSpacing));
                assignedPositions[j].Set(0, y, -(i - Contents.Count + 1) * Mathf.Abs(spacing) * 2);
                j--;
            }

            return assignedPositions;
        }

        public void ChangePageHorizontal(int initial, int target, float time, float duration, float spacing)
        {
            Vector3[] initialPositions = new Vector3[Contents.Count];
            for (int i = 0; i < Contents.Count; i++)
            {
                initialPositions[i] = Contents[i].transform.localPosition;
            }

            float expoSpacing = Mathf.Abs(spacing) * 0.1f;
            float power = 2f;

            frameDelta = new Vector3[Contents.Count];

            float clampedTime = Mathf.Clamp(time, 0, duration);
            int j = Contents.Count - 1;
            for (int i = 0; i < Contents.Count; i++)
            {
                float x = Mathf.Sign(spacing) * (Mathf.Pow(power, (i + target) * expoSpacing) - Mathf.Pow(power, (Contents.Count - 1) * expoSpacing));
                frameDelta[j] = new Vector3(x, 0, -((i + target) - Contents.Count + 1) * Mathf.Abs(spacing) * 2) - initialPositions[j];

                Contents[j].LocalPosition = Easings.Linear(clampedTime, initialPositions[j], frameDelta[j], duration);

                //Needed so the top-most frame doesn't go behind the second-to-top frame (very strange...)
                if (Contents[j].LocalPosition.z < spacing)
                {
                    Contents[j].GetComponent<MeshRenderer>().sortingOrder = 1;
                }
                else
                {
                    Contents[j].GetComponent<MeshRenderer>().sortingOrder = 0;
                }

                j--;
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
            Vector3[] initialPositions = new Vector3[Contents.Count];
            for (int i = 0; i < Contents.Count; i++)
            {
                initialPositions[i] = Contents[i].transform.localPosition;
            }

            float expoSpacing = Mathf.Abs(spacing) * 0.1f;
            float power = 2f;

            frameDelta = new Vector3[Contents.Count];

            float clampedTime = Mathf.Clamp(time, 0, duration);
            int j = Contents.Count - 1;
            for (int i = 0; i < Contents.Count; i++)
            {
                float y = Mathf.Sign(spacing) * (Mathf.Pow(power, (i + target) * expoSpacing) - Mathf.Pow(power, (Contents.Count - 1) * expoSpacing));
                frameDelta[j] = new Vector3(0, y, -((i + target) - Contents.Count + 1) * Mathf.Abs(spacing) * 2) - initialPositions[j];

                Contents[j].LocalPosition = Easings.Linear(clampedTime, initialPositions[j], frameDelta[j], duration);

                //Needed so the top-most frame doesn't go behind the second-to-top frame (very strange...)
                if (Contents[j].LocalPosition.z < spacing)
                {
                    Contents[j].GetComponent<MeshRenderer>().sortingOrder = 1;
                }
                else
                {
                    Contents[j].GetComponent<MeshRenderer>().sortingOrder = 0;
                }

                j--;
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