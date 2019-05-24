using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framer
{
    public class PageTransitionPile : IPageableTransition
    {
        public List<RectTransform> contents;

        public Vector3[] frameDelta = null;

        public IPageableObject pageInstance;

        public PageTransitionPile(List<RectTransform> contents, IPageableObject pageInstance)
        {
            this.contents = contents;
            this.pageInstance = pageInstance;
        }

        public Vector3[] LineUpHorizontal(Rect bounds, Vector2[] padding, float spacing)
        {
            Vector3[] assignedPositions = new Vector3[contents.Count];

            float expoSpacing = spacing / 10f;

            int j = assignedPositions.Length - 1;
            for (int i = 0; i < assignedPositions.Length; i++)
            {
                assignedPositions[j].Set(Mathf.Pow(2, i * expoSpacing) - Mathf.Pow(2, (assignedPositions.Length - 1) * expoSpacing), 0, -(i - contents.Count + 1) * spacing * 2);
                j--;
            }

            return assignedPositions;
        }

        public Vector3[] LineUpVertical(Rect bounds, Vector2[] padding, float spacing)
        {
            Vector3[] assignedPositions = new Vector3[contents.Count];

            float expoSpacing = spacing / 10f;

            int j = assignedPositions.Length - 1;
            for (int i = 0; i < assignedPositions.Length; i++)
            {
                assignedPositions[j].Set(0, -Mathf.Pow(2, i * expoSpacing) + Mathf.Pow(2, (assignedPositions.Length - 1) * expoSpacing), -(i - contents.Count + 1) * spacing * 2);
                j--;
            }

            return assignedPositions;
        }

        public void ChangePageHorizontal(int initial, int target, float time, float duration, float spacing)
        {
            Vector3[] initialPositions = new Vector3[contents.Count];
            for (int i = 0; i < contents.Count; i++)
            {
                initialPositions[i] = contents[i].transform.localPosition;
            }

            frameDelta = new Vector3[contents.Count];

            float clampedTime = Mathf.Clamp(time, 0, duration);
            float expoSpacing = spacing / 10f;
            int j = contents.Count - 1;
            for (int i = 0; i < contents.Count; i++)
            {
                frameDelta[j] = new Vector3(Mathf.Pow(2, (i + target) * expoSpacing) - Mathf.Pow(2, (contents.Count - 1) * expoSpacing), 0, -((i + target) - contents.Count + 1) * spacing * 2) - initialPositions[j];

                contents[j].localPosition = Easings.Linear(clampedTime, initialPositions[j], frameDelta[j], duration);

                //Needed so the top-most frame doesn't go behind the second-to-top frame (very strange...)
                if (contents[j].localPosition.z < spacing)
                {
                    contents[j].GetComponent<MeshRenderer>().sortingOrder = 1;
                }
                else
                {
                    contents[j].GetComponent<MeshRenderer>().sortingOrder = 0;
                }

                j--;
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
            Vector3[] initialPositions = new Vector3[contents.Count];
            for (int i = 0; i < contents.Count; i++)
            {
                initialPositions[i] = contents[i].transform.localPosition;
            }

            frameDelta = new Vector3[contents.Count];

            float clampedTime = Mathf.Clamp(time, 0, duration);
            float expoSpacing = spacing / 10f;
            int j = contents.Count - 1;
            for (int i = 0; i < contents.Count; i++)
            {
                frameDelta[j] = new Vector3(0, -Mathf.Pow(2, (i + target) * expoSpacing) + Mathf.Pow(2, (contents.Count - 1) * expoSpacing), -((i + target) - contents.Count + 1) * spacing * 2) - initialPositions[j];

                contents[j].localPosition = Easings.Linear(clampedTime, initialPositions[j], frameDelta[j], duration);

                //Needed so the top-most frame doesn't go behind the second-to-top frame (very strange...)
                if (contents[j].localPosition.z < spacing)
                {
                    contents[j].GetComponent<MeshRenderer>().sortingOrder = 1;
                }
                else
                {
                    contents[j].GetComponent<MeshRenderer>().sortingOrder = 0;
                }

                j--;
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