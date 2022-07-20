using UnityEngine;

namespace BlastonCameraBehaviour.Motions
{
    public class TweenMotion : IMotion
    {
        public IMotion from;
        public IMotion to;
        public Easing.Function function;
        public float delay;
        public float duration;
        private IMotion lookAt;

        private double elapsedTime;
        private Transform transform;
        public IMotion LookAt { set => lookAt = value; }


        public TweenMotion()
        {
            elapsedTime = 0;
            transform = new GameObject().transform;
        }

        public Transform Transform(double deltaTime)
        {
            elapsedTime += deltaTime;

            float ratio = ((float)elapsedTime - delay) / duration;

            Transform fromTransform = from.Transform(deltaTime);
            Transform toTransform = to.Transform(deltaTime);

            float lerpRatio = Easing.Ease(function, ratio);

            transform.position = Vector3.Lerp(fromTransform.position, toTransform.position, lerpRatio);
            transform.rotation = Quaternion.Slerp(fromTransform.rotation, toTransform.rotation, lerpRatio);

            if (lookAt != null)
            {
                transform.LookAt(lookAt.Transform(deltaTime));
            }

            return transform;
        }

        public void Reset()
        {
            elapsedTime = 0;
            from.Reset();
            to.Reset();
            if (lookAt != null)
            {
                lookAt.Reset();
            }
        }
    }
}
