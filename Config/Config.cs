using System.Collections.Generic;
using UnityEngine;
using BlastonCameraBehaviour.Motions;

namespace BlastonCameraBehaviour.Config
{
    public struct Config
    {
        public Dictionary<string, Vector3> positions;
        public Dictionary<string, IMotion> motions;
    }
}
