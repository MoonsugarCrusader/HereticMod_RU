using System.Reflection;
using UnityEngine;

namespace HereticMod
{
    public class Assets
    {
        public static AssetBundle assetBundle;
        internal static string languageRoot => System.IO.Path.Combine(Assets.assemblyDir, "language");
        internal static string assemblyDir
        {
            get
            {
                return System.IO.Path.GetDirectoryName(HereticPlugin.pluginInfo.Location);
            }
        }

        public static void Init()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Heretic.hereticassetbundle"))
            {
                assetBundle = AssetBundle.LoadFromStream(stream);
            }
        }
    }
}
