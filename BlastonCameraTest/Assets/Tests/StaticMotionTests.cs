using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using BlastonCameraBehaviour.Motions;

public class StaticMotionTests
{

    // A Test behaves as an ordinary method
    [Test]
    public void PositionTest()
    {
        StaticMotion motion = new StaticMotion() { Position = new Vector3(1.0f, 2.0f, 3.0f) };

        Transform t = motion.Transform(1.0);

        Assert.AreEqual(t.position.x, 1.0f, "x positions should be equal");
        Assert.AreEqual(t.position.y, 2.0f, "y positions should be equal");
        Assert.AreEqual(t.position.z, 3.0f, "z positions should be equal");
    }

    [Test]
    public void RotationTest()
    {
        StaticMotion motion = new StaticMotion() { Position = new Vector3(1.0f, 2.0f, 3.0f) };

        Assert.That(motion.Transform(1.0).rotation.Equals(Quaternion.identity), "rotation without lookAt should be identity");
    }

    [Test]
    public void LookAtTest()
    {
        var lookAt = new StaticMotion() { Position = new Vector3(1.0f, 0.0f, 0.0f) };
        var motion = new StaticMotion() { Position = new Vector3(0.0f, 0.0f, 0.0f), LookAt = lookAt };

        Assert.That(TestHelpers.QuaternionEquals(motion.Transform(1.0).rotation, Quaternion.AngleAxis(90, Vector3.up)), "rotation should look to the right");
    }
}