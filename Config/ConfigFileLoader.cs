using System.IO;
using Newtonsoft.Json.Linq;


namespace BlastonCameraBehaviour.Config
{
    using System;

    public class ConfigException : Exception
    {
        public ConfigException(string message): base(message) { }

        public ConfigException(string message, Exception inner): base(message, inner) { }

        public ConfigException(JToken token, string message) : base(message + " at " + token.Path) { }
    }

    public class ConfigFileLoader
    {
        public static Config LoadFile(IPlayerHelper helper, string filepath)
        {
            string configText = LoadFileToString(filepath);
            ConfigJsonV1Loader loader = new ConfigJsonV1Loader(helper, configText);
            return loader.Config;
        }

        private static string LoadFileToString(string filepath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filepath))
                {
                    return sr.ReadToEnd();
                }
            } catch (Exception ex)
            {
                throw new ConfigException("Could not open config file", ex);
            }
        }
    }
}
