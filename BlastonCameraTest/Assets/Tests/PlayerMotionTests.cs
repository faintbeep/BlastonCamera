using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using BlastonCameraBehaviour;
using BlastonCameraBehaviour.Motions;

public class PlayerMotionTests
{
    class TestPlayerHelper : IPlayerHelper
    {
        public Transform _playerWaist;
        public Transform _playerRightHand;
        public Transform _playerLeftHand;
        public Transform _playerLeftFoot;
        public Transform _playerHead;
        public Transform _playerRightFoot;

        public Transform playerWaist { get => _playerWaist; }
        public Transform playerRightHand { get => _playerRightHand; }
        public Transform playerLeftHand { get => _playerLeftHand; }
        public Transform playerLeftFoot { get => _playerLeftFoot; }
        public Transform playerHead { get => _playerHead; }
        public Transform playerRightFoot { get => _playerRightFoot; }
    }

    [Test]
    public void FirstPersonTest()
    {
        var playerHead = new GameObject().transform;
        var helper = new TestPlayerHelper() { _playerHead = playerHead };
        var firstPerson = PlayerMotion.FirstPerson(helper);

        var transform = firstPerson.Transform(1.0f);
        Assert.That(playerHead.position == transform.position, "transform should match player head");

        playerHead.position = new Vector3(1f, 0f, 0f);
        transform = firstPerson.Transform(1f);
        Assert.That(playerHead.position == transform.position, "transform should match player head");

        playerHead.rotation = Quaternion.AngleAxis(90, Vector3.up);
        transform = firstPerson.Transform(1f);
        Assert.That(playerHead.position == transform.position, "transform should rotate with player head");
    }

    [Test]
    public void ThirdPersonTest()
    {
        var playerWaist = new GameObject().transform;
        var helper = new TestPlayerHelper() { _playerWaist = playerWaist };
        var thirdPerson = PlayerMotion.ThirdPerson(helper);
        var expectedTransform = new GameObject().transform;

        var transform = thirdPerson.Transform(1f);
        expectedTransform.position = new Vector3(1f, 1.5f, -1f);
        Assert.That(expectedTransform.position == transform.position, "transform should be behind, above, to right of player");

        playerWaist.rotation = Quaternion.AngleAxis(90, Vector3.up);
        transform = thirdPerson.Transform(1f);
        expectedTransform.position = new Vector3(-1f, 1.5f, -1f);
        Assert.That(expectedTransform.position == transform.position, "transform should rotate with player");
        Assert.That(TestHelpers.QuaternionEquals(playerWaist.rotation, transform.rotation), "transform should face same direction as player");
    }

    [Test]
    public void ArbitraryOffsetTest()
    {
        var playerLeftHand = new GameObject().transform;
        var helper = new TestPlayerHelper() { _playerLeftHand = playerLeftHand };
        var playerMotion = new PlayerMotion(helper) { bodyPart = PlayerMotion.BodyPart.leftHand };

        playerLeftHand.position = new Vector3(1f, 2f, 0.5f);
        playerLeftHand.rotation = Quaternion.AngleAxis(90, Vector3.up);
        playerMotion.offset.x = 0;
        playerMotion.offset.y = 0.5f;
        playerMotion.offset.z = -0.5f;

        var transform = playerMotion.Transform(1f);
        var expectedTransform = new GameObject().transform;
        expectedTransform.position = new Vector3(0.5f, 2.5f, 0.5f);
        expectedTransform.rotation = Quaternion.identity;
        Assert.That(expectedTransform.position == transform.position, "transform should be offset from player");
    }

    [Test]
    public void LookAtTest()
    {
        var playerHead = new GameObject().transform;
        var lookAt = new TestHelpers.TestMotion(new Vector3(0.0f, 0.0f, 1.0f), Quaternion.identity);

        var helper = new TestPlayerHelper() { _playerHead = playerHead };
        var firstPerson = PlayerMotion.FirstPerson(helper);
        firstPerson.LookAt = lookAt;

        var transform = firstPerson.Transform(1f);
        Quaternion expectedRotation = Quaternion.identity;
        Assert.That(TestHelpers.QuaternionEquals(expectedRotation, transform.rotation), "transform should look at lookAt");

        lookAt.transform.position = new Vector3(1f, 0f, 0f);
        transform = firstPerson.Transform(1f);
        expectedRotation = Quaternion.AngleAxis(90, Vector3.up);
        Assert.That(TestHelpers.QuaternionEquals(expectedRotation, transform.rotation), "transform should follow lookAt");

    }
}