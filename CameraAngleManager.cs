using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BlastonCameraBehaviour
{
    interface ICameraAngle
    {
        Transform GetCameraPose();
    }

    class StaticCameraAngle : ICameraAngle
    {
        static Vector3 arenaCenter = new Vector3(0.0f, 0.6f, 1.6f);
        static Transform lookAt = null;

        private Transform pose;

        public StaticCameraAngle(Vector3 position)
        {
            if (lookAt == null)
            {
                lookAt = new GameObject().transform;
                lookAt.position = arenaCenter;
            }

            pose = new GameObject().transform;
            pose.position = position;
            pose.LookAt(lookAt);
        }

        public Transform GetCameraPose()
        {
            return pose;
        }
    }

    class AutoAngleManager : ICameraAngle
    {
        static Vector3 highCenter = new Vector3(2.5f, 5.0f, 1.6f);
        static Vector3 lowCenter = new Vector3(4.0f, 1.8f, 1.6f);
        static Vector3 playerSide = new Vector3(3.5f, 1.8f, -1.5f);
        static Vector3 opponentSide = new Vector3(3.5f, 1.8f, 4.7f);

        private ICameraAngle[] angles;
        private float elapsedTime;
        private float dwellTime;
        private float tweenTime;

        public AutoAngleManager(float dwell = 15.0f, float tween = 1.0f)
        {
            dwellTime = dwell;
            tweenTime = tween;

            elapsedTime = 0;

            angles = new ICameraAngle[] {
                new StaticCameraAngle(playerSide),
                new StaticCameraAngle(lowCenter),
                new StaticCameraAngle(opponentSide),
                new StaticCameraAngle(highCenter),
            };
        }

        public Transform GetCameraPose()
        {
            elapsedTime += Time.deltaTime;

            float angleTime = elapsedTime % dwellTime;
            int angle = (int)(elapsedTime / dwellTime) % angles.Length;

            Transform currentPose = angles[angle].GetCameraPose();

            if (elapsedTime > dwellTime && angleTime < tweenTime)
            {
                int previousAngle = angle > 0 ? angle - 1 : angles.Length - 1;

                Transform previousPose = angles[previousAngle].GetCameraPose();

                Transform tween = new GameObject().transform;

                float lerpRatio = EaseInOutCubic(angleTime / tweenTime);

                tween.position = Vector3.Lerp(previousPose.position, currentPose.position, lerpRatio);
                tween.rotation = Quaternion.Slerp(previousPose.rotation, currentPose.rotation, lerpRatio);

                return tween;
            }

            return currentPose;
        }

        private static float EaseInOutCubic(float x) {
            return (float)(x < 0.5 ? 4.0 * Math.Pow(x, 3) : 1.0 - Math.Pow(-2 * x + 2, 3) / 2.0);
        }
    }
}
