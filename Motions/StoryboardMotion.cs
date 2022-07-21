using System.Collections.Generic;
using UnityEngine;

namespace BlastonCameraBehaviour.Motions
{
    public class StoryboardMotion : IMotion
    {
        public struct Shot
        {
            public IMotion motion;
            public float duration;
        }

        public class Transition
        {
            public Easing.Function function;
            public float duration;
        }

        public List<Shot> shots;
        public bool repeat;
        public Transition transition;

        private int index = 0;
        private double elapsedTime = 0;
        private TweenMotion tween;
        private Transform transform;

        private IMotion lookAt;

        public IMotion LookAt { set => lookAt = value; }

        public StoryboardMotion()
        {
            shots = new List<StoryboardMotion.Shot>();
            transform = new GameObject().transform;
        }

        private Shot CurrentShot
        {
            get => shots[index];
        }

        private Shot NextShot
        {
            get => shots[NextIndex()];
        }

        private int NextIndex()
        {
            int nextIndex = index + 1;
            if (nextIndex >= shots.Count)
            {
                nextIndex = 0;
            }
            return nextIndex;
        }

        private Transform CurrentTransform(double deltaTime)
        {
            elapsedTime += deltaTime;

            if (tween != null)
            {
                if (elapsedTime <= tween.duration)
                {
                    return tween.Transform(deltaTime);
                }
                else
                {
                    tween = null;
                    index = NextIndex();
                }
            }
            else
            {
                if (elapsedTime > CurrentShot.duration)
                {
                    elapsedTime -= CurrentShot.duration;
                    if (transition != null)
                    {
                        NextShot.motion.Reset();
                        tween = new TweenMotion { delay = 0, duration = transition.duration, function = transition.function, from = CurrentShot.motion, to = NextShot.motion };
                        return tween.Transform(deltaTime);
                    }
                    else
                    {
                        index = NextIndex();
                        CurrentShot.motion.Reset();
                    }
                }
            }

            return CurrentShot.motion.Transform(deltaTime);
        }

        public Transform Transform(double deltaTime)
        {
                if (shots.Count == 0)
                {
                    return transform;
                }

                Transform currentTransform = CurrentTransform(deltaTime);

                transform.position = currentTransform.position;
                transform.rotation = currentTransform.rotation;

                if (lookAt != null)
                {
                    transform.LookAt(lookAt.Transform(deltaTime));
                }

                return transform;
        }

        public void Reset()
        {
            index = 0;
            elapsedTime = 0;

            if (shots.Count > 0)
            {
                CurrentShot.motion.Reset();
            }

            if (lookAt != null)
            {
                lookAt.Reset();
            }
        }
    }
}
