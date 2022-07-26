# FaintBeep's Blaston Camera

A configurable LIV camera plugin for Blaston (and other games). The default config file cycles between 4 viewpoints which work for Blaston.

Place `BlastonCameraBehaviour.dll` and `BlastonCamera.json` in `Documents\LIV\Plugins\CameraBehaviours` and select "BlastonCamera" under "Plugins" from LIV.

You can edit the config file to configure the camera. Deselecting and reselecting "BlastonCamera" inside LIV reloads the config, so you can work on it without quitting the game.

## Config file format

If you like json schemas, [see the code here](./Config/V1/SchemaV1Validator.cs).

### The main BlastonCamera.json
```json
{
    "version": 1,
    "positions": {
        // Add favourite position vectors here so they can be referenced by name below
    },
    "motions": {
        "default": {
            // This is the "motion" which the camera will use.
            // This is usually a storyboard motion
        }
        // You can store other named motions here which will be ignored
    }
}
```

### Positions

This is an object with names as keys and 3d vectors as values. Example:

```json
{
    "arenaCenter": { "x": 0.0, "y": 0.6, "z": 1.6 },
    "playerSide": { "x": 3.5, "y": 1.8, "z": -1.6 },
    "opponentSide": { "x": 3.5, "y": 1.8, "z": 4.8 }
}
```

### Motions

#### Static motion

This is a static camera. It has a position. Example:

```json
{
    "type": "static",
    "position": { "x": 1.5, "y": 2.0, "z": -1.25 }
}
```

Or using a named position:

```json
{
    "type": "static",
    "position": "playerSide"
}
```

All motions can take an optional `lookAt` property, which is itself another motion:

```json
{
    "type": "static",
    "position": "playerSide",
    "lookAt": {
        "type": "static",
        "position": "arenaCenter"
    }
}
```

#### Player motion

This motion follows one of the player's body parts. Possible body parts are `head`, `waist`, `leftHand`, `rightHand`, `leftFoot`, `rightFoot`.

For example, a first person camera:

```json
{
    "type": "player",
    "bodyPart": "head"
}
```

It takes an optional `offset` vector, so for example a third person camera (1.5m to the right, above and behind the player's waist):

```json
{
    "type": "player",
    "bodyPart": "waist",
    "offset": { "x": 1.5, "y": 1.5, "z": -1.5}
}
```

Or a selfie cam held in the player's right hand and looking at their head:

```json
{
    "type": "player",
    "bodyPart": "rightHand",
    "lookAt": {
        "type": "player",
        "bodyPart": "head"
    }
}
```

#### Orbit motion

A motion that revolves around another motion, looking at the center point by default. This takes a `center` motion, an `offset` vector representing the starting distance from the center, a 2d `direction` vector and a `speed` in degrees per second.

The `direction` vector allows orbiting in any direction rather than just horizontally. If you imagine your head at the `center` point and you are looking at the `offset` point, `direction.x` is the right/left direction and `direction.y` is the up/down direction from that point of view.

So for example, starting 2m in front and orbiting the player's head horizontally in a clockwise direction:

```json
{
    "type": "orbit",
    "center": {
        "type": "player",
        "bodyPart": "head"
    },
    "offset": { "x": 0, "y": 0, "z": 2 }, // 2m in front
    "direction": { "x": 1, "y": 0 }, // To the right but not up or down
    "speed": 30
}
```

However with the same direction but a different offset this can orbit at a 45 degree angle:

```json
{
    "type": "orbit",
    "center": {
        "type": "player",
        "bodyPart": "head"
    },
    "offset": { "x", "y": -2, "z": 2 }, // 2m in front and below
    "direction": { "x": 1, "y": 0 }, // If you are looking down at the offset, then "right" takes you on an orbit up over your shoulder
    "speed": 30
}
```

This can be confusing at first but allows a lot of flexibility. The simplest option is to always choose an offset at the same y-position as the center, then direction will always make intuitive sense.

#### Tween motion

A motion that animates between two other motions, using an easing function. Available easing functions are `linear`, `easeInSine`, `easeOutSine`, `easeInOutSine`, `easeInQuad`, `easeOutQuad`, `easeInOutQuad`, `easeInCubic`, `easeOutCubic`, `easeInOutCubic`, `easeInBounce`, `easeOutBounce` and `easeInOutBounce`.

For example, a tracking shot moving between two points while looking at a third point (taking 5 seconds, after waiting on the first point for 3 seconds):

```json
{
    "type": "tween",
    "from": {
        "type": "static",
        "position": "playerSide",
    },
    "to": {
        "type": "static",
        "position": "opponentSide"
    },
    "function": "linear",
    "delay": 3,
    "transitionDuration": 5,
    "lookAt": {
        "type": "static",
        "position": "arenaCenter"
    }
}
```

Or a "bullet cam" which shoots forward from your hand, following the direction where you might be shooting:

```json
{
    "type": "tween",
    "from": {
        "type": "player",
        "bodyPart": "rightHand"
    },
    "to": {
        "type": "player",
        "bodyPart": "rightHand",
        "offset": { "x": 0, "y": 0, "z": 50 }
    },
    "delay": 1,
    "transitionDuration": 1
}
```

#### Storyboard motion

This ties all the other motions together into a playlist. The `shots` property is an array of motions, each with an extra `duration` property added for how long each shot should last.

An optional `transition` property acts like a tween between each shot (taking time away from the duration of the subsequent shot).

For example, this storyboard cycles between player and opponent sides and a first person view for 10s each, with a 2s eased transition between them:

```json
{
    "type": "storyboard",
    "shots": [
        {
            "type": "static",
            "position": "playerSide",
            "lookAt": "arenaCenter",
            "duration": 10
        },
        {
            "type": "static",
            "position": "opponentSide",
            "lookAt": "arenaCenter",
            "duration": 10
        },
        {
            "type": "player",
            "bodyPart": "head",
            "duration": 10
        }
    ],
    "transition": {
        "function": "easeInOutSine",
        "duration": 2
    },
    "repeat": true
}
```

### Composing motions

You will have noticed that many motions take other motions as options, so you can build up quite complicated behaviours.

For example, you might have a camera which spirals by orbiting around a moving track, before slowly transitioning to look at the player's face from the point of view of their feet.
