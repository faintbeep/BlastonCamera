using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using BlastonCameraBehaviour.Motions;

public class OrbitMotionTests
{
    [Test]
    public void OrbitMotionTest()
    {
        var center = new TestHelpers.TestMotion(new Vector3(0, 1f, 1f), Quaternion.identity);
        var start = new Vector3(1f, 0, 0);
        var direction = new Vector2(1f, 0);
        var orbitMotion = new OrbitMotion(center, start, direction, 90);

        var transform = orbitMotion.Transform(0);
        var expectedTransform = new GameObject().transform;
        expectedTransform.position = new Vector3(1f, 1f, 1f);
        expectedTransform.rotation = Quaternion.AngleAxis(-90, Vector3.up);
        Assert.That(expectedTransform.position == transform.position, "should start in start position");
        Assert.That(TestHelpers.QuaternionEquals(expectedTransform.rotation, transform.rotation), "should look at center");

        transform = orbitMotion.Transform(1f);
        expectedTransform.position = new Vector3(0, 1f, 0);
        expectedTransform.rotation = Quaternion.identity;
        Assert.That(expectedTransform.position == transform.position, "should rotate around center");
        Assert.That(TestHelpers.QuaternionEquals(expectedTransform.rotation, transform.rotation), "should look at center");

        transform = orbitMotion.Transform(1f);
        expectedTransform.position = new Vector3(-1f, 1f, 1f);
        expectedTransform.rotation = Quaternion.AngleAxis(90, Vector3.up);
        Assert.That(expectedTransform.position == transform.position, "should rotate around center");
        Assert.That(TestHelpers.QuaternionEquals(expectedTransform.rotation, transform.rotation), "should look at center");
    }

    [Test]
    public void OrbitDirectionTest()
    {
        var center = new TestHelpers.TestMotion(new Vector3(0, 1f, 0), Quaternion.identity);
        var offset = new Vector3(0, -1f, 0);
        var direction = new Vector2(0, 1f);
        var orbitMotion = new OrbitMotion(center, offset, direction, 90);

        var transform = orbitMotion.Transform(1f);
        var expected = new GameObject().transform;
        expected.position = new Vector3(0, 1f, 1f);
        expected.rotation = Quaternion.AngleAxis(180, Vector3.up);
        Assert.That(expected.position == transform.position, "should rotate vertically around center");
        Assert.That(TestHelpers.QuaternionEquals(expected.rotation, transform.rotation), "should look at center");
    }

    [Test]
    public void LookAtTest()
    {
        var center = new TestHelpers.TestMotion(new Vector3(0, 1f, 0), Quaternion.identity);
        var offset = new Vector3(0, -1f, 0);
        var direction = new Vector2(0, 1f);
        var lookAt = new TestHelpers.TestMotion(new Vector3(1f, 1f, 1f), Quaternion.identity);
        var orbitMotion = new OrbitMotion(center, offset, direction, 90) { LookAt = lookAt };

        var transform = orbitMotion.Transform(1f);
        var expected = new GameObject().transform;
        expected.position = new Vector3(0, 1f, 1f);
        expected.rotation = Quaternion.AngleAxis(90, Vector3.up);
        Assert.That(TestHelpers.QuaternionEquals(expected.rotation, transform.rotation), "should look at lookAt");
    }
}
