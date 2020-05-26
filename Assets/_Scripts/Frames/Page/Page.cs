using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace ifelse.Frames
{
    [ExecuteInEditMode]
    public class Page : MonoBehaviour
    {
        public RectTransform RectTransform { get; private set; }
        public List<Frame> contents = new List<Frame>();
        public PageDirection direction = PageDirection.Horizontal;
        public PageAlignment alignment = PageAlignment.Center;
        public PageTransition transition = PageTransition.Linear;
        public float spacing = 20;
        public Vector2[] padding = new Vector2[2],
                         assignedPositions = null;
        public int currentIndex = 0,
                   targetIndex = 0;
        public float animationDuration = 0.375f,
                     animationTimeElapsed = 0;

        public IPageableObject pageInstance;

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            ChangeDirection();
            ChangeTransitionType();
        }

        private void Update()
        {
            if (currentIndex != targetIndex && Application.isPlaying)
            {
                animationTimeElapsed += Time.deltaTime;
                ChangePage(targetIndex, animationTimeElapsed, animationDuration);
            }
        }

        //Reset and gather children at top level
        public void ResetChildren()
        {
            contents.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                contents.Add(transform.GetChild(i).GetComponent<Frame>());
            }
        }

        public void ChangeDirection()
        {
            if (direction == PageDirection.Horizontal)
            {
                pageInstance = new HorizontalPage(RectTransform, contents, alignment, transition, padding, spacing);
            }
            else
            {
                pageInstance = new VerticalPage(RectTransform, contents, alignment, transition, padding, spacing);
            }
        }

        public void ChangeTransitionType()
        {
            pageInstance.ChangeTransition(transition);
        }

        public void LineUp()
        {
            if (contents.Count == 0) { return; }

            pageInstance.LineUp(spacing);
        }

        public void ChangePage(int target, float time, float duration)
        {
            if (target >= 0 && target < contents.Count)
            {
                pageInstance.TransitionPage(currentIndex, target, time, duration);

                if (animationTimeElapsed >= duration)
                {
                    currentIndex = target;
                    animationTimeElapsed = 0;
                }
            }
            else
            {
                if (target < 0 && target < contents.Count)
                {
                    currentIndex = 0;
                    targetIndex = 0;
                    target = 0;
                }
                else if (target >= contents.Count)
                {
                    currentIndex = contents.Count - 1;
                    targetIndex = contents.Count - 1;
                    target = contents.Count - 1;
                }

                animationTimeElapsed = 0;
            }
        }

        public void SetPage(int initial, int target)
        {
            if (contents.Count == 0) { return; }

            if (target < 0 && target < contents.Count)
            {
                currentIndex = 0;
                targetIndex = 0;
                target = 0;
            }
            else if (target >= contents.Count)
            {
                currentIndex = contents.Count - 1;
                targetIndex = contents.Count - 1;
                target = contents.Count - 1;
            }

            currentIndex = target;

            pageInstance.SetPage(initial, target);
        }
    }
}