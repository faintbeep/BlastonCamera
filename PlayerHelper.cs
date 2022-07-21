using UnityEngine;

namespace BlastonCameraBehaviour
{
    public interface IPlayerHelper
    {
        Transform playerWaist { get; }
        Transform playerRightHand { get; }
        Transform playerLeftHand { get; }
        Transform playerLeftFoot { get; }
        Transform playerHead { get; }
        Transform playerRightFoot { get; }
    }

    public class PlayerHelper : IPlayerHelper
    {
        private PluginCameraHelper _pluginCameraHelper;
        public PlayerHelper(PluginCameraHelper pluginCameraHelper)
        {
            _pluginCameraHelper = pluginCameraHelper;
        }

        public Transform playerWaist { get => _pluginCameraHelper.playerWaist; }
        public Transform playerRightHand { get => _pluginCameraHelper.playerRightHand; }
        public Transform playerLeftHand { get => _pluginCameraHelper.playerLeftHand; }
        public Transform playerLeftFoot { get => _pluginCameraHelper.playerLeftFoot; }
        public Transform playerHead { get => _pluginCameraHelper.playerHead; }
        public Transform playerRightFoot { get => _pluginCameraHelper.playerRightFoot; }
    }
}
