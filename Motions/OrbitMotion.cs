using UnityEngine;

namespace BlastonCameraBehaviour.Motions
{
    public class OrbitMotion : IMotion
    {
        private IMotion _center;
        private Vector3 _offset;
        private Vector3 _satellite;
        private float _speed;
        private Quaternion _rotation;

        private IMotion lookAt;
        private Transform transform;

        public IMotion LookAt { set => lookAt = value; }

        public OrbitMotion(IMotion center, Vector3 offset, Vector2 direction, float speed)
        {
            _center = center;
            _offset = offset;
            _satellite = offset;

            Vector3 direction3 = new Vector3(direction.x, direction.y, 0);
            direction3 = Quaternion.FromToRotation(Vector3.forward, offset) * direction3;

            Vector3 axis = Vector3.Cross(offset, direction3);
            _rotation = Quaternion.AngleAxis(179.99f, axis);
            _speed = speed;

            transform = new GameObject().transform;
        }

        public Transform Transform(double deltaTime)
        {
            _satellite = Quaternion.RotateTowards(Quaternion.identity, _rotation, _speed * (float)deltaTime) * _satellite;

            Transform center = _center.Transform(deltaTime);
            transform.position = center.position + _satellite;
            transform.LookAt(lookAt != null ? lookAt.Transform(deltaTime) : center);

            return transform;
        }

        public void Reset()
        {
            _satellite = _offset;
            _center.Reset();
            if (lookAt != null)
            {
                lookAt.Reset();
            }
        }
    }
}
