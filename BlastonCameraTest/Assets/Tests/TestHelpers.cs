using UnityEngine;
using BlastonCameraBehaviour.Motions;

public class TestHelpers
{
    public class TestMotion : IMotion
    {
        public Transform transform;

        public TestMotion(Vector3 position, Quaternion rotation)
        {
            transform = new GameObject().transform;
            transform.position = position;
            transform.rotation = rotation;
        }

        public Transform Transform(double deltaTime)
        {
            return transform;
        }

        public IMotion LookAt { set { } }

        public void Reset() { }
    }
    public static bool QuaternionEquals(Quaternion q1, Quaternion q2)
    {
        return q1.Equals(q2) || q1 == q2;
    }
}
