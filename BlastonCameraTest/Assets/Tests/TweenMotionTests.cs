using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using BlastonCameraBehaviour;
using BlastonCameraBehaviour.Motions;

public class TweenMotionTests
{

    [Test]
    public void TweenMotionTest()
    {
        var motion1 = new TestHelpers.TestMotion(new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        var motion2 = new TestHelpers.TestMotion(new Vector3(1.0f, 0.0f, 0.0f), Quaternion.AngleAxis(90, Vector3.up));

        var tweenMotion = new TweenMotion()
        {
            delay = 1.0f,
            duration = 2.0f,
            function = Easing.Function.linear,
            from = motion1,
            to = motion2
        };

        Transform after1second = tweenMotion.Transform(1.0);
        Assert.AreEqual(after1second.position.x, 0.0f, "should not have moved after 1s");
        Assert.That(TestHelpers.QuaternionEquals(after1second.rotation, Quaternion.identity), "should face forward after 1s");

        Transform after2seconds = tweenMotion.Transform(1.0);
        Assert.AreEqual(after2seconds.position.x, 0.5f, "should have moved halfway after 2s");
        Assert.That(TestHelpers.QuaternionEquals(after2seconds.rotation, Quaternion.AngleAxis(45, Vector3.up)), "should face 45 degrees after 2s");

        Transform after3seconds = tweenMotion.Transform(1.0);
        Assert.AreEqual(after3seconds.position.x, 1.0f, "should moved all the way after 3s");
        Assert.That(TestHelpers.QuaternionEquals(after3seconds.rotation, Quaternion.AngleAxis(90, Vector3.up)), "should face 90 degrees after 3s");


        Transform after4seconds = tweenMotion.Transform(1.0);
        Assert.AreEqual(after4seconds.position.x, 1.0f, "should not have moved further after 4s");
        Assert.That(TestHelpers.QuaternionEquals(after4seconds.rotation, Quaternion.AngleAxis(90, Vector3.up)), "should face 90 degrees after 4s");
    }

    [Test]
    public void TweenMotionLookAtTest()
    {
        var motion1 = new TestHelpers.TestMotion(new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        var motion2 = new TestHelpers.TestMotion(new Vector3(1.0f, 0.0f, 0.0f), Quaternion.AngleAxis(90, Vector3.up));
        var lookAt = new TestHelpers.TestMotion(new Vector3(0.0f, 0.0f, 1.0f), Quaternion.identity);


        var tweenMotion = new TweenMotion()
        {
            delay = 1.0f,
            duration = 2.0f,
            function = Easing.Function.linear,
            from = motion1,
            to = motion2,
            LookAt = lookAt
        };

        Transform after1second = tweenMotion.Transform(1.0);
        Assert.That(TestHelpers.QuaternionEquals(after1second.rotation, Quaternion.identity), "should face forward after 1s");

        Transform after2seconds = tweenMotion.Transform(1.0);
        Assert.That(TestHelpers.QuaternionEquals(after2seconds.rotation, Quaternion.AngleAxis(-90 + Mathf.Rad2Deg * Mathf.Atan(1f / 0.5f), Vector3.up)), "should face -23 degrees after 2s");

        Transform after3seconds = tweenMotion.Transform(1.0);
        Assert.That(TestHelpers.QuaternionEquals(after3seconds.rotation, Quaternion.AngleAxis(-90 + Mathf.Rad2Deg * Mathf.Atan(1f / 1f), Vector3.up)), "should face -45 degrees after 3s");
    }
}
