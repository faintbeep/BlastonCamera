using UnityEngine;

namespace BlastonCameraBehaviour.Motions
{
    public class StaticMotion : IMotion
    {
        private IMotion lookAt;
        private Transform transform;

        public Vector3 Position { set => transform.position = value; }
        public IMotion LookAt { set => lookAt = value; }

        public StaticMotion()
        {
            transform = new GameObject().transform;
        }

        public Transform Transform(double deltaTime)
        {
                if (lookAt != null)
                {
                    transform.LookAt(lookAt.Transform(deltaTime));
                }
                return transform;
        }

        public void Reset() {
            if (lookAt != null)
            {
                lookAt.Reset();
            }
        }
    }
}
