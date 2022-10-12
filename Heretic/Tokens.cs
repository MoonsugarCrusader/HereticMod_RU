using RoR2;
using System.Linq;

namespace HereticMod
{
    class Tokens
    {
        private static bool initialized = false;

        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            LoadLanguage();
        }


        private static void LoadLanguage()
        {
            On.RoR2.Language.SetFolders += fixme;
        }

        //Credits to Anreol for this code
        private static void fixme(On.RoR2.Language.orig_SetFolders orig, Language self, System.Collections.Generic.IEnumerable<string> newFolders)
        {
            if (System.IO.Directory.Exists(Assets.languageRoot))
            {
                var dirs = System.IO.Directory.EnumerateDirectories(System.IO.Path.Combine(Assets.languageRoot), self.name);
                orig(self, newFolders.Union(dirs));
                return;
            }
            orig(self, newFolders);
        }
    }
}
