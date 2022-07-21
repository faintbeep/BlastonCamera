using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using UnityEngine;
using BlastonCameraBehaviour.Motions;

namespace BlastonCameraBehaviour.Config
{
    public class ConfigJsonV1Loader
    {
        private IPlayerHelper _helper;
        private Config config;
        public Config Config { get => config; }

        public ConfigJsonV1Loader(IPlayerHelper helper, string jsonText)
        {
            SchemaV1Validator validator = new SchemaV1Validator();
            JObject configObject = JObject.Parse(jsonText);
            IList<string> messages;
            if (!configObject.IsValid(validator.Schema, out messages))
            {
                foreach(var message in messages)
                {
                    Debug.LogError(message);
                }
                throw new ConfigException("Invalid config");
            }

            _helper = helper;

            JObject positionsToken = (JObject)configObject.Property("positions").Value;
            config.positions = PositionsFromJson(positionsToken);

            JObject motionsToken = (JObject)configObject.Property("motions").Value;
            config.motions = MotionsFromJson(motionsToken);
        }


        private Dictionary<string, Vector3> PositionsFromJson(JObject obj)
        {
            Dictionary<string, Vector3> positionDict = new Dictionary<string, Vector3>();

            foreach (JProperty position in obj.Properties())
            {
                positionDict.Add(position.Name, GetVector3(position.Value));
            }

            return positionDict;
        }

        private Dictionary<string, IMotion> MotionsFromJson(JObject obj)
        {
            Dictionary<string, IMotion> motionDict = new Dictionary<string, IMotion>();

            foreach (JProperty motion in obj.Properties())
            {
                motionDict.Add(motion.Name, MotionFromJson((JObject)motion.Value));
            }

            return motionDict;
        }

        public IMotion MotionFromJson(JObject obj)
        {
            IMotion motion;
            string type = obj.Value<string>("type");

            switch (type)
            {
                case "static":
                    motion = StaticMotionFromJson(obj);
                    break;
                case "storyboard":
                    motion = StoryboardMotionFromJson(obj);
                    break;
                case "tween":
                    motion = TweenMotionFromJson(obj);
                    break;
                case "player":
                    motion = PlayerMotionFromJson(obj);
                    break;
                case "orbit":
                    motion = OrbitMotionFromJson(obj);
                    break;
                default:
                    throw new ConfigException(obj.Property("type"), "Motion must have a known type");
            }

            JProperty lookAtToken = obj.Property("lookAt");
            if (lookAtToken != null)
            {
                motion.LookAt = MotionFromJson((JObject)lookAtToken.Value);
            }

            return motion;
        }

        public OrbitMotion OrbitMotionFromJson(JObject obj)
        {
            IMotion center = MotionFromJson((JObject)obj.Property("center").Value);
            Vector3 offset = GetVector3(obj.Property("offset").Value);
            Vector2 direction = GetVector2(obj.Property("direction").Value);
            float speed = obj.Value<float>("speed");

            return new OrbitMotion(center, offset, direction, speed);
        }

        public PlayerMotion PlayerMotionFromJson(JObject obj)
        {
            PlayerMotion motion = new PlayerMotion(_helper);

            JProperty offset = obj.Property("offset");
            if (offset != null)
            {
                motion.offset = GetVector3(offset.Value);
            }

            string bodyPart = obj.Value<string>("bodyPart");
            foreach (PlayerMotion.BodyPart value in Enum.GetValues(typeof(PlayerMotion.BodyPart)))
            {
                if (bodyPart == value.ToString())
                {
                    motion.bodyPart = value;
                    return motion;
                }
            }

            throw new ConfigException(obj.Property("bodyPart").Value, "PlayerMotion must use a known bodyPart");
        }

        public TweenMotion TweenMotionFromJson(JObject obj)
        {
            TweenMotion motion = new TweenMotion()
            {
                from = MotionFromJson((JObject)obj.Property("from").Value),
                to = MotionFromJson((JObject)obj.Property("to").Value),
                delay = obj.Value<float>("delay"),
                duration = obj.Value<float>("transitionDuration")
            };

            var fn = obj.Value<string>("function");
            foreach (Easing.Function value in Enum.GetValues(typeof(Easing.Function)))
            {
                if (fn == value.ToString())
                {
                    motion.function = value;
                    return motion;
                }
            }

            throw new ConfigException(obj.Property("function").Value, "TweenMotion must use a known function");
        }

        public StaticMotion StaticMotionFromJson(JObject obj)
        {
            JToken position = obj.Property("position").Value;

            switch (position.Type)
            {
                case JTokenType.Object:
                    return new StaticMotion { Position = GetVector3(position) };
                case JTokenType.String:
                    string positionName = position.ToObject<string>();
                    try
                    {
                        return new StaticMotion { Position = config.positions[positionName] };
                    }
                    catch (KeyNotFoundException)
                    {
                        throw new ConfigException(position, "Position " + positionName + " not found");
                    }
                default:
                    throw new ConfigException(position, "Position is unknown type");
            }
        }

        public StoryboardMotion.Transition TransitionFromJson(JObject obj)
        {
            StoryboardMotion.Transition transition = new StoryboardMotion.Transition
            {
                duration = obj.Value<float>("duration")
            };

            var fn = obj.Value<string>("function");
            foreach (Easing.Function value in Enum.GetValues(typeof(Easing.Function)))
            {
                if (fn == value.ToString())
                {
                    transition.function = value;
                    return transition;
                }
            }

            throw new ConfigException(obj.Property("function").Value, "Transition must use a known function");
        }

        public StoryboardMotion.Shot ShotFromJson(JObject obj)
        {
            StoryboardMotion.Shot shot = new StoryboardMotion.Shot { motion = MotionFromJson(obj), duration = obj.Value<float>("duration") };

            return shot;
        }

        public StoryboardMotion StoryboardMotionFromJson(JObject obj)
        {
            StoryboardMotion storyboard = new StoryboardMotion();

            JProperty transitionProperty = obj.Property("transition");
            if (transitionProperty != null)
            {
                storyboard.transition = TransitionFromJson((JObject)transitionProperty.Value);
            }

            JProperty repeatProperty = obj.Property("repeat");
            if (repeatProperty == null)
            {
                storyboard.repeat = true;
            }
            else
            {
                storyboard.repeat = repeatProperty.Value.ToObject<bool>();
            }

            JArray shots = (JArray)obj.Property("shots").Value;

            foreach (JToken shotToken in shots)
            {
                storyboard.shots.Add(ShotFromJson((JObject)shotToken));
            }

            return storyboard;
        }

        public static Vector3 GetVector3(JToken token)
        {
            JObject obj = (JObject)token;

            float x = obj.Value<float>("x");
            float y = obj.Value<float>("y");
            float z = obj.Value<float>("z");

            return new Vector3(x, y, z);
        }
        public static Vector2 GetVector2(JToken token)
        {
            JObject obj = (JObject)token;

            float x = obj.Value<float>("x");
            float y = obj.Value<float>("y");

            return new Vector2(x, y);
        }
    }
}
