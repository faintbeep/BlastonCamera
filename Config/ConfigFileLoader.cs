using System.IO;

namespace BlastonCameraBehaviour.Config
{
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
            using (StreamReader sr = new StreamReader(filepath))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
