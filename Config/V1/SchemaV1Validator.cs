using System;
using System.Collections.Generic;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;

namespace BlastonCameraBehaviour.Config
{
    public class SchemaV1Validator
    {
        public static string VectorSchema = @"{
            'type': 'object',
            'properties': {
                'x': { 'type': 'number' },
                'y': { 'type': 'number' },
                'z': { 'type': 'number' }
            },
            'required': ['x', 'y', 'z'],
            'additionalProperties': false
        }";

        public static string StaticMotionSchema = @"{
            'type': 'object',
            'properties': {
                'type': { 'const': 'static' },
                'position': {
                    'oneOf': [
                        { 'type': 'string' },
                        { '$ref': 'vector.json' }
                    ]
                },
                'lookAt': { '$ref': 'motion.json' }
            },
            'required': ['type', 'position']
        }";

        public static string MotionSchema = @"{
            'oneOf': [
                { '$ref': 'staticMotion.json' },
                { '$ref': 'tweenMotion.json' },
                { '$ref': 'storyboardMotion.json' },
                { '$ref': 'playerMotion.json' },
                { '$ref': 'orbitMotion.json' }
            ]
        }";

        public static string EasingSchema = @"{
            'enum': [
                'linear',
                'easeInSine',
                'easeOutSine',
                'easeInOutSine',
                'easeInQuad',
                'easeOutQuad',
                'easeInOutQuad',
                'easeInCubic',
                'easeOutCubic',
                'easeInOutCubic',
                'easeInBounce',
                'easeOutBounce',
                'easeInOutBounce'
            ]
        }";

        public static string StoryboardMotionSchema = @"{
            'type': 'object',
            'properties': {
                'type': { 'const': 'storyboard' },
                'shots': {
                    'type': 'array',
                    'items': {
                        'allOf': [
                            { '$ref': 'motion.json' },
                            { '$ref': '#/$defs/shot' }
                        ]
                    }
                },
                'transition': { '$ref': '#/$defs/transition' },
                'repeat': { 'type': 'boolean' },
                'lookAt': { '$ref': 'motion.json' },
            },
            'required': ['type', 'shots'],
            '$defs': {
                'shot': {
                    'type': 'object',
                    'properties': {
                        'duration': { 'type': 'number' }
                    },
                    'required': ['duration']
                },
                'transition': {
                    'type': 'object',
                    'properties': {
                        'function': { '$ref': 'easing.json' },
                        'duration': { 'type': 'number' }
                    },
                    'required': ['function', 'duration']
                }
            },
        }";

        public static string TweenMotionSchema = @"{
            'type': 'object',
            'properties': {
                'type': { 'const': 'tween' },
                'from': { '$ref': 'motion.json' },
                'to': { '$ref': 'motion.json' },
                'function': { '$ref': 'easing.json' },
                'delay': { 'type': 'number' },
                'transitionDuration': { 'type': 'number' },
                'lookAt': { '$ref': 'motion.json' },
            },
            'required': ['from', 'to', 'function', 'delay', 'transitionDuration']
        }";

        public static string OrbitMotionSchema = @"{
            'type': 'object',
            'properties': {
                'type': { 'const': 'orbit' },
                'center': { '$ref': 'motion.json' },
                'offset': { '$ref': 'vector.json' },
                'direction': { '$ref': '#/$defs/vector2' },
                'speed': { 'type': 'number' }
            },
            'required': ['type', 'center', 'offset', 'direction', 'speed'],
            '$defs': {
                'vector2': {
                    'type': 'object',
                    'properties': {
                        'x': { 'type': 'number' },
                        'y': { 'type': 'number' }
                    },
                    'required': ['x', 'y']
                }
            }
        }";

        public static string PlayerMotionSchema = @"{
            'type': 'object',
            'properties': {
                'type': { 'const': 'player' },
                'bodyPart': {
                    'enum': [
                        'head',
                        'waist',
                        'leftHand',
                        'rightHand',
                        'leftFoot',
                        'rightFoot'
                    ]
                },
                'offset': { '$ref': 'vector.json' },
                'lookAt': { '$ref': 'motion.json' }
            },
            'required': ['type', 'bodyPart']
        }";

        public static string ConfigV1Schema = @"{
            'type': 'object',
            'properties': {
                'version': { 'const': 1 },
                'positions': {
                    'type': 'object',
                    'patternProperties': {
                        '^.*$': { '$ref': 'vector.json' }
                    }
                },
                'motions': {
                    'type': 'object',
                    'patternProperties': {
                        '^.*$': { '$ref': 'motion.json' }
                    }
                }
            },
            'required': ['version', 'positions', 'motions']
        }";

        private JSchemaPreloadedResolver resolver;
        private JSchema schema;
        public JSchema Schema { get => schema; }

        public SchemaV1Validator()
        {
            resolver = new JSchemaPreloadedResolver();
            resolver.Add(new Uri("vector.json", UriKind.RelativeOrAbsolute), VectorSchema);
            resolver.Add(new Uri("easing.json", UriKind.RelativeOrAbsolute), EasingSchema);
            resolver.Add(new Uri("storyboardMotion.json", UriKind.RelativeOrAbsolute), StoryboardMotionSchema);
            resolver.Add(new Uri("staticMotion.json", UriKind.RelativeOrAbsolute), StaticMotionSchema);
            resolver.Add(new Uri("tweenMotion.json", UriKind.RelativeOrAbsolute), TweenMotionSchema);
            resolver.Add(new Uri("playerMotion.json", UriKind.RelativeOrAbsolute), PlayerMotionSchema);
            resolver.Add(new Uri("orbitMotion.json", UriKind.RelativeOrAbsolute), OrbitMotionSchema);
            resolver.Add(new Uri("motion.json", UriKind.RelativeOrAbsolute), MotionSchema);
            schema = JSchema.Parse(ConfigV1Schema, resolver);
        }


        public bool IsValid(JObject json)
        {
            return json.IsValid(schema);
        }

        public bool IsValid(string schemaName, JObject json)
        {
            JSchema schema = JSchema.Parse("{ '$ref': '" + schemaName + "' }", resolver);
            return json.IsValid(schema);
        }

        public bool IsValid(string schemaName, JObject json, out IList<string> messages)
        {
            JSchema schema = JSchema.Parse("{ '$ref': '" + schemaName + "' }", resolver);
            return json.IsValid(schema, out messages);
        }
    }
}
