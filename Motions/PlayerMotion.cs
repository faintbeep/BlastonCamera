using UnityEngine;

namespace BlastonCameraBehaviour.Motions
{
    public class PlayerMotion : IMotion
    {
        public enum BodyPart
        {
            head,
            waist,
            rightHand,
            leftHand,
            rightFoot,
            leftFoot
        };

        public BodyPart bodyPart;
        public Vector3 offset;
        private IPlayerHelper _helper;
        private Transform transform;
        private IMotion lookAt;

        public IMotion LookAt { set => lookAt = value; }

        public PlayerMotion(IPlayerHelper helper)
        {
            _helper = helper;
            offset = new Vector3(0, 0, 0);
            transform = new GameObject().transform;
        }

        public static PlayerMotion FirstPerson(IPlayerHelper helper)
        {
            return new PlayerMotion(helper) { bodyPart = BodyPart.head };
        }

        public static PlayerMotion ThirdPerson(IPlayerHelper helper)
        {
            Vector3 offset = new Vector3(1f, 1.5f, -1f);
            return new PlayerMotion(helper) { bodyPart = BodyPart.waist, offset = offset };
        }

        public Transform Transform(double deltaTime)
        {
            Transform bodyPartTransform;

            switch (bodyPart)
            {
                default:
                case BodyPart.head:
                    bodyPartTransform = _helper.playerHead;
                    break;
                case BodyPart.waist:
                    bodyPartTransform = _helper.playerWaist;
                    break;
                case BodyPart.rightHand:
                    bodyPartTransform = _helper.playerRightHand;
                    break;
                case BodyPart.leftHand:
                    bodyPartTransform = _helper.playerLeftHand;
                    break;
                case BodyPart.rightFoot:
                    bodyPartTransform = _helper.playerRightFoot;
                    break;
                case BodyPart.leftFoot:
                    bodyPartTransform = _helper.playerLeftFoot;
                    break;
            }

            transform.rotation = bodyPartTransform.rotation;
            transform.position = bodyPartTransform.position + bodyPartTransform.rotation * offset;

            if (lookAt != null)
            {
                transform.LookAt(lookAt.Transform(deltaTime));
            }

            return transform;
        }

        public void Reset()
        {
            if (lookAt != null)
            {
                lookAt.Reset();
            }
        }
    }
}
