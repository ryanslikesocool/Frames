using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace Framer
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Frame))]
    public class Page : MonoBehaviour
    {
        [HideInInspector]
        public RectTransform rectTransform = null;
        [HideInInspector]
        public List<RectTransform> contents = new List<RectTransform>();
        [HideInInspector]
        public PageDirection direction = PageDirection.Horizontal;
        [HideInInspector]
        public PageAlignment alignment = PageAlignment.Center;
        [HideInInspector]
        public PageTransition transition = PageTransition.Linear;
        [HideInInspector]
        public float spacing = 20;
        [HideInInspector]
        public Vector2[] padding = new Vector2[2];
        [HideInInspector]
        public int currentIndex = 0;
        [HideInInspector]
        public int targetIndex = 0;
        [HideInInspector]
        public float timeTakenDuringAnimation = 0.375f;
        [HideInInspector]
        public float animationTimeElapsed = 0;
        [HideInInspector]
        public Vector2[] assignedPositions = null;

        public IPageableDirection pageInstance;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        void OnEnable()
        {
            rectTransform = GetComponent<RectTransform>();

            ChangeDirection();
            ChangeTransitionType();
        }

        void Update()
        {
            if (currentIndex != targetIndex && Application.isPlaying)
            {
                animationTimeElapsed += Time.deltaTime;
                ChangePage(targetIndex, animationTimeElapsed, timeTakenDuringAnimation);
            }
        }

        //Resets and gathers children on top level
        public void ResetChildren()
        {
            contents.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                contents.Add(transform.GetChild(i).GetComponent<RectTransform>());
            }
        }

        public void ChangeDirection()
        {
            if (direction == PageDirection.Horizontal)
            {
                pageInstance = new HorizontalPage(rectTransform, contents, alignment, transition, padding);
            }
            else
            {
                pageInstance = new VerticalPage(rectTransform, contents, alignment, transition, padding);
            }
        }

        public void ChangeTransitionType()
        {
            pageInstance.ChangeTransition(transition);
        }

        public void LineUp()
        {
            pageInstance.LineUp(spacing);
        }

        public void ChangePage(int target, float time, float duration)
        {
            if (target >= 0 && target < contents.Count)
            {
                pageInstance.TransitionPage(target, time, duration);

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

        public void SetPage(int target)
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

            currentIndex = target;

            pageInstance.SetPage(target);
        }
    }
}