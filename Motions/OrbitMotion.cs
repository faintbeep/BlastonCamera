using UnityEngine;

namespace BlastonCameraBehaviour.Motions
{
    public class OrbitMotion : IMotion
    {
        private IMotion _center;
        private Vector3 _offset;
        private float _speed;
        private Vector3 axis;
        private float elapsedTime;

        private IMotion lookAt;
        private Transform transform;

        public IMotion LookAt { set => lookAt = value; }

        public OrbitMotion(IMotion center, Vector3 offset, Vector2 direction, float speed)
        {
            elapsedTime = 0;

            _center = center;
            _offset = offset;
            _speed = speed;

            Vector3 direction3 = new Vector3(direction.x, direction.y, 0);
            direction3 = Quaternion.FromToRotation(Vector3.forward, offset) * direction3;
            axis = Vector3.Cross(offset, direction3);

            transform = new GameObject().transform;
        }

        public Transform Transform(double deltaTime)
        {
            elapsedTime += (float)deltaTime;

            Quaternion rotation = Quaternion.AngleAxis(_speed * (float)elapsedTime, axis);

            Vector3 position = rotation * _offset;

            Transform center = _center.Transform(deltaTime);
            transform.position = center.position + position;
            transform.LookAt(lookAt != null ? lookAt.Transform(deltaTime) : center);

            return transform;
        }

        public void Reset()
        {
            elapsedTime = 0;
            _center.Reset();

            if (lookAt != null)
            {
                lookAt.Reset();
            }
        }
    }
}
