using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using BlastonCameraBehaviour.Config;
using Newtonsoft.Json.Linq;

public class SchemaV1ValidatorTests
{
    [Test]
    public void StaticMotionValidator()
    {
        SchemaV1Validator validator = new SchemaV1Validator();

        string testMotion1 = @"{
            'type': 'static',
            'position': {
                'x': 0.4,
                'y': 3,
                'z': -1
            }
        }";

        Assert.IsTrue(validator.IsValid("staticMotion.json", JObject.Parse(testMotion1)), "schema should be valid");

        string testMotion2 = @"{
            'type': 'static',
            'position': 'arenaCenter'
        }";

        Assert.IsTrue(validator.IsValid("staticMotion.json", JObject.Parse(testMotion2)), "schema should be valid");


        string testMotion3 = @"{
            'type': 'static',
            'position': {
                'x': 0.4,
                'y': 3,
                'z': -1
            },
            'lookAt': 1
        }";

        Assert.IsFalse(validator.IsValid("staticMotion.json", JObject.Parse(testMotion3)), "schema should be invalid");
    }

    [Test]
    public void VectorValidator()
    {
        SchemaV1Validator validator = new SchemaV1Validator();

        JObject vector1 = JObject.Parse(@"{
            'x': 1,
            'y': 2,
            'z': 3
        }");

        Assert.IsTrue(validator.IsValid("vector.json", vector1), "object should be valid");

        JObject vector2 = JObject.Parse(@"{
            'x': 1,
            'y': '2',
            'z': 3
        }");

        Assert.IsFalse(validator.IsValid("vector.json", vector2), "text 'y' should not be valid");


        JObject vector4 = JObject.Parse(@"{
            'x': 1,
            'y': 2
        }");

        Assert.IsFalse(validator.IsValid("vector.json", vector4), "missing 'z' should not be valid");
    }

    [Test]
    public void StorybordValidator()
    {
        SchemaV1Validator validator = new SchemaV1Validator();

        JObject storyboard1 = JObject.Parse(@"{
            'type': 'storyboard',
            'shots': [
                {
                    'type': 'static',
                    'position': 'playerSide',
                    'lookAt': {
                        'type': 'static',
                        'position': 'arenaCenter'
                    },
                    'duration': 5
                },
                {
                    'type': 'static',
                    'position': 'lowCenter',
                    'lookAt': {
                        'type': 'static',
                        'position': 'arenaCenter'
                    },
                    'duration': 5
                },
                {
                    'type': 'static',
                    'position': 'opponentSide',
                    'lookAt': {
                        'type': 'static',
                        'position': 'arenaCenter'
                    },
                    'duration': 5
                },
                {
                    'type': 'static',
                    'position': 'highCenter',
                    'lookAt': {
                        'type': 'static',
                        'position': 'arenaCenter'
                    },
                    'duration': 5
                }
            ],
            'transition': {
                'function': 'easeInOutCubic',
                'duration': 1
            },
            'repeat': true
        }");

        Assert.IsTrue(validator.IsValid("storyboardMotion.json", storyboard1), "storyboard1 should be valid");

        JObject storyboard2 = JObject.Parse(@"{
            'type': 'storyboard',
            'shots': [
                {
                    'type': 'static',
                    'position': 'playerSide',
                    'lookAt': {
                        'type': 'static',
                        'position': 'arenaCenter'
                    },
                    'duration': 5
                },
                {
                    'type': 'static',
                    'position': 'lowCenter',
                    'lookAt': {
                        'type': 'static',
                        'position': 'arenaCenter'
                    },
                    'duration': 5
                },
                {
                    'type': 'static',
                    'position': 'opponentSide',
                    'lookAt': {
                        'type': 'static',
                        'position': 'arenaCenter'
                    },
                    'duration': 5
                },
                {
                    'type': 'static',
                    'position': 'highCenter',
                    'lookAt': {
                        'type': 'static',
                        'position': 'arenaCenter'
                    },
                    'duration': 5
                }
            ],
            'transition': {
                'duration': 1
            },
            'repeat': true
        }");

        Assert.IsFalse(validator.IsValid("storyboardMotion.json", storyboard2), "storyboard2 should not be valid");
    }

    [Test]
    public void ConfigValidatorTest()
    {
        SchemaV1Validator validator = new SchemaV1Validator();

        JObject config1 = JObject.Parse(@"{
            'version': 1,
            'positions': {
                'arenaCenter': { 'x': 0.0, 'y': 0.6, 'z': 1.6 },
                'highCenter': { 'x': 2.5, 'y': 5.0, 'z': 1.6 },
                'lowCenter': { 'x': 4.0, 'y': 1.8, 'z': 1.6 },
                'playerSide': { 'x': 3.5, 'y': 1.8, 'z': -1.6 },
                'opponentSide': { 'x': 3.5, 'y': 1.8, 'z': 4.8 }
            },
            'motions': {
                'default': {
                    'type': 'storyboard',
                    'shots': [
                        {
                            'type': 'static',
                            'position': 'playerSide',
                            'lookAt': {
                                'type': 'static',
                                'position': 'arenaCenter'
                            },
                            'duration': 5
                        },
                        {
                            'type': 'static',
                            'position': 'lowCenter',
                            'lookAt': {
                                'type': 'static',
                                'position': 'arenaCenter'
                            },
                            'duration': 5
                        },
                        {
                            'type': 'static',
                            'position': 'opponentSide',
                            'lookAt': {
                                'type': 'static',
                                'position': 'arenaCenter'
                            },
                            'duration': 5
                        },
                        {
                            'type': 'static',
                            'position': 'highCenter',
                            'lookAt': {
                                'type': 'static',
                                'position': 'arenaCenter'
                            },
                            'duration': 5
                        }
                    ],
                    'transition': {
                        'function': 'easeInOutCubic',
                        'duration': 1
                    },
                    'repeat': true
                }
            }
        }");


        Assert.IsTrue(validator.IsValid(config1), "config1 should be valid");
    }

    [Test]
    public void TweenMotionValidatorTest()
    {
        SchemaV1Validator validator = new SchemaV1Validator();

        JObject tween1 = JObject.Parse(@"{
            'type': 'tween',
            'from': {
                'type': 'static',
                'position': 'position1'
            },
            'to': {
                'type': 'static',
                'position': 'position2'
            },
            'function': 'linear',
            'delay': 1,
            'transitionDuration': 2
        }");

        Assert.IsTrue(validator.IsValid("tweenMotion.json", tween1), "tween1 should be valid");
    }
}
