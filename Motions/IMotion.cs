using UnityEngine;

namespace BlastonCameraBehaviour.Motions
{
    public interface IMotion
    {
        Transform Transform(double deltaTime);
        IMotion LookAt { set; }
        void Reset();
    }
}
